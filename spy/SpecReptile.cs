using System.Data;
using System.Text.RegularExpressions;

namespace SepcReptile
{
    public class SpecReptile : Reptile
    {
        object datarowLocker = new object();
        public SpecReptile(string config)
        {
            ConfigIni(config);
        }

        protected override void TableIni()
        {
            CommonStructureTable = new DataTable();
            CommonStructureTable.Columns.Add("id");
            CommonStructureTable.Columns.Add("peak");
            CommonStructureTable.Columns.Add("base");
            CommonStructureTable.Columns.Add("testdate");
            CommonStructureTable.Columns.Add("cpu");
            CommonStructureTable.Columns.Add("cpus");
            CommonStructureTable.Columns.Add("frequency");
            CommonStructureTable.Columns.Add("memory");
            CommonStructureTable.Columns.Add("feature");
            CommonStructureTable.Columns.Add("os");
            CommonStructureTable.Columns.Add("compiler");         
            CommonStructureTable.Columns.Add("osstate");
            if (PreWorkFlow.SN == 1 || PreWorkFlow.SN ==2)
            {
                #region int & intrates
                CommonStructureTable.Columns.Add("perlbenchbases");
                CommonStructureTable.Columns.Add("perlbenchbaser");
                CommonStructureTable.Columns.Add("perlbenchpeaks");
                CommonStructureTable.Columns.Add("perlbenchpeakr");
                CommonStructureTable.Columns.Add("bzip2bases");
                CommonStructureTable.Columns.Add("bzip2baser");
                CommonStructureTable.Columns.Add("bzip2peaks");
                CommonStructureTable.Columns.Add("bzip2peakr");
                CommonStructureTable.Columns.Add("gccbases");
                CommonStructureTable.Columns.Add("gccbaser");
                CommonStructureTable.Columns.Add("gccpeaks");
                CommonStructureTable.Columns.Add("gccpeakr");
                CommonStructureTable.Columns.Add("mcfbases");
                CommonStructureTable.Columns.Add("mcfbaser");
                CommonStructureTable.Columns.Add("mcfpeaks");
                CommonStructureTable.Columns.Add("mcfpeakr");
                CommonStructureTable.Columns.Add("gobmkbases");
                CommonStructureTable.Columns.Add("gobmkbaser");
                CommonStructureTable.Columns.Add("gobmkpeaks");
                CommonStructureTable.Columns.Add("gobmkpeakr");
                CommonStructureTable.Columns.Add("hmmerbases");
                CommonStructureTable.Columns.Add("hmmerbaser");
                CommonStructureTable.Columns.Add("hmmerpeaks");
                CommonStructureTable.Columns.Add("hmmerpeakr");
                CommonStructureTable.Columns.Add("sjengbases");
                CommonStructureTable.Columns.Add("sjengbaser");
                CommonStructureTable.Columns.Add("sjengpeaks");
                CommonStructureTable.Columns.Add("sjengpeakr");
                CommonStructureTable.Columns.Add("libquantumbases");
                CommonStructureTable.Columns.Add("libquantumbaser");
                CommonStructureTable.Columns.Add("libquantumpeaks");
                CommonStructureTable.Columns.Add("libquantumpeakr");
                CommonStructureTable.Columns.Add("h264refbases");
                CommonStructureTable.Columns.Add("h264refbaser");
                CommonStructureTable.Columns.Add("h264refpeaks");
                CommonStructureTable.Columns.Add("h264refpeakr");
                CommonStructureTable.Columns.Add("omnetppbases");
                CommonStructureTable.Columns.Add("omnetppbaser");
                CommonStructureTable.Columns.Add("omnetpppeaks");
                CommonStructureTable.Columns.Add("omnetpppeakr");
                CommonStructureTable.Columns.Add("astarbases");
                CommonStructureTable.Columns.Add("astarbaser");
                CommonStructureTable.Columns.Add("astarpeaks");
                CommonStructureTable.Columns.Add("astarpeakr");
                CommonStructureTable.Columns.Add("xalancbmkbases");
                CommonStructureTable.Columns.Add("xalancbmkbaser");
                CommonStructureTable.Columns.Add("xalancbmkpeaks");
                CommonStructureTable.Columns.Add("xalancbmkpeakr");
                #endregion
            }
            else if (PreWorkFlow.SN == 3 || PreWorkFlow.SN == 4)
            {
                #region fp & fprates
                CommonStructureTable.Columns.Add("bwavesbases");
                CommonStructureTable.Columns.Add("bwavesbaser");
                CommonStructureTable.Columns.Add("bwavespeaks");
                CommonStructureTable.Columns.Add("bwavespeakr");

                CommonStructureTable.Columns.Add("gamessbases");
                CommonStructureTable.Columns.Add("gamessbaser");
                CommonStructureTable.Columns.Add("gamesspeaks");
                CommonStructureTable.Columns.Add("gamesspeakr");

                CommonStructureTable.Columns.Add("milcbases");
                CommonStructureTable.Columns.Add("milcbaser");
                CommonStructureTable.Columns.Add("milcpeaks");
                CommonStructureTable.Columns.Add("milcpeakr");

                CommonStructureTable.Columns.Add("zeusmpbases");
                CommonStructureTable.Columns.Add("zeusmpbaser");
                CommonStructureTable.Columns.Add("zeusmppeaks");
                CommonStructureTable.Columns.Add("zeusmppeakr");

                CommonStructureTable.Columns.Add("gromacsbases");
                CommonStructureTable.Columns.Add("gromacsbaser");
                CommonStructureTable.Columns.Add("gromacspeaks");
                CommonStructureTable.Columns.Add("gromacspeakr");

                CommonStructureTable.Columns.Add("cactusADMbases");
                CommonStructureTable.Columns.Add("cactusADMbaser");
                CommonStructureTable.Columns.Add("cactusADMpeaks");
                CommonStructureTable.Columns.Add("cactusADMpeakr");

                CommonStructureTable.Columns.Add("leslie3dbases");
                CommonStructureTable.Columns.Add("leslie3dbaser");
                CommonStructureTable.Columns.Add("leslie3dpeaks");
                CommonStructureTable.Columns.Add("leslie3dpeakr");


                CommonStructureTable.Columns.Add("namdbases");
                CommonStructureTable.Columns.Add("namdbaser");
                CommonStructureTable.Columns.Add("namdpeaks");
                CommonStructureTable.Columns.Add("namdpeakr");

                CommonStructureTable.Columns.Add("dealIIbases");
                CommonStructureTable.Columns.Add("dealIIbaser");
                CommonStructureTable.Columns.Add("dealIIpeaks");
                CommonStructureTable.Columns.Add("dealIIpeakr");

                CommonStructureTable.Columns.Add("soplexbases");
                CommonStructureTable.Columns.Add("soplexbaser");
                CommonStructureTable.Columns.Add("soplexpeaks");
                CommonStructureTable.Columns.Add("soplexpeakr");

                CommonStructureTable.Columns.Add("povraybases");
                CommonStructureTable.Columns.Add("povraybaser");
                CommonStructureTable.Columns.Add("povraypeaks");
                CommonStructureTable.Columns.Add("povraypeakr");

                CommonStructureTable.Columns.Add("calculixbases");
                CommonStructureTable.Columns.Add("calculixbaser");
                CommonStructureTable.Columns.Add("calculixpeaks");
                CommonStructureTable.Columns.Add("calculixpeakr");

                CommonStructureTable.Columns.Add("GemsFDTDbases");
                CommonStructureTable.Columns.Add("GemsFDTDbaser");
                CommonStructureTable.Columns.Add("GemsFDTDpeaks");
                CommonStructureTable.Columns.Add("GemsFDTDpeakr");

                CommonStructureTable.Columns.Add("tontobases");
                CommonStructureTable.Columns.Add("tontobaser");
                CommonStructureTable.Columns.Add("tontopeaks");
                CommonStructureTable.Columns.Add("tontopeakr");

                CommonStructureTable.Columns.Add("lbmbases");
                CommonStructureTable.Columns.Add("lbmbaser");
                CommonStructureTable.Columns.Add("lbmpeaks");
                CommonStructureTable.Columns.Add("lbmpeakr");

                CommonStructureTable.Columns.Add("wrfbases");
                CommonStructureTable.Columns.Add("wrfbaser");
                CommonStructureTable.Columns.Add("wrfpeaks");
                CommonStructureTable.Columns.Add("wrfpeakr");

                CommonStructureTable.Columns.Add("sphinx3bases");
                CommonStructureTable.Columns.Add("sphinx3baser");
                CommonStructureTable.Columns.Add("sphinx3peaks");
                CommonStructureTable.Columns.Add("sphinx3peakr");
                #endregion
            }
        }

