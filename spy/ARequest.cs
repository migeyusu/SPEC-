using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SepcReptile
{
    public class ARequest
    {
        private readonly Dictionary<string, IPAddress> _dnsCache;
        private const int IpPort = 80;
        private readonly int _buffersize = 16384;
        public bool Read;
        public int WaitTime { get; set; }

        public ARequest()
        {
            WaitTime = 3400;
            Read = true;
            _dnsCache = new Dictionary<string, IPAddress>();
        }

        public int ReceiveLength { get; set; }

        private string Request(string url, string host)
        {
            if (url.Contains("http://"))
                url = url.Substring(7, url.Length - 7);
            var head = new StringBuilder();
            head.Append("GET ");
            var position = url.IndexOf('/');
            head.Append(url.Substring(position, url.Length - position));
            head.Append(" HTTP/1.1");
            head.Append("\r\n");
            head.Append("Host:");
            head.Append(host);
            head.Append("\r\n");
            head.Append("\r\n");
            return head.ToString();
        }

        //同步方法
        public string GetHtml(string url)
        {
            if (url.Contains("https://")) {
                url = url.Substring(8, url.Length - 8);
            }
            else if (url.Contains("http://")) {
                url = url.Substring(7, url.Length - 7);
            }
            else {
                throw new ArgumentException("未识别的URL");
            }
            string page = null;
            var ms = new MemoryStream();
            var position = url.IndexOf('/') + 1;
            var tc = new TcpClient();
            var host = url.Substring(0, position - 1);
            var acomplished = false;
            NetworkStream ns = null;
            var td = new Thread(() => {
                try {
                    var ipa = _dnsCache.Keys.Contains(host) ? _dnsCache[host] : Dns.GetHostAddresses(host)[0];
                    tc.Connect(new IPEndPoint(ipa, IpPort));
                    ns = tc.GetStream();
                    var buffer = new byte[_buffersize];
                    var head = Encoding.UTF8.GetBytes(Request(url, host));
                    ns.Write(head, 0, head.Length);
                    Read = true;
                    while (Read) {
                        position = ns.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, position);
                        if (Encoding.UTF8.GetString(ms.ToArray()).Contains("</html>")) {
                            Read = false;
                        }
                    }
                    page = Encoding.UTF8.GetString(ms.ToArray());
                    ReceiveLength = ms.ToArray().Length;
                    ns.Close();
                    tc.Close();
                    if (!page.Contains("</html>")) {
                        page = null;
                    }
                    acomplished = true;
                }
                catch (Exception) {
                    Read = false;
                    acomplished = true;
                    page = null;
                }

            });
            td.IsBackground = true;
            td.Start();
            var i = 0;
            while (i < 3000) {
                Thread.Sleep(300);
                i += 300;
                if (acomplished) {
                    break;
                }
            }
            if (Read) {
                ns?.Close();
                tc.Close();
            }
            return page;
        }

    }
}
