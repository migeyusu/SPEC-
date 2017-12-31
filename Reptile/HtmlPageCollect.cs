using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reptile
{
    /// <summary>
    /// 线程安全
    /// </summary>
    internal class HtmlPageCollect
    {
        private bool _working;
        private bool _waiting; //抓取线程是否处于等待
        private Queue<string> _urlSource = new Queue<string>();
        private readonly Queue<string> _resultHtml = new Queue<string>();
        private readonly object _sourcelocker = new object();
        private readonly object _resultlocker = new object();
        private readonly ManualResetEvent _grabsign = new ManualResetEvent(false);
        public event Action OnEnd;

        public void Start()
        {
            if (_working)
                return;
            Task.Run(() => GrabThread());
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
                while (_working) {
                    string url;
                    lock (_sourcelocker) {
                        url = _urlSource.Count > 0 ? _urlSource.Dequeue() : null;
                    }
                    if (url == null) {
                        _waiting = true;
                        _grabsign.WaitOne();
                        continue;
                    }
                    _waiting = false;
                    var request = (HttpWebRequest) WebRequest.Create(url);
                    request.Method = "GET";
                    request.Timeout = 3000;
                    var response = (HttpWebResponse) request.GetResponse();
                    var contenttype = response.Headers[HttpResponseHeader.ContentType];
                    var encodestr = RegexpOperation.GroupSingleRegex(contenttype, "charset=(.*)");
                    var encoding = encodestr != null ? Encoding.GetEncoding(encodestr.Trim()) : Encoding.Default;
                    string htmldata;
                    using (var sr = new StreamReader(response.GetResponseStream(), encoding)) {
                        htmldata = sr.ReadToEnd();
                        sr.Close();
                    }
                    response.Close();
                    lock (_resultlocker) {
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
            lock (_resultlocker) {
                if (_resultHtml.Count > 0) {
                    result = _resultHtml.Dequeue();
                }
            }
            return result;
        }

        public void PushUrl(string val)
        {
            lock (_sourcelocker) {
                _urlSource.Enqueue(val);
            }
            if (_waiting)
                _grabsign.Set();
        }

        public string[] Collection {
            get {
                lock (_resultlocker) {
                    var vals = _resultHtml.ToArray();
                    return vals;
                }
            }
        }

        public string[] Source {
            get {
                lock (_sourcelocker) {
                    var vals = _urlSource.ToArray();
                    return vals;
                }
            }
            set {
                lock (_sourcelocker) {
                    _urlSource = new Queue<string>(value);
                }
            }
        }
    }
}