        protected override DataRow[] Deal(string htmlPage)
        {
            DataRow dr=null;
            lock (datarowLocker)
            {
                dr = CommonStructureTable.NewRow();
            }
            switch (PreWorkFlow.SN)
            {
                case 1://int
                    HeadDetail(dr, htmlPage);
                    IntDetail(dr, htmlPage);
                    break;
                case 2://intrates
                    HeadDetail(dr, htmlPage);
                    IntDetail(dr,htmlPage);
                    break;
                case 3://fp
                    HeadDetail(dr, htmlPage);
                    FPDetail(dr, htmlPage);
                    break;
                case 4://fprates
                    HeadDetail(dr, htmlPage);
                    FPDetail(dr, htmlPage);
                    break;
                default:
                    break;
            }
            return new DataRow[] { dr };
        }

        void HeadDetail(DataRow dr, string page)
        {
            Regex r0 = new Regex(@"<span class=""value"">(.*?)</span>");
            Regex r1 = new Regex(@"test_date_val"">(.*?)</td>");
            Regex r2 = new Regex(@"CPU Name</a>:</th>\n\s+?<td>(.*?)</td>");
            Regex r3 = new Regex(@"orderable</a>:</th>\n\s*?<td>(.*?)</td>");
            Regex r4 = new Regex(@"CPU MHz</a>:</th>\n\s+?<td>(.*?)</td>");
            Regex r5 = new Regex(@"Memory</a>:</th>\n\s+?<td>([\s\S]*?)</td>");
            Regex r6 = new Regex(@"Characteristics</a>:</th>\n\s+?<td>(.*?)</td>");
            Regex r7 = new Regex(@"Operating System</a>:</th>[\s\S]*?<td>([\s\S]*?)</td>");
            Regex r8 = new Regex(@"System State</a>:</th>\n\s+?<td>(.*?)</td>");
            Regex r9 = new Regex(@"Compiler</a>:</th>\n\s+?<td>([\s\S]*?)</td>");
            MatchCollection mc = r0.Matches(page);
            float test=0;
            int test1=0;
            dr["peak"] = float.TryParse(mc[0].Groups[1].Value,out test) ? test : 0;
            dr["base"] = float.TryParse(mc[1].Groups[1].Value, out test) ? test : 0;
            dr["testdate"] = r1.Match(page).Groups[1].Value;
            dr["cpu"] = r2.Match(page).Groups[1].Value;
            dr["cpus"] = r3.Match(page).Groups[1].Value;
            dr["frequency"] = int.TryParse(r4.Match(page).Groups[1].Value, out test1) ? test1 : 0;
            dr["memory"] = r5.Match(page).Groups[1].Value;
            dr["feature"] = r6.Match(page).Groups[1].Value;
            dr["os"] = r7.Match(page).Groups[1].Value;
            dr["osstate"] = r8.Match(page).Groups[1].Value;
            dr["compiler"] = r9.Match(page).Groups[1].Value;
        }

