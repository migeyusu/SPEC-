using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reptile.Spec
{
    public partial class MonitorForm : Form
    {
        public MonitorForm()
        {
        
            InitializeComponent();
            timer1.Enabled = false;
        }

        private SqlWorkUnit _database;

        private void Form1_Load(object sender, EventArgs e)
        {
            var tl = new TextLog(Application.StartupPath+"direct.txt");
          
            _sr.WorkFlowCompleted += sr_OnWorkEnd;
            _sr.OnUrlError += (s) =>
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    listBox1.Items.Add(s);
                    tl.Write(s);
                }));
            };
            _sr.WorkFlowCompleted += sr_OnWorkComplete;
            _sr.ProcessStopped += sr_OnStop;
            _sr.SpeedReported = (a, b, c) =>
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    label6.Text = a.ToString();
                    label8.Text = b + "/" + c;
                }));
            };
            _database = new SqlWorkUnit(@"D:\spy\spy\bin\Release\GPUBenchmark.mdf", @".\SQLEXPRESS");
        }

        private void sr_OnStop()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                label10.Text = "已暂停";
            }));
        }

        private void sr_OnWorkComplete()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                label10.Text = "已完成";
            }));
        }

        private void List()
        {
            var map = new List<string>();
            map.Add("system");
            map.Add("scoretype");
            map.Add("result");
            map.Add("baseline");
            map.Add("cores");
            map.Add("published");
            map.Add("csv");
            map.Add("html");
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("id"));
            dt.Columns.Add(new DataColumn("system"));
            dt.Columns.Add(new DataColumn("scoretype"));
            dt.Columns.Add(new DataColumn("result"));
            dt.Columns.Add(new DataColumn("baseline"));
            dt.Columns.Add(new DataColumn("cores"));
            dt.Columns.Add(new DataColumn("published"));
            dt.Columns.Add(new DataColumn("csv"));
            dt.Columns.Add(new DataColumn("html"));
            var s = new StreamReader(Application.StartupPath+"\\CPU2006 Results3 -- Results.html");
            var page = s.ReadToEnd();
            s.Close();
            var r0 = new Regex(@"CINT2006</h3>[\s\S]+?</table>");
            var r1 = new Regex(@"CFP2006</h3>[\s\S]+?</table>");
            var r2 = new Regex(@"CINT2006 Rates</h3>[\s\S]+?</table>");
            var r3 = new Regex(@"CFP2006 Rates</h3>[\s\S]+?</table>");
            Int(dt, r0.Match(page).Value);
            IntRates(dt, r2.Match(page).Value);
            Fp(dt, r1.Match(page).Value);
            FpRates(dt, r3.Match(page).Value);
            _database.Save(dt, "speclist", map);
            MessageBox.Show("ok");
        }

        private DataTable TableIni(string type)
        {
            var dt = new DataTable();
            dt.Columns.Add("peak");
            dt.Columns.Add("base");
            dt.Columns.Add("testdate");
            dt.Columns.Add("cpu");
            dt.Columns.Add("cpus");
            dt.Columns.Add("frequency");
            dt.Columns.Add("memory");
            dt.Columns.Add("feature");
            dt.Columns.Add("os");
            dt.Columns.Add("osversion");
            dt.Columns.Add("osstate");
            dt.Columns.Add("compiler");
            if (type == "int" || type == "intrates")
            {
                dt.Columns.Add("perlbenchbases");
                dt.Columns.Add("perlbenchbaser");
                dt.Columns.Add("perlbenchpeaks");
                dt.Columns.Add("perlbenchpeakr");
                dt.Columns.Add("bzip2bases");
                dt.Columns.Add("bzip2baser");
                dt.Columns.Add("bzip2peaks");
                dt.Columns.Add("bzip2peakr");
                dt.Columns.Add("gccbases");
                dt.Columns.Add("gccbaser");
                dt.Columns.Add("gccpeaks");
                dt.Columns.Add("gccpeakr");
                dt.Columns.Add("mcfbases");
                dt.Columns.Add("mcfbaser");
                dt.Columns.Add("mcfpeaks");
                dt.Columns.Add("mcfpeakr");
                dt.Columns.Add("gobmkbases");
                dt.Columns.Add("gobmkbaser");
                dt.Columns.Add("gobmkpeaks");
                dt.Columns.Add("gobmkpeakr");
                dt.Columns.Add("hmmerbases");
                dt.Columns.Add("hmmerbaser");
                dt.Columns.Add("hmmerpeaks");
                dt.Columns.Add("hmmerpeakr");
                dt.Columns.Add("sjengbases");
                dt.Columns.Add("sjengbaser");
                dt.Columns.Add("sjengpeaks");
                dt.Columns.Add("sjengpeakr");
                dt.Columns.Add("libquantumbases");
                dt.Columns.Add("libquantumbaser");
                dt.Columns.Add("libquantumpeaks");
                dt.Columns.Add("libquantumpeakr");
                dt.Columns.Add("h264refbases");
                dt.Columns.Add("h264refbaser");
                dt.Columns.Add("h264refpeaks");
                dt.Columns.Add("h264refpeakr");
                dt.Columns.Add("omnetppbases");
                dt.Columns.Add("omnetppbaser");
                dt.Columns.Add("omnetpppeaks");
                dt.Columns.Add("omnetpppeakr");
                dt.Columns.Add("astarbases");
                dt.Columns.Add("astarbaser");
                dt.Columns.Add("astarpeaks");
                dt.Columns.Add("astarpeakr");
                dt.Columns.Add("xalancbmkbases");
                dt.Columns.Add("xalancbmkbaser");
                dt.Columns.Add("xalancbmkpeaks");
                dt.Columns.Add("xalancbmkpeakr");
            }
            else if (type == "fp" || type == "fprates")
            {
                dt.Columns.Add("bwavesbases");
                dt.Columns.Add("bwavesbaser");
                dt.Columns.Add("bwavespeaks");
                dt.Columns.Add("bwavespeakr");

                dt.Columns.Add("gamessbases");
                dt.Columns.Add("gamessbaser");
                dt.Columns.Add("gamesspeaks");
                dt.Columns.Add("gamesspeakr");

                dt.Columns.Add("milcbases");
                dt.Columns.Add("milcbaser");
                dt.Columns.Add("milcpeaks");
                dt.Columns.Add("milcpeakr");

                dt.Columns.Add("zeusmpbases");
                dt.Columns.Add("zeusmpbaser");
                dt.Columns.Add("zeusmppeaks");
                dt.Columns.Add("zeusmppeakr");

                dt.Columns.Add("gromacsbases");
                dt.Columns.Add("gromacsbaser");
                dt.Columns.Add("gromacspeaks");
                dt.Columns.Add("gromacspeakr");

                dt.Columns.Add("cactusADMbases");
                dt.Columns.Add("cactusADMbaser");
                dt.Columns.Add("cactusADMpeaks");
                dt.Columns.Add("cactusADMpeakr");

                dt.Columns.Add("leslie3dbases");
                dt.Columns.Add("leslie3dbaser");
                dt.Columns.Add("leslie3dpeaks");
                dt.Columns.Add("leslie3dpeakr");


                dt.Columns.Add("namdbases");
                dt.Columns.Add("namdbaser");
                dt.Columns.Add("namdpeaks");
                dt.Columns.Add("namdpeakr");

                dt.Columns.Add("dealIIbases");
                dt.Columns.Add("dealIIbaser");
                dt.Columns.Add("dealIIpeaks");
                dt.Columns.Add("dealIIpeakr");

                dt.Columns.Add("soplexbases");
                dt.Columns.Add("soplexbaser");
                dt.Columns.Add("soplexpeaks");
                dt.Columns.Add("soplexpeakr");

                dt.Columns.Add("povraybases");
                dt.Columns.Add("povraybaser");
                dt.Columns.Add("povraypeaks");
                dt.Columns.Add("povraypeakr");

                dt.Columns.Add("calculixbases");
                dt.Columns.Add("calculixbaser");
                dt.Columns.Add("calculixpeaks");
                dt.Columns.Add("calculixpeakr");

                dt.Columns.Add("GemsFDTDbases");
                dt.Columns.Add("GemsFDTDbaser");
                dt.Columns.Add("GemsFDTDpeaks");
                dt.Columns.Add("GemsFDTDpeakr");

                dt.Columns.Add("tontobases");
                dt.Columns.Add("tontobaser");
                dt.Columns.Add("tontopeaks");
                dt.Columns.Add("tontopeakr");

                dt.Columns.Add("lbmbases");
                dt.Columns.Add("lbmbaser");
                dt.Columns.Add("lbmpeaks");
                dt.Columns.Add("lbmpeakr");

                dt.Columns.Add("wrfbases");
                dt.Columns.Add("wrfbaser");
                dt.Columns.Add("wrfpeaks");
                dt.Columns.Add("wrfpeakr");

                dt.Columns.Add("sphinx3bases");
                dt.Columns.Add("sphinx3baser");
                dt.Columns.Add("sphinx3peaks");
                dt.Columns.Add("sphinx3peakr");
            }

            return dt;
        }

        private readonly SpecReptile _sr = new SpecReptile(Application.StartupPath + "\\config.xml");

        private void button1_Click(object sender, EventArgs e)
        {
            _sr.Start(26);
            timer1.Enabled = true;
 
            label10.Text = "已开始";
            //DataTable dt = TableIni("fp");
            //StreamReader sr = new StreamReader(Application.StartupPath + "\\sccd.html");
            //string page = sr.ReadToEnd();
            //sr.Close(); 
            //DataRow dr=dt.NewRow();
            //HeadDetail(dr, page);
            //Regex r10 = new Regex(@"<span class=""selected"">(.*?)</span>");
            //MatchCollection mc = r10.Matches(page);
            //List<float> vals = new List<float>();
            //foreach (Match m in mc)
            //    vals.Add(float.Parse(m.Groups[1].Value));
            //FPDetail(dr, vals);
            //dt.Rows.Add(dr);
            //MessageBox.Show(dt.ToString());
            //MessageBox.Show(hs.GetHtml("http://www.baidu.com"));
            //DatabaseOps dbo = new DatabaseOps(Application.StartupPath + @"\GPUBenchmark.mdf");
            //dbo.OpenTable("select * from BaseInfo");
          //  string str1 = @"<TD><A HREF="".*"">(.*)</A></TD><TD>";
         //   string str1=@"Description:</span>&nbsp;&nbsp;<em>(.*)</em><br>";
         //   //StreamReader sr=new StreamReader(Application.StartupPath+@"\test1.txt");
         //   //string str2=sr.ReadToEnd();
         //   string str2=hs.GetHtml("http://www.videocardbenchmark.net/gpu.php?gpu=FireGL%20V3300");
         //  // sr.Close();
         ////   string[] ary=htmlanalyzer.multiMatch(str1, str2,true);
         //   string a1 = htmlanalyzer.singlMatch(str1, str2, true);
         //   MessageBox.Show(a1.Length.ToString());

        }


        private void sr_OnWorkEnd()
        {
            this.Invoke(new MethodInvoker(() => { label10.Text = "已完成"; }));
        }

        private void HeadDetail(DataRow dr,string page)
        {
            var r0 = new Regex(@"<span class=""value"">(.*?)</span>");
            var r1 = new Regex(@"test_date_val"">(.*?)</td>");
            var r2 = new Regex(@"CPU Name</a>:</th>\n\s+?<td>(.*?)</td>");
            var r3 = new Regex(@"orderable</a>:</th>\n\s*?<td>(.*?)</td>");
            var r4 = new Regex(@"CPU MHz</a>:</th>\n\s+?<td>(.*?)</td>");
            var r5 = new Regex(@"Memory</a>:</th>\n\s+?<td>(.*?)</td>");
            var r6 = new Regex(@"Characteristics</a>:</th>\n\s+?<td>(.*?)</td>");
            var r7 = new Regex(@"Operating System</a>:</th>\n\s+?<td>(.*?)<br>\n(.*?)<br>");
            var r8 = new Regex(@"System State</a>:</th>\n\s+?<td>(.*?)</td>");
            var r9 = new Regex(@"Compiler</a>:</th>\n\s+?<td>([\s\S]*?)</td>");
            var mc = r0.Matches(page);
            dr["peak"] = mc[0].Groups[1].Value;
            dr["base"] = mc[0].Groups[1].Value;
            dr["testdate"] = r1.Match(page).Groups[1].Value;
            dr["cpu"] = r2.Match(page).Groups[1].Value;
            dr["cpus"] = r3.Match(page).Groups[1].Value;
            dr["frequency"] = r4.Match(page).Groups[1].Value;
            dr["memory"] = r5.Match(page).Groups[1].Value;
            dr["feature"] = r6.Match(page).Groups[1].Value;
            var m=r7.Match(page);
            dr["os"] = m.Groups[1].Value;
            dr["osversion"] = m.Groups[2].Value;
            dr["osstate"] = r8.Match(page).Groups[1].Value;
            dr["compiler"] = r9.Match(page).Groups[1].Value;
        }

        private void IntDetail(DataRow dt,List<float> f)
        {
            dt["perlbenchbases"] = f[0];
            dt["perlbenchbaser"] = f[1];
            dt["perlbenchpeaks"] = f[2];
            dt["perlbenchpeakr"] = f[3];
            dt["bzip2bases"] = f[4];
            dt["bzip2baser"] = f[5];
            dt["bzip2peaks"] = f[6];
            dt["bzip2peakr"] = f[7];
            dt["gccbases"] = f[8];
            dt["gccbaser"] = f[9];
            dt["gccpeaks"] = f[10];
            dt["gccpeakr"] = f[11];
            dt["mcfbases"] = f[12];
            dt["mcfbaser"] = f[13];
            dt["mcfpeaks"] = f[14];
            dt["mcfpeakr"] = f[15];
            dt["gobmkbases"] = f[16];
            dt["gobmkbaser"] = f[17];
            dt["gobmkpeaks"] = f[18];
            dt["gobmkpeakr"] = f[19];
            dt["hmmerbases"] = f[20];
            dt["hmmerbaser"] = f[21];
            dt["hmmerpeaks"] = f[22];
            dt["hmmerpeakr"] = f[23];
            dt["sjengbases"] = f[24];
            dt["sjengbaser"] = f[25];
            dt["sjengpeaks"] = f[26];
            dt["sjengpeakr"] = f[27];
            dt["libquantumbases"] = f[28];
            dt["libquantumbaser"] = f[29];
            dt["libquantumpeaks"] = f[30];
            dt["libquantumpeakr"] = f[31];
            dt["h264refbases"] = f[32];
            dt["h264refbaser"] = f[33];
            dt["h264refpeaks"] = f[34];
            dt["h264refpeakr"] = f[35];
            dt["omnetppbases"] = f[36];
            dt["omnetppbaser"] = f[37];
            dt["omnetpppeaks"] = f[38];
            dt["omnetpppeakr"] = f[39];
            dt["astarbases"] = f[40];
            dt["astarbaser"] = f[41];
            dt["astarpeaks"] = f[42];
            dt["astarpeakr"] = f[43];
            dt["xalancbmkbases"] = f[44];
            dt["xalancbmkbaser"] = f[45];
            dt["xalancbmkpeaks"] = f[46];
            dt["xalancbmkpeakr"] = f[47];
        }

        private void FpDetail(DataRow dt, List<float> f)
        {
            dt["bwavesbases"] = f[0];
            dt["bwavesbaser"] = f[1];
            dt["bwavespeaks"] = f[2];
            dt["bwavespeakr"] = f[3];

            dt["gamessbases"]=f[4];
            dt["gamessbaser"]=f[5];
            dt["gamesspeaks"]=f[6];
            dt["gamesspeakr"]=f[7];

            dt["milcbases"]=f[8];
            dt["milcbaser"]=f[9];
            dt["milcpeaks"]=f[10];
            dt["milcpeakr"]=f[11];

            dt["zeusmpbases"]=f[12];
            dt["zeusmpbaser"]=f[13];
            dt["zeusmppeaks"]=f[14];
            dt["zeusmppeakr"]=f[15];

            dt["gromacsbases"]=f[16];
            dt["gromacsbaser"]=f[17];
            dt["gromacspeaks"]=f[18];
            dt["gromacspeakr"]=f[19];

            dt["cactusADMbases"]=f[20];
            dt["cactusADMbaser"]=f[21];
            dt["cactusADMpeaks"]=f[22];
            dt["cactusADMpeakr"]=f[23];

            dt["leslie3dbases"]=f[24];
            dt["leslie3dbaser"]=f[25];
            dt["leslie3dpeaks"]=f[26];
            dt["leslie3dpeakr"]=f[27];


            dt["namdbases"]=f[28];
            dt["namdbaser"]=f[29];
            dt["namdpeaks"]=f[30];
            dt["namdpeakr"]=f[31];

            dt["dealIIbases"]=f[32];
            dt["dealIIbaser"]=f[33];
            dt["dealIIpeaks"]=f[34];
            dt["dealIIpeakr"]=f[35];

            dt["soplexbases"]=f[36];
            dt["soplexbaser"]=f[37];
            dt["soplexpeaks"]=f[38];
            dt["soplexpeakr"]=f[39];

            dt["povraybases"]=f[40];
            dt["povraybaser"]=f[41];
            dt["povraypeaks"]=f[42];
            dt["povraypeakr"]=f[43];

            dt["calculixbases"]=f[44];
            dt["calculixbaser"]=f[45];
            dt["calculixpeaks"]=f[46];
            dt["calculixpeakr"]=f[47];

            dt["GemsFDTDbases"]=f[48];
            dt["GemsFDTDbaser"]=f[49];
            dt["GemsFDTDpeaks"]=f[50];
            dt["GemsFDTDpeakr"]=f[51];


            dt["tontobases"]=f[52];
            dt["tontobaser"]=f[53];
            dt["tontopeaks"]=f[54];
            dt["tontopeakr"]=f[55];

            dt["lbmbases"]=f[56];
            dt["lbmbaser"]=f[57];
            dt["lbmpeaks"]=f[58];
            dt["lbmpeakr"]=f[59];

            dt["wrfbases"]=f[60];
            dt["wrfbaser"]=f[61];
            dt["wrfpeaks"]=f[62];
            dt["wrfpeakr"]=f[63];

            dt["sphinx3bases"]=f[64];
            dt["sphinx3baser"]=f[65];
            dt["sphinx3peaks"]=f[66];
            dt["sphinx3peakr"]=f[67];
           
        }

        private void Int(DataTable dt, string paragraph)
        {
            var r0 = new Regex(@"<tr>\n\s\s<td valign=""top"">[\s\S]+?</tr>");
            var r1 = new Regex(@"<td valign=""top"">(.*?)</td>");
            var r2 = new Regex(@"<a href=""(.*?)"">HTML");
            var r3 = new Regex(@"a> <a href=""(.*?)"">CSV");
            var test=0f;
            var testi=0;
            var m = r0.Matches(paragraph);
            foreach (Match x in m)
            {
                var m0 = r1.Matches(x.Value);
                var dr = dt.NewRow();
                dr["system"] = m0[1].Groups[1].Value;
                dr["scoretype"] = "int";
                dr["result"] = float.TryParse(m0[2].Groups[1].Value,out test) ? test : 0f;
                dr["baseline"] = float.TryParse(m0[3].Groups[1].Value, out test) ? test : 0f;
                dr["cores"] = int.TryParse(m0[4].Groups[1].Value, out testi) ? testi : 0;
                dr["published"] = m0[7].Groups[1].Value;
                dr["csv"] = r2.Match(x.Value).Groups[1].Value;
                dr["html"] = r3.Match(x.Value).Groups[1].Value;
                dt.Rows.Add(dr);
            }
        }

        private void Fp(DataTable dt, string paragraph)
        {
            var r0 = new Regex(@"<tr>\n\s\s<td valign=""top"">[\s\S]+?</tr>");
            var r1 = new Regex(@"<td valign=""top"">(.*?)</td>");
            var r2 = new Regex(@"<a href=""(.*?)"">HTML");
            var r3 = new Regex(@"a> <a href=""(.*?)"">CSV");
            var test = 0f;
            var testi = 0;
            var m = r0.Matches(paragraph);
            foreach (Match x in m)
            {
                var m0 = r1.Matches(x.Value);
                var dr = dt.NewRow();
                dr["system"] = m0[1].Groups[1].Value;
                dr["scoretype"] = "fp";
                dr["result"] = float.TryParse(m0[2].Groups[1].Value, out test) ? test : 0f;
                dr["baseline"] = float.TryParse(m0[3].Groups[1].Value, out test) ? test : 0f;
                dr["cores"] = int.TryParse(m0[4].Groups[1].Value, out testi) ? testi : 0;
                dr["published"] = m0[7].Groups[1].Value;
                dr["csv"] = r2.Match(x.Value).Groups[1].Value;
                dr["html"] = r3.Match(x.Value).Groups[1].Value;
                dt.Rows.Add(dr);
            }
        }

        private void IntRates(DataTable dt,string paragraph)
        {
            var r0 = new Regex(@"<tr>\n\s\s<td valign=""top"">[\s\S]+?</tr>");
            var r1 = new Regex(@"<td valign=""top"">(.*?)</td>");
            var r2 = new Regex(@"<a href=""(.*?)"">HTML");
            var r3 = new Regex(@"a> <a href=""(.*?)"">CSV");
            var m = r0.Matches(paragraph);
            var test = 0f;
            var testi = 0;
            foreach (Match x in m)
            {
                var m0 = r1.Matches(x.Value);
                var dr = dt.NewRow();
                dr["system"] = m0[1].Groups[1].Value;
                dr["scoretype"] = "intrates";
                dr["result"] = float.TryParse(m0[2].Groups[1].Value, out test) ? test : 0f;
                dr["baseline"] = float.TryParse(m0[3].Groups[1].Value, out test) ? test : 0f;
                dr["cores"] = int.TryParse(m0[4].Groups[1].Value, out testi) ? testi : 0;
                dr["published"] = m0[7].Groups[1].Value;
                dr["csv"] = r2.Match(x.Value).Groups[1].Value;
                dr["html"] = r3.Match(x.Value).Groups[1].Value;
                dt.Rows.Add(dr);
            }
        }

        private void FpRates(DataTable dt,string paragraph)
        {
            var r0 = new Regex(@"<tr>\n\s\s<td valign=""top"">[\s\S]+?</tr>");
            var r1 = new Regex(@"<td valign=""top"">(.*?)</td>");
            var r2 = new Regex(@"<a href=""(.*?)"">HTML");
            var r3 = new Regex(@"a> <a href=""(.*?)"">CSV");
            var test = 0f;
            var testi = 0;
            var m = r0.Matches(paragraph);
            foreach (Match x in m)
            {
                var m0 = r1.Matches(x.Value);
                var dr = dt.NewRow();
                dr["system"] = m0[1].Groups[1].Value;
                dr["scoretype"] = "fprates";
                dr["result"] = float.TryParse(m0[2].Groups[1].Value, out test) ? test : 0f;
                dr["baseline"] = float.TryParse(m0[3].Groups[1].Value, out test) ? test : 0f;
                dr["cores"] = int.TryParse(m0[4].Groups[1].Value, out testi) ? testi : 0;
                dr["published"] = m0[7].Groups[1].Value;
                dr["csv"] = r2.Match(x.Value).Groups[1].Value;
                dr["html"] = r3.Match(x.Value).Groups[1].Value;
                dt.Rows.Add(dr);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _sr.End();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.ba.com/k");
            //request.Method = "GET";
            //HttpWebResponse st = (HttpWebResponse)request.GetResponse();
            //Stream s =st.GetResponseStream();
            //StreamReader sr1 = new StreamReader(s);
            //MessageBox.Show(sr1.ReadToEnd());
            var sr = new StreamReader(Application.StartupPath + "\\a.txt",false);
            var page = sr.ReadToEnd();
            sr.Close();
            var r10 = new Regex(@"Operating System</a>:</th>[\s\S]*?<td>(.*?)<br />([\s\S]*?)<br /></td>");
            var mt = r10.Match(page);
            MessageBox.Show(mt.Groups[1].Value);


        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ar = new HttpRequest();

            var sw = new StreamWriter(Application.StartupPath + "\\a.txt");
            sw.Write(ar.GetHtml("http://www.spec.org/cpu2006/results/res2014q4/cpu2006-20141216-33623.html"));
            sw.Close();
            //HttpWebRequest r = (HttpWebRequest)WebRequest.Create("http://www.spec.org/cgi-bin/osgresults?conf=cpu2006");
            //HttpWebResponse w=(HttpWebResponse)r.GetResponse();
            //MessageBox.Show(w.ContentLength.ToString());
            MessageBox.Show("ok");

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = _sr.CurrentTdCount.ToString();
            label3.Text = _sr.LeaveThread.ToString();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            
        }

      
    }
}
