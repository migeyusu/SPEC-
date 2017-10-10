using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace SepcReptile
{
    public partial class Form3 : Form
    {
        DataTable seriesTable, nameTable;
        public SqlWorkUnit database;
        public Form3()
        {
            InitializeComponent();
            listBox1.Sorted = true;
            database = new SqlWorkUnit(Application.StartupPath + "\\database\\secret.mdf");
            seriesTable = database.ExuSQLDataTable("select * from series");
            comboboxReflash();
            panel1.Enabled = false;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        void comboboxReflash()
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < seriesTable.Rows.Count; ++i)
            {
                comboBox1.Items.Add(seriesTable.Rows[i]["series"].ToString());
            }
        }

        void listboxReflash()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < nameTable.Rows.Count; ++i)
            {
                listBox1.Items.Add(nameTable.Rows[i]["name"].ToString());
            }
      
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                nameTable = database.ExuSQLDataTable("select * from names where series='" +
                    comboBox1.SelectedItem.ToString()+"'");
                listboxReflash();
            }
        }
        /// <summary>
        /// 每次添加都会更新index，直接用index做索引
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                panel1.Enabled = true;
                textBox1.Text = nameTable.Rows[listBox1.SelectedIndex]["url"].ToString();
                textBox2.Text = nameTable.Rows[listBox1.SelectedIndex]["downloadurl"].ToString();
                textBox3.Text = nameTable.Rows[listBox1.SelectedIndex]["name"].ToString();
                textBox4.Text = nameTable.Rows[listBox1.SelectedIndex]["series"].ToString();
                picname = nameTable.Rows[listBox1.SelectedIndex]["picloc"].ToString();
                pictureBox1.ImageLocation = Application.StartupPath + "\\Pics\\" + picname;
            }
            else
            {
                panel1.Enabled = false;
            }
        }
        //更改
        private void button3_Click(object sender, EventArgs e)
        {
            nameTable.Rows[listBox1.SelectedIndex]["url"] = textBox1.Text;
            nameTable.Rows[listBox1.SelectedIndex]["downloadurl"] = textBox2.Text;
            nameTable.Rows[listBox1.SelectedIndex]["name"] = textBox3.Text;
            nameTable.Rows[listBox1.SelectedIndex]["series"] = textBox4.Text;
            nameTable.Rows[listBox1.SelectedIndex]["picloc"] = picname;
            database.Update(nameTable);
        }
        string picname;
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                picname = ofd.SafeFileName;
                pictureBox1.ImageLocation = ofd.FileName;
                if(!File.Exists(Application.StartupPath + "\\Pics\\" + ofd.SafeFileName))
                {
                    File.Copy(ofd.FileName, Application.StartupPath + "\\Pics\\" + ofd.SafeFileName);
                }
            }
        }
        //删除
        private void button4_Click(object sender, EventArgs e)
        {
            nameTable.Rows.RemoveAt(listBox1.SelectedIndex);
            listboxReflash();
        }
        //保存
        private void button1_Click(object sender, EventArgs e)
        {
            DataRow dr = nameTable.NewRow();
            dr["url"] = textBox1.Text;
            dr["downloadurl"] = textBox2.Text;
            dr["name"] = textBox3.Text;
            dr["picloc"] = picname;
            dr["series"] = textBox4.Text;
            nameTable.Rows.Add(dr);
            database.Update(nameTable);
            listboxReflash();
            bool append = true;
            for(int i=0;i<seriesTable.Rows.Count;++i)
            {
                if(seriesTable.Rows[i]["series"].ToString()==textBox4.Text)
                {
                    append = false;
                }
            }
            if(append)
            {
                DataRow drs = seriesTable.NewRow();
                drs["series"] = textBox4.Text;
                seriesTable.Rows.Add(drs);
                database.Update(seriesTable);
                comboboxReflash();
            } 
        }
    }
}