        Regex basesec = new Regex(@"basecol secs selected.*?selected"">(.*?)</span>");
        Regex baserat = new Regex(@"basecol ratio selected.*?selected"">(.*?)</span>");
        Regex peaksec = new Regex(@"peakcol secs selected.*?selected"">(.*?)</span>");
        Regex pealrat = new Regex(@"peakcol ratio selected.*?selected"">(.*?)</span>");
        void ItemMatch(DataRow dr, string name, Regex r,string page)
        {
            float test = 0;
            Match para = r.Match(page);
            if (para == null)
            {
                dr[name + "bases"] = 0;
                dr[name + "baser"] = 0;
                dr[name + "peaks"] = 0;
                dr[name + "peakr"] = 0;
                return;
            }
            Match select = basesec.Match(para.Value);
            if (select == null)
            {
                dr[name+"bases"] = 0;
            }
            else
            {
                dr[name+"bases"] = float.TryParse(select.Groups[1].Value, out test) ? test : 0;
            }
            select = baserat.Match(para.Value);
            if (select == null)
            {
                dr[name + "baser"] = 0;
            }
            else
            {
                dr[name + "baser"] = float.TryParse(select.Groups[1].Value, out test) ? test : 0;
            }
            select = peaksec.Match(para.Value);
            if (select == null)
            {
                dr[name + "peaks"] = 0;
            }
            else
            {
                dr[name + "peaks"] = float.TryParse(select.Groups[1].Value, out test) ? test : 0;
            }
            select = pealrat.Match(para.Value);
            if (select == null)
            {
                dr[name + "peakr"] = 0;
            }
            else
            {
                dr[name + "peakr"] = float.TryParse(select.Groups[1].Value, out test) ? test : 0;
            }
        }

