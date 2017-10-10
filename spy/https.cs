using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SepcReptile
{

    /*
     * 爬虫过程包括URL生成-获取网页-正则-放入数据库
     * URL可能包括按照规则生成和在新的页面里抓取
     * 对于在需要新的页面里抓取地址的网页，第一次不抓取，只作为源保存
     * 因此设计这样一种通用的爬虫类：设定待抓取结果列+可能需要二次抓取的结果
     * 设计一种通用的正则操作，譬如比如让正则表达式得到的值直接就是结果
     * 然后就可以放入数据库了，在dal层建立一种通用的创建列规则
     * 特殊情况：1.没有确定的搜索结束，所以开始就无法生成确定的待抓取url集合
     * 对于许多网站，有页面显示带抓取数量，可以当做二次搜索，对于其他情况
     * 设计一个调度器，调度器能对所有的功能类结果进行检索和控制
     * 按照流程调用各个功能以实现灵活性,称为“工作流”
     */
    public class WorkFlow
    {
        public string DataBase { get; set; }
        public string FromSql { get; set; }
        public string UrlColumn { get; set; }
        public string ValTable { get; set; }
        public bool Enable { get; set; }
        public List<string> ValColumns { get; set; }
        public int Sn { get; set; }
        public string Sign { get; set; }
    }
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
        protected ManualResetEvent Suspendsign;
        protected SqlWorkUnit Database;
        public bool Work { get; set; }
        public bool Suspending { get; set; }
        protected object UrlLocker, SaveLocker, CountLocker;
        protected DataTable Url { get; set; }
        protected int UrlHand, NetSpeed;
        /// <summary>
        /// 子类需要初始化datatable的架构定义
        /// </summary>
        protected DataTable CommonStructureTable { get; set; }
        protected int PreRowsCount { get; set; }
        public int MaxRowsCount { get; set; }
        protected int MaxTdCount,leaveThread,Interval;//连接超时
        public int CurrentTdCount { get; set; }
        public event Action OnWorkComplete, OnStop, OnWorkFlowEnd;
        public event Action<string> OnUrlError;
        /// <summary>
        /// 下载速度，当前指针，总数
        /// </summary>
        public Action<int, int, int> OnStateMonitor;
        protected XmlDocument ConfigurationDoc;
        protected string ConfigPath;
        protected abstract DataRow[] Deal(string htmlPage);
        protected Queue<WorkFlow> WorkFlows;
        protected WorkFlow PreWorkFlow;
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
            WorkFlows = new Queue<WorkFlow>();
            Suspendsign = new ManualResetEvent(false);
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
                    OnWorkFlowEnd?.Invoke();
                    StartWorkFolow();
                }
                else
                {
                    OnWorkComplete?.Invoke();
                }
            }
            else
            {
                OnStop?.Invoke();
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
            Suspendsign.Set();
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
                var wf = new WorkFlow() { Enable = bool.Parse(x.Attributes["enable"].Value), Sn = int.Parse(x.Attributes["sn"].Value) };
                var xn = x.GetElementsByTagName("URL")[0];
                wf.UrlColumn = xn.Attributes["column"].Value;
                wf.FromSql = xn.Attributes["fromsql"].Value;
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
                    OnWorkComplete?.Invoke();
                    return;
                }
            }
            Url = Database.ExuSqlDataTable(PreWorkFlow.FromSql);
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
                OnStateMonitor?.Invoke(NetSpeed, urlhand, urlcount);
            }
        }

        //protected void MainThread()
        //{
        //    Regex reg = new Regex("charset=(.*)");
        //    Encoding encoding;
        //    DataTable dt;
        //    string encodestr, url = "";
        //    try
        //    {
        //        while (Work)
        //        {
        //            if (Suspending)
        //            {
        //                suspendsign.WaitOne();
        //            }
        //            lock (urlLocker)
        //            {
        //                if (Urls.Count>0)
        //                {
        //                    url = Urls.Dequeue();
        //                }
        //                else
        //                {
        //                    url = null;
        //                }
        //            }
        //            if (url == null)
        //            {
        //                break;
        //            }
        //            HttpWebRequest request = null;
        //            HttpWebResponse response = null;
        //            int times = 0;
        //            string page="";
        //            Thread td = new Thread(() =>
        //            {
        //                try
        //                {
        //                    request = (HttpWebRequest)WebRequest.Create(url);
        //                    request.Method = "GET";
        //                    response = (HttpWebResponse)request.GetResponse();
        //                }
        //                catch (Exception)
        //                {
                            
        //                }
        //            });
        //            td.IsBackground = true;
        //            td.Start();
        //            times += 1;
        //            Thread.Sleep(Interval);
        //            while (response==null)
        //            {
        //                Thread td0 = new Thread(() =>
        //                {
        //                    try
        //                    {
        //                        request.Abort();
        //                        request = (HttpWebRequest)WebRequest.Create(url);
        //                        request.Method = "GET";
        //                        response = (HttpWebResponse)request.GetResponse();
        //                    }
        //                    catch (Exception)
        //                    {
                               
        //                    }
        //                });
        //                td0.IsBackground = true;
        //                td0.Start();
        //                times += 1;
        //                Thread.Sleep(Interval);
        //                if (times == 5)
        //                {
        //                    throw new Exception("无法连接");
        //                }
        //            }
        //            Stream responsestream = response.GetResponseStream();
        //            Match m = reg.Match(response.Headers[HttpResponseHeader.ContentType]);
        //            encodestr = m.Groups[1].Value;
        //            if (encodestr != null)
        //            {
        //                if (encodestr == "")
        //                {
        //                    encoding = Encoding.Default;
        //                }
        //                else
        //                {
        //                    encoding = Encoding.GetEncoding(encodestr.Trim());
        //                }
        //            }
        //            else
        //            {
        //                encoding = Encoding.Default;
        //            }
        //            StreamReader sr = new StreamReader(responsestream, encoding);
        //            page = sr.ReadToEnd();
        //            sr.Close();
        //            dt = Deal(page);
        //            lock (saveLocker)
        //            {
        //                Database.Save(dt, PreWorkFlow.ValTable, PreWorkFlow.ValColumns);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write(ex.Message + "ERRORURL:" + url);
        //        if (OnErrorExist != null)
        //            OnErrorExist(ex.Message + "ERRORURL:" + url);
        //    }
        //    finally
        //    {
        //        LeaveThread += 1;   //线程标记回收
        //    }
        //}

        protected void MainThread()
        {
            var ar = new ARequest();
            var url = "";
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
                    Suspendsign.WaitOne();
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

    internal class UrlCreate
    {
        public static string ParameterEncode(string str)
        {
            var seed = new StringBuilder();
            var results = Encoding.UTF8.GetBytes(str);
            for (var i = 0; i < results.Length; ++i)
                seed.Append(@"%" + Convert.ToString(results[i], 16));
            return seed.ToString();
        }
        public void HashtableSerialize(Hashtable ht,string filepath)
        {
            var fs = new FileStream(filepath, FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, ht);
            fs.Close();
        }
        public Hashtable HashtableDeserialize(string filepath)
        {
            var fs = new FileStream(filepath, FileMode.OpenOrCreate);
            var bf = new BinaryFormatter();
            var ht = (Hashtable)bf.Deserialize(fs);
            fs.Close();
            return ht;
        }
    }
    /// <summary>
    /// 线程安全
    /// </summary>
    internal class HtmlPageCollect
    {
        private bool _working;
        private bool _waiting = false;//抓取线程是否处于等待
        private Queue<string> _urlSource=new Queue<string>();
        private readonly Queue<string> _resultHtml = new Queue<string>();
        private readonly object _sourcelocker = new object();
        private readonly object _resultlocker = new object();
        private readonly ManualResetEvent _grabsign = new ManualResetEvent(false);
        public event Action OnEnd;
        public void Start()
        {
            if (_working)
                return;
            Task.Run(()=>GrabThread());
        }
        public void End()
        {
            _working = false;
            if (_waiting)
                _grabsign.Set();
        }
        public void GrabThread()
        {
            try {
                while (_working)
                {
                    string url;
                    lock (_sourcelocker) {
                        url = _urlSource.Count > 0 ? _urlSource.Dequeue() : null;
                    }
                    if (url == null)
                    {
                        _waiting = true;
                        _grabsign.WaitOne();
                        continue;
                    }
                    _waiting = false;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.Timeout = 3000;
                    var response = (HttpWebResponse)request.GetResponse();
                    var contenttype = response.Headers[HttpResponseHeader.ContentType];
                    var encodestr = RegexpOperation.GroupSingleRegex(contenttype, "charset=(.*)");
                    var encoding = encodestr != null ? Encoding.GetEncoding(encodestr.Trim()) : Encoding.Default;
                    string htmldata;
                    using (var sr = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        htmldata = sr.ReadToEnd();
                        sr.Close();
                    }
                    response.Close();
                    lock (_resultlocker)
                    {
                        _resultHtml.Enqueue(htmldata);
                    }
                }
            }
            finally {
                OnEnd?.Invoke();
            }
        }
        public string PopHtml()
        {
            string result = null;
            lock(_resultlocker)
            {
                if(_resultHtml.Count>0)
                {
                    result = _resultHtml.Dequeue();
                }
            }
            return result;
        }
        public void PushUrl(string val)
        {
            lock (_sourcelocker)
            {
                _urlSource.Enqueue(val);
            }
            if (_waiting)
                _grabsign.Set();
        }
        public string[] Collection
        {
            get
            {
                lock(_resultlocker)
                {
                    var vals = _resultHtml.ToArray();
                    return vals;
                }
            }
        }
        public string[] Source
        {
            get
            {
                lock(_sourcelocker)
                {
                    var vals = _urlSource.ToArray();
                    return vals;
                }
            }
            set
            {
                lock(_sourcelocker)
                {
                    _urlSource = new Queue<string>(value);
                }
            }
        }
    }
    //abstract class Reptile
    //{

    //    protected SqlWorkUnit Database;
    //    protected bool downloadWork, dealWork;
    //    protected object urlLocker, pageLocker, saveLocker;
    //    protected Queue<string> Urls, Pages;
    //    protected int downloadthreadCount, dealthreadCount, downloadleaveThread, dealleaveThread, Interval;
    //    protected event Action OnDownloadEnd, OnDealEnd, errorExist;
    //    //工作流集合
    //    protected Queue<WorkFlow> WorkFlows;
    //    protected int DownloadleaveThread
    //    {
    //        get
    //        {

    //            return downloadleaveThread;
    //        }
    //        set
    //        {
    //            downloadleaveThread = value;
    //            if (value == downloadthreadCount && OnDownloadEnd != null)
    //                OnDownloadEnd();
    //        }
    //    }

    //    protected int DealleaveThread
    //    {
    //        get
    //        {
    //            return dealleaveThread;
    //        }
    //        set
    //        {
    //            dealleaveThread = value;
    //            if (value == dealthreadCount && OnDealEnd != null)
    //                OnDealEnd();
    //        }
    //    }

    //    protected TextLog log;
    //    public virtual void Start()
    //    {

    //    }
    //    public abstract void End();
    //    public abstract void Suspend();
    //    public abstract void Resume();

    //    protected virtual void ConfigIni(XmlDocument xd)
    //    {
    //        XmlElement root = xd.DocumentElement;
    //        Database = new SqlWorkUnit(root["database"].Value, @".\SQLEXPRESS");
    //        XmlNodeList xnl = root.GetElementsByTagName("workflow");
    //        foreach (XmlElement x in xnl)
    //        {
    //            if (x["enable"].Value != "TRUE")
    //            {
    //                continue;
    //            }
    //            WorkFlow wf = new WorkFlow() { Enable = true };
    //            XmlNode xn = x.GetElementsByTagName("URL")[0];
    //            wf.UrlTable = xn["table"].Value;
    //            wf.UrlColumn = xn["column"].Value;
    //            wf.UrlStart = int.Parse(xn["start"].Value);
    //            wf.ValTable = x.GetElementsByTagName("Value")[0].InnerText;
    //        }
    //    }



    //    public Reptile()
    //    {
    //        Urls = new Queue<string>();
    //        urlLocker = new object();
    //        pageLocker = new object();
    //        saveLocker = new object();
    //        Pages = new Queue<string>();
    //        WorkFlows = new Queue<WorkFlow>();
    //    }

    //    protected void PageDownloadStart(int max)
    //    {
    //        if (downloadWork)
    //            return;
    //        downloadWork = true;
    //        downloadthreadCount = max;
    //        downloadleaveThread = 0;
    //        for (int i = 0; i < max; ++i)
    //        {
    //            Thread td = new Thread(PageDownload);
    //            td.IsBackground = true;
    //            td.Start();
    //        }
    //    }

    //    protected void PageDownload()
    //    {
    //        try
    //        {
    //            Regex reg = new Regex("charset=(.*)");
    //            Encoding encoding;
    //            string encodestr, url = "";
    //            while (downloadWork)
    //            {
    //                lock (urlLocker)
    //                {
    //                    if (Urls.Count > 0)
    //                    {
    //                        url = Urls.Dequeue();
    //                    }
    //                    else
    //                    {
    //                        url = null;
    //                    }
    //                }
    //                if (url == null)
    //                {
    //                    Thread.Sleep(Interval);
    //                    continue;
    //                }
    //                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //                request.Method = "GET";
    //                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //                Match m = reg.Match(response.Headers[HttpResponseHeader.ContentType]);
    //                encodestr = m.Groups[1].Value;
    //                if (encodestr != null)
    //                {
    //                    encoding = Encoding.GetEncoding(encodestr.Trim());
    //                }
    //                else
    //                {
    //                    encoding = Encoding.Default;
    //                }
    //                using (StreamReader sr = new StreamReader(response.GetResponseStream(), encoding))
    //                {
    //                    string page = sr.ReadToEnd();
    //                    lock (pageLocker)
    //                    {
    //                        Pages.Enqueue(page);
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Write(ex.Message);
    //            if (errorExist != null)
    //                errorExist();
    //        }
    //        finally
    //        {
    //            DownloadleaveThread += 1;   //线程回收
    //        }
    //    }

    //    protected void PageDownloadEnd()
    //    {
    //        downloadWork = false;
    //    }

    //    protected void StartDeal(int max)
    //    {
    //        if (dealWork)
    //            return;
    //        dealWork = true;
    //        DealleaveThread = 0;
    //        dealthreadCount = max;
    //        for (int i = 0; i < max; ++i)
    //        {
    //            Thread dt = new Thread(DealThread);
    //            dt.IsBackground = true;
    //            dt.Start();
    //        }
    //    }

    //    protected void EndDeal()
    //    {
    //        dealWork = false;
    //    }

    //    protected void DealThread()
    //    {
    //        try
    //        {
    //            string page;
    //            DataTable dt;
    //            while (dealWork)
    //            {
    //                lock (pageLocker)
    //                {
    //                    page = Pages.Count > 0 ? Pages.Dequeue() : null;
    //                }
    //                if (page == null)
    //                {
    //                    Thread.Sleep(Interval);
    //                }
    //                dt = Deal(page);
    //                lock (saveLocker)
    //                {
    //                    Database.Update(dt);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Write(ex.Message);
    //        }
    //        finally
    //        {
    //            DealleaveThread += 1;
    //        }
    //    }

    //    protected abstract DataTable Deal(string htmlPage);
    //}
    

}
