using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace spy
{
    public class ARequest
    {
        Dictionary<string, IPAddress> DNSCache;
        const int IPPort = 80;
        int buffersize = 16384;
        public bool read;
        public int WaitTime { get; set; }
        public ARequest()
        {
            WaitTime = 3400;
            read = true;
            DNSCache = new Dictionary<string, IPAddress>();
        }
        public int ReceiveLength { get; set; }
        string Request(string url, string host)
        {
            if (url.Contains("http://"))
                url = url.Substring(7, url.Length - 7);
            StringBuilder head = new StringBuilder();
            head.Append("GET ");
            int position = url.IndexOf('/');
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
            if (url.Contains("https://"))
            {
                url = url.Substring(8, url.Length - 8);
            }
            else if (url.Contains("http://"))
            {
                url = url.Substring(7, url.Length - 7);
            }
            else
            {
                throw new ArgumentException("未识别的URL");
            }
            string page = null;
            MemoryStream ms = new MemoryStream();
            int position = url.IndexOf('/') + 1;
            TcpClient tc = new TcpClient();
            string host = url.Substring(0, position - 1);
            bool acomplished = false;
            NetworkStream ns = null;
            Thread td = new Thread(() =>
            {
                try
                {
                    IPAddress ipa = DNSCache.Keys.Contains(host) ? DNSCache[host] : Dns.GetHostAddresses(host)[0];
                    tc.Connect(new IPEndPoint(ipa, IPPort));
                    ns = tc.GetStream();
                    byte[] buffer = new byte[buffersize];
                    byte[] head = Encoding.UTF8.GetBytes(Request(url, host));
                    ns.Write(head, 0, head.Length);
                    read = true;
                    while (read)
                    {
                        position = ns.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, position);
                        if (Encoding.UTF8.GetString(ms.ToArray()).Contains("</html>"))
                        {
                            read = false;
                        }
                    }
                    page = Encoding.UTF8.GetString(ms.ToArray());
                    ReceiveLength = ms.ToArray().Length;
                    ns.Close();
                    tc.Close();
                    if (!page.Contains("</html>"))
                    {
                        page = null;
                    }
                    acomplished = true;
                }
                catch (Exception)
                {
                    read = false;
                    acomplished = true;
                    page = null;
                }

            });
            td.IsBackground = true;
            td.Start();
            int i = 0;
            while (i < 3000)
            {
                Thread.Sleep(300);
                i += 300;
                if (acomplished)
                {
                    break;
                }
            }
            if (read)
            {
               if(ns!=null)
                   ns.Close();
                tc.Close();
            }
            return page;
        }

    }
}