        void IntDetail(DataRow dr, string page)
        {
            Regex r0 = new Regex(@"400.perlbench</a></td>[\s\S]*?</tr>");
            Regex r1 = new Regex(@"401.bzip2</a></td>[\s\S]*?</tr>");
            Regex r2 = new Regex(@"403.gcc</a></td>[\s\S]*?</tr>");
            Regex r3 = new Regex(@"429.mcf</a></td>[\s\S]*?</tr>");
            Regex r4 = new Regex(@"445.gobmk</a></td>[\s\S]*?</tr>");
            Regex r5 = new Regex(@"456.hmmer</a></td>[\s\S]*?</tr>");
            Regex r6 = new Regex(@"458.sjeng</a></td>[\s\S]*?</tr>");
            Regex r7 = new Regex(@"462.libquantum</a></td>[\s\S]*?</tr>");
            Regex r8 = new Regex(@"464.h264ref</a></td>[\s\S]*?</tr>");
            Regex r9 = new Regex(@"471.omnetpp</a></td>[\s\S]*?</tr>");
            Regex r10 = new Regex(@"473.astar</a></td>[\s\S]*?</tr>");
            Regex r11 = new Regex(@"483.xalancbmk</a></td>[\s\S]*?</tr>");
            ItemMatch(dr, "perlbench", r0, page);
            ItemMatch(dr, "bzip2", r1, page);
            ItemMatch(dr, "gcc", r2, page);
            ItemMatch(dr, "mcf", r3, page);
            ItemMatch(dr, "gobmk", r4, page);
            ItemMatch(dr, "hmmer", r5, page);
            ItemMatch(dr, "sjeng", r6, page);
            ItemMatch(dr, "libquantum", r7, page);
            ItemMatch(dr, "h264ref", r8, page);
            ItemMatch(dr, "omnetpp", r9, page);
            ItemMatch(dr, "astar", r10, page);
            ItemMatch(dr, "xalancbmk", r11, page);
        }

        void FPDetail(DataRow dr, string page)
        {
            Regex r0 = new Regex(@"410.bwaves</a></td>[\s\S]*?</tr>");
            Regex r1 = new Regex(@"416.gamess</a></td>[\s\S]*?</tr>");
            Regex r2 = new Regex(@"433.milc</a></td>[\s\S]*?</tr>");
            Regex r3 = new Regex(@"434.zeusmp</a></td>[\s\S]*?</tr>");
            Regex r4 = new Regex(@"435.gromacs</a></td>[\s\S]*?</tr>");
            Regex r5 = new Regex(@"436.cactusADM</a></td>[\s\S]*?</tr>");
            Regex r6 = new Regex(@"437.leslie3d</a></td>[\s\S]*?</tr>");
            Regex r7 = new Regex(@"444.namd</a></td>[\s\S]*?</tr>");
            Regex r8 = new Regex(@"447.dealII</a></td>[\s\S]*?</tr>");
            Regex r9 = new Regex(@"450.soplex</a></td>[\s\S]*?</tr>");
            Regex r10 = new Regex(@"453.povray</a></td>[\s\S]*?</tr>");
            Regex r11 = new Regex(@"454.calculix</a></td>[\s\S]*?</tr>");
            Regex r12 = new Regex(@"459.GemsFDTD</a></td>[\s\S]*?</tr>");
            Regex r13 = new Regex(@"465.tonto</a></td>[\s\S]*?</tr>");
            Regex r14 = new Regex(@"470.lbm</a></td>[\s\S]*?</tr>");
            Regex r15 = new Regex(@"481.wrf</a></td>[\s\S]*?</tr>");
            Regex r16 = new Regex(@"482.sphinx3</a></td>[\s\S]*?</tr>");
            ItemMatch(dr, "bwaves",r0,page);
            ItemMatch(dr, "gamess", r1, page);
            ItemMatch(dr, "milc", r2, page);
            ItemMatch(dr, "zeusmp", r3, page);
            ItemMatch(dr, "gromacs", r4, page);
            ItemMatch(dr, "cactusADM", r5, page);
            ItemMatch(dr, "leslie3d", r6, page);
            ItemMatch(dr, "namd", r7, page);
            ItemMatch(dr, "dealII", r8, page);
            ItemMatch(dr, "soplex", r9, page);
            ItemMatch(dr, "povray", r10, page);
            ItemMatch(dr, "calculix", r11, page);
            ItemMatch(dr, "GemsFDTD", r12, page);
            ItemMatch(dr, "tonto", r13, page);
            ItemMatch(dr, "lbm", r14, page);
            ItemMatch(dr, "wrf", r15, page);
            ItemMatch(dr, "sphinx3", r16, page);

        }
        //protected override void OnEndURLSave()
        //{
        //    StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + PreWorkFlow.SN + ".txt");
        //    while(Urls.Count>0)
        //    {
        //        sw.Write(Urls.Dequeue());
        //    }
        //    sw.Close();
        //}
    }

}
