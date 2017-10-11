using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SepcReptile
{


    /// <summary>
    /// 为了简化模型，下载和处理在同一个线程中完成
    /// 9.12：数据库要求提供原始url和未完成的功能
    ///     新增状态变化日志，实时提供状态
    ///     url下载后储存，以guid作为文件名，储存在url库
    ///     数据库写和网络读分离以提高性能
    ///     每次解析返回更细粒度的datarow，待数量一定后一次写入
    /// </summary>
    public abstract class Reptile
    {
        protected readonly ManualResetEvent SuspendEvent;
        protected SqlWorkUnit Database;
        public bool Work { get; set; }
        public bool Suspending { get; set; }
        protected object UrlLocker, SaveLocker, CountLocker;
        protected DataTable Url { get; set; }
        protected int UrlHand, NetSpeed;
        /// <summary>
        /// 子类需要初始化datatable的列定义
        /// </summary>
        protected DataTable CommonStructureTable { get; set; }
        protected int PreRowsCount { get; set; }
        public int MaxRowsCount { get; set; }   
        protected int MaxTdCount,leaveThread,Interval;//连接超时
        public int CurrentTdCount { get; set; }     
        public event Action WorkFlowCompleted, ProcessStopped, WorkFlowEnded;
        public event Action<string> OnUrlError;
        /// <summary>
        /// 下载速度，当前行序号，总行数      
        /// </summary>  
        public Action<int, int, int> SpeedReported;
        protected XmlDocument ConfigurationDoc;
        protected string ConfigPath;
        protected abstract DataRow[] Deal(string htmlPage);
        protected Queue<ReptileWorkFlow> WorkFlows;
        protected ReptileWorkFlow PreWorkFlow;
        public int LeaveThread
        {
            get => leaveThread;
            set
            {
                leaveThread = value;
                if (value == MaxTdCount)
                    Reptile_OnDealEnd();
            }
        }

        protected abstract void TableIni();

        public Reptile()
        {
            Suspending = false;
            UrlLocker = new object();
            SaveLocker = new object();
            CountLocker = new object();
            WorkFlows = new Queue<ReptileWorkFlow>();
            SuspendEvent = new ManualResetEvent(false);
            UrlHand = 0;
            Work = false;
            Interval = 1800;
            PreRowsCount = 0;
            MaxRowsCount = 500;
        }

        private void Reptile_OnDealEnd()
        {
            //更新url表
            Database.Update(Url);
            //储存未写入数据
            if (CommonStructureTable.Rows.Count > 0)
            {
                Database.Save(CommonStructureTable, PreWorkFlow.ValTable, PreWorkFlow.ValColumns);
            }
            //reflash config
            var xnl = ConfigurationDoc.GetElementsByTagName("workflow");
            //工作流更新
            if (Work)
            {
                PreWorkFlow.Enable = false;
                foreach (XmlNode xn in xnl)
                {
                    if (xn.Attributes["sn"].Value == PreWorkFlow.Sn.ToString())
                    {
                        xn.Attributes["enable"].Value = false.ToString();
                        ConfigurationDoc.Save(ConfigPath);
                        break;
                    }
                }
                Work = false;
                if (WorkFlows.Count > 0)//切换工作流
                {
                    WorkFlowEnded?.Invoke();
                    StartWorkFolow();
                }
                else
                {
                    WorkFlowCompleted?.Invoke();
                }
            }
            else
            {
                ProcessStopped?.Invoke();
            }
        }

        public virtual void Start(int max)
        {
            if (Work)
                return;
            MaxTdCount = max;
            StartWorkFolow();
        }

        public virtual void End()
        {
            PageDownloadEnd();
        }

        public virtual void Suspend()
        {
            Suspending = true;
        }

        public virtual void Resume()
        {
            Suspending = false;
            SuspendEvent.Set();
        }

        protected virtual void ConfigIni(string config)
        {
            ConfigPath = config;
            var xd = new XmlDocument();
            xd.Load(config);
            ConfigurationDoc = xd;
            var root = xd.DocumentElement;
            var datapath = root.Attributes["database"].Value;
            Database = new SqlWorkUnit(datapath, @".\SQLEXPRESS");
            var xnl = root.GetElementsByTagName("workflow");
            foreach (XmlElement x in xnl)
            {
                var wf = new ReptileWorkFlow() { Enable = bool.Parse(x.Attributes["enable"].Value), Sn = int.Parse(x.Attributes["sn"].Value) };
                var xn = x.GetElementsByTagName("URL")[0];
                wf.UrlColumn = xn.Attributes["column"].Value;
                wf.UrlSql = xn.Attributes["fromsql"].Value;
                wf.Sign = xn.Attributes["sign"].Value;
                var valuele = (XmlElement)x.GetElementsByTagName("Value")[0];
                wf.ValTable = valuele.Attributes["table"].Value;
                var columns = valuele.GetElementsByTagName("column");
                wf.ValColumns = new List<string>();
                foreach (XmlNode column in columns)
                {
                    wf.ValColumns.Add(column.InnerText);
                }
                WorkFlows.Enqueue(wf);
            }
            while (WorkFlows.Count > 0)
            {
                PreWorkFlow = WorkFlows.Dequeue();
                if (PreWorkFlow.Enable)
                    return;
            }
            throw new InvalidExpressionException("未有可用的工作流");
        }

        protected virtual void StartWorkFolow()
        {
            if (!PreWorkFlow.Enable)
            {
                PreWorkFlow = null;
                while (WorkFlows.Count > 0)
                {
                    PreWorkFlow = WorkFlows.Dequeue();
                    if (PreWorkFlow.Enable)
                        break;
                }
                if (PreWorkFlow == null) {
                    WorkFlowCompleted?.Invoke();
                    return;
                }
            }
            Url = Database.ExuSqlDataTable(PreWorkFlow.UrlSql);
            UrlHand = 0;
            TableIni();
            ReptileStart();
        }

        protected void ReptileStart()
        {
            Work = true;
            CurrentTdCount = 0;
            leaveThread = MaxTdCount;
            Task.Run(() => Monitor());
            for (var i = 0; i < MaxTdCount; ++i) {
                Task.Run(() => MainThread());
            }
        }

        protected void Monitor()
        {
            var pre = 0;
            while(Work)
            {
                Thread.Sleep(1000);
                NetSpeed -= pre;
                pre = NetSpeed;

                int urlhand;
                int urlcount;
                lock(UrlLocker)
                {
                    urlhand = UrlHand;
                    urlcount = Url.Rows.Count;
                }
                SpeedReported?.Invoke(NetSpeed, urlhand, urlcount);
            }
        }

        protected void MainThread()
        {
            var ar = new ARequest();
            string url;
            var prehand = 0;
            lock (CountLocker)
            {
                CurrentTdCount += 1;
                LeaveThread -= 1;
            }
            while (Work)
            {
                if (Suspending)
                {
                    SuspendEvent.WaitOne();
                }
                lock (UrlLocker)
                {
                    if (UrlHand < Url.Rows.Count)
                    {
                        prehand = UrlHand;
                        url = Url.Rows[UrlHand][PreWorkFlow.UrlColumn].ToString();
                        UrlHand += 1;
                    }
                    else
                    {
                        url = null;
                    }
                }
                if (url == null)
                    break;
                var times = 0;
                var page = "";
                while (times <= 5)
                {
                    page = ar.GetHtml(url);
                    times += 1;
                    if (page != null)
                        break;
                }
                if (page == null && OnUrlError != null)
                {
                    OnUrlError("获取时间过长" + "ERRORURL:" + url);
                    continue;
                }
                NetSpeed += ar.ReceiveLength / 1024;
                var drs = Deal(page);
                lock (UrlLocker)
                {
                    //表示读取成功
                    Url.Rows[prehand][PreWorkFlow.Sign] = 1;
                }
                lock (SaveLocker)
                {
                    foreach (var dr in drs)
                    {
                        CommonStructureTable.Rows.Add(dr);
                    }
                    if (CommonStructureTable.Rows.Count > MaxRowsCount)
                    {
                        //由于稳定运行时数据库出错几率极小，不予考虑
                        Database.Save(CommonStructureTable, PreWorkFlow.ValTable, PreWorkFlow.ValColumns);
                        CommonStructureTable.Rows.Clear();
                    }
                }
            }
            lock (CountLocker)
            {
                CurrentTdCount -= 1;
                LeaveThread += 1;   //线程标记回收
            }
        }

        protected void PageDownloadEnd()
        {
            Work = false;
        }

    }

}
