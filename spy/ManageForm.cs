using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SepcReptile
{
    public partial class ManageForm : Form
    {
        public ManageForm()
        {
            InitializeComponent();
            Ini();
        }
        public SqlWorkUnit Database;
        public DataTable Pre;
        private DataSet _scoreCollection = new System.Data.DataSet();
        private List<string> _amdcpuSeries = new List<string>(), _intelCpuSeries = new List<string>();
        private void Manage_Load(object sender, EventArgs e)
        {
            
        }

        private void Ini()
        {
            scoretype.Items.Add("单线程整数");
            scoretype.Items.Add("单线程浮点");
            scoretype.Items.Add("多线程整数");
            scoretype.Items.Add("多线程浮点");
            scoretype.SelectedIndex = 0;
            //cpuMf.Items.Add("AMD");
            //cpuMf.Items.Add("Intel");
            //cpuMf.SelectedIndex = 0;
            Database = new SqlWorkUnit(Application.StartupPath + "\\GPUBenchmark.mdf", @".\SQLEXPRESS");
            //listView1.View = View.LargeIcon;
            //DataTable dt0 = Database.ExuSQLDataTable("select * from int");
            //dt0.TableName = "单线程整数";
            //ScoreCollection.Tables.Add(dt0);
            //DataTable dt1 = Database.ExuSQLDataTable("select * from fp");
            //dt1.TableName = "单线程浮点";
            //ScoreCollection.Tables.Add(dt1);
            //DataTable dt2 = Database.ExuSQLDataTable("select * from intrates");
            //dt2.TableName = "多线程整数";
            //ScoreCollection.Tables.Add(dt2);
            //DataTable dt3 = Database.ExuSQLDataTable("select * from fprates");
            //dt3.TableName = "多线程浮点";
            //ScoreCollection.Tables.Add(dt3);
            var icos = new ImageList();
            icos.Images.Add(Properties.Resources.A10);//0
            icos.Images.Add(Properties.Resources.apu);//1
            icos.Images.Add(Properties.Resources.atom);//2
            icos.Images.Add(Properties.Resources.i3);//3
            icos.Images.Add(Properties.Resources.i5);//4
            icos.Images.Add(Properties.Resources.i7); //5
            icos.Images.Add(Properties.Resources.奔腾);//6
            icos.Images.Add(Properties.Resources.酷睿2);//7
            icos.Images.Add(Properties.Resources.酷睿2E);//8
            icos.Images.Add(Properties.Resources.酷睿2q);//9
            icos.Images.Add(Properties.Resources.赛扬1);//10
            icos.Images.Add(Properties.Resources.速龙);//11
            icos.Images.Add(Properties.Resources.土龙);//12
            icos.Images.Add(Properties.Resources.羿龙);//13
            icos.Images.Add(Properties.Resources.至强);//14
            icos.Images.Add(Properties.Resources.logo);//15
            icos.Images.Add(Properties.Resources.ico2);//16 intel
            icos.Images.Add(Properties.Resources.fx);//17
            icos.Images.Add(Properties.Resources.皓龙);//18
            var amd = new TreeNode("AMD",15,15);
            amd.Nodes.Add("A10","A10",0);
            amd.Nodes.Add("A8", "A8", 1);
            amd.Nodes.Add("A6", "A6", 1);
            amd.Nodes.Add("A4", "A4", 1);
            amd.Nodes.Add("FX", "FX", 17);
            amd.Nodes.Add("Turion", "Turion", 12);
            var athlon = new TreeNode("Athlon",1,1);
            athlon.Nodes.Add("Athlon 64","Athlon 64",1);
            athlon.Nodes.Add("Athlon X2","Athlon X2",1);
            athlon.Nodes.Add("Athlon II","Athlon II",1);
            var opteron = new TreeNode("Opteron",18,18);
            opteron.Nodes.Add("Opteron 1","Opteron 1",18);
            opteron.Nodes.Add("Opteron 2", "Opteron 2", 18);
            opteron.Nodes.Add("Opteron 4", "Opteron 4", 18);
            opteron.Nodes.Add("Opteron 6", "Opteron 6", 18);
            opteron.Nodes.Add("Opteron 8", "Opteron 8", 18);
            var phenom = new TreeNode("Phenom",13,13);
            phenom.Nodes.Add("Phenom X4", "Phenom X4", 13);
            phenom.Nodes.Add("Phenom X3", "Phenom X3", 13);
            phenom.Nodes.Add("Phenom II","Phenom II",13);
            amd.Nodes.Add(athlon);
            amd.Nodes.Add(opteron);
            amd.Nodes.Add(phenom);
            CPUseriesView.Nodes.Add(amd);
            
            var intel = new TreeNode("Intel",16,16);
            intel.Nodes.Add("Atom", "Atom", 2);
            var celeron = new TreeNode("Celeron",10,10);
            celeron.Nodes.Add("Celeron 4", "Celeron 4", 10);
            celeron.Nodes.Add("Celeron E", "Celeron E", 10);
            celeron.Nodes.Add("Celeron G", "Celeron G", 10);
            var core = new TreeNode("Core 2",7,7);
            core.Nodes.Add("Core 2 Duo E", "Core 2 Duo E", 7);
            core.Nodes.Add("Core 2 Duo T", "Core 2 Duo T", 7);
            core.Nodes.Add("Core 2 Duo X", "Core 2 Duo X", 8);
            core.Nodes.Add("Core 2 Extreme","Core 2 Extreme",8);
            core.Nodes.Add("Core 2 Quad","Core 2 Quad",9);
            var i3 = new TreeNode("Core i3",3,3);
            i3.Nodes.Add("Core i3-2","Core i3-2",3);
            i3.Nodes.Add("Core i3-3","Core i3-3",3);
            i3.Nodes.Add("Core i3-4","Core i3-4",3);
            i3.Nodes.Add("Core i3-5","Core i3-5",3);
            i3.Nodes.Add("Core i3-6","Core i3-6",3);
            var i5 = new TreeNode("Core i5",4,4);
            i5.Nodes.Add("Core i5-2","Core i5-2",4);
            i5.Nodes.Add("Core i5-3","Core i5-3",4);
            i5.Nodes.Add("Core i5-4","Core i5-4",4);
            i5.Nodes.Add("Core i5-5","Core i5-5",4);
            i5.Nodes.Add("Core i5-6","Core i5-6",4);
            var i7 = new TreeNode("Core i7",5,5);
            i7.Nodes.Add("Core i7-2","Core i7-2",5);
            i7.Nodes.Add("Core i7-3","Core i7-3",5);
            i7.Nodes.Add("Core i7-4","Core i7-4",5);
            i7.Nodes.Add("Core i7-5","Core i7-5",5);
            i7.Nodes.Add("Core i7-6","Core i7-6",5);
            i7.Nodes.Add("Core i7-8","Core i7-8",5);
            i7.Nodes.Add("Core i7-9","Core i7-9",5);
            var pentium = new TreeNode("Pentium",6,6);
            pentium.Nodes.Add("Pentium 4","Pentium 4",6);
            pentium.Nodes.Add("Pentium D","Pentium D",6);
            pentium.Nodes.Add("Pentium E","Pentium E",6);
            pentium.Nodes.Add("Pentium G","Pentium G",6);
            pentium.Nodes.Add("Pentium M","Pentium M",6);
            var xeon = new TreeNode("Xeon",14,14);
            xeon.Nodes.Add("Xeon 3","Xeon 3",14);
            xeon.Nodes.Add("Xeon 5","Xeon 5",14);
            xeon.Nodes.Add("Xeon 7","Xeon 7",14);
            xeon.Nodes.Add("Xeon D","Xeon D",14);
            xeon.Nodes.Add("Xeon E","Xeon E",14);
            xeon.Nodes.Add("Xeon L","Xeon L",14);
            xeon.Nodes.Add("Xeon W", "Xeon W", 14);
            xeon.Nodes.Add("Xeon X", "Xeon X", 14);
            xeon.Nodes.Add("Xeon E3-", "Xeon E3-", 14);
            xeon.Nodes.Add("Xeon E5-", "Xeon E5-", 14);
            xeon.Nodes.Add("Xeon E7-", "Xeon E7-", 14);
            intel.Nodes.Add(celeron);
            intel.Nodes.Add(core);
            intel.Nodes.Add(i3);
            intel.Nodes.Add(i5);
            intel.Nodes.Add(i7);
            intel.Nodes.Add(xeon);
            intel.Nodes.Add(pentium);
            CPUseriesView.Nodes.Add(intel);
            CPUseriesView.ImageList = icos;
            CPUseriesView.ImageList.ImageSize = new Size(70, 50);
        }
        //cpu
        private void label8_Click(object sender, EventArgs e)
        {
            //listView1.Items.Clear();
            //string query = string.Format("select * from {0} where cpu like '%{1}%'",
            //    scoretype.SelectedItem.ToString(), cpuMf.SelectedItem.ToString());
            //Pre = Database.ExuSQLDataTable(query, false);
            //Pre.TableName = "int";
            //dataGridView1.DataSource = Pre;
            //DataView dv = Pre.DefaultView;
            //DataTable dt = dv.ToTable(true,"cpu");
            //listView1.BeginUpdate();
            //foreach (DataRow dr in dt.Rows)
            //{

            //    listView1.Items.Add(dr["cpu"].ToString());

            //}
            //listView1.EndUpdate();
        }
        //os
        private void label1_Click(object sender, EventArgs e)
        {
            //listView1.Items.Clear();
            //string query = string.Format("select cpu from {0} where cpu like '%{1}%'",
            //    scoretype.SelectedItem.ToString(), cpuMf.SelectedItem.ToString());
            //DataTable dt = Database.ExuSQLDataTable(query, false);

            //listView1.BeginUpdate();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    listView1.Items.Add(dr["cpu"].ToString());
            //}
            //listView1.EndUpdate();
        }
        //os version
        private void label2_Click(object sender, EventArgs e)
        {
            //listView1.Items.Clear();
            //string query = string.Format("select os from {0} where cpu like '%{1}%'",
            //    scoretype.SelectedItem.ToString(), cpuMf.SelectedItem.ToString());
            //DataTable dt = Database.ExuSQLDataTable(query, false);
            //listView1.BeginUpdate();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    listView1.Items.Add(dr["os"].ToString());
            //}
            //listView1.EndUpdate();
        }
        //cp
        private void label3_Click(object sender, EventArgs e)
        {
            //listView1.Items.Clear();
            //string query = string.Format("select cpu from {0} where cpu like '%{1}%'",
            //    scoretype.SelectedItem.ToString(), cpuMf.SelectedItem.ToString());
            //DataTable dt = Database.ExuSQLDataTable(query, false);
            //listView1.BeginUpdate();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    listView1.Items.Add(dr["cpu"].ToString());
            //}
            //listView1.EndUpdate();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count==0)
                return;
            label6.Text = listView1.SelectedItems[0].Text;
        }
    }
}
