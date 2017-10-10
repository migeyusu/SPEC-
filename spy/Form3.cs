using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace SepcReptile
{
    public partial class Form3 : Form
    {
        private readonly DataTable _seriesTable;
        private DataTable _nameTable;
        public SqlWorkUnit Database;
        public Form3()
        {
            InitializeComponent();
            listBox1.Sorted = true;
            Database = new SqlWorkUnit(Application.StartupPath + "\\database\\secret.mdf");
            _seriesTable = Database.ExuSqlDataTable("select * from series");
            ComboboxReflash();
            panel1.Enabled = false;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void ComboboxReflash()
        {
            comboBox1.Items.Clear();
            for (var i = 0; i < _seriesTable.Rows.Count; ++i)
            {
                comboBox1.Items.Add(_seriesTable.Rows[i]["series"].ToString());
            }
        }

        private void ListboxReflash()
        {
            listBox1.Items.Clear();
            for (var i = 0; i < _nameTable.Rows.Count; ++i)
            {
                listBox1.Items.Add(_nameTable.Rows[i]["name"].ToString());
            }
      
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                _nameTable = Database.ExuSqlDataTable("select * from names where series='" +
                    comboBox1.SelectedItem.ToString()+"'");
                ListboxReflash();
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
                textBox1.Text = _nameTable.Rows[listBox1.SelectedIndex]["url"].ToString();
                textBox2.Text = _nameTable.Rows[listBox1.SelectedIndex]["downloadurl"].ToString();
                textBox3.Text = _nameTable.Rows[listBox1.SelectedIndex]["name"].ToString();
                textBox4.Text = _nameTable.Rows[listBox1.SelectedIndex]["series"].ToString();
                _picname = _nameTable.Rows[listBox1.SelectedIndex]["picloc"].ToString();
                pictureBox1.ImageLocation = Application.StartupPath + "\\Pics\\" + _picname;
            }
            else
            {
                panel1.Enabled = false;
            }
        }
        //更改
        private void button3_Click(object sender, EventArgs e)
        {
            _nameTable.Rows[listBox1.SelectedIndex]["url"] = textBox1.Text;
            _nameTable.Rows[listBox1.SelectedIndex]["downloadurl"] = textBox2.Text;
            _nameTable.Rows[listBox1.SelectedIndex]["name"] = textBox3.Text;
            _nameTable.Rows[listBox1.SelectedIndex]["series"] = textBox4.Text;
            _nameTable.Rows[listBox1.SelectedIndex]["picloc"] = _picname;
            Database.Update(_nameTable);
        }

        private string _picname;
        private void button2_Click(object sender, EventArgs e)
        {
            var ofd=new OpenFileDialog();
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                _picname = ofd.SafeFileName;
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
            _nameTable.Rows.RemoveAt(listBox1.SelectedIndex);
            ListboxReflash();
        }
        //保存
        private void button1_Click(object sender, EventArgs e)
        {
            var dr = _nameTable.NewRow();
            dr["url"] = textBox1.Text;
            dr["downloadurl"] = textBox2.Text;
            dr["name"] = textBox3.Text;
            dr["picloc"] = _picname;
            dr["series"] = textBox4.Text;
            _nameTable.Rows.Add(dr);
            Database.Update(_nameTable);
            ListboxReflash();
            var append = true;
            for(var i=0;i<_seriesTable.Rows.Count;++i)
            {
                if(_seriesTable.Rows[i]["series"].ToString()==textBox4.Text)
                {
                    append = false;
                }
            }
            if(append)
            {
                var drs = _seriesTable.NewRow();
                drs["series"] = textBox4.Text;
                _seriesTable.Rows.Add(drs);
                Database.Update(_seriesTable);
                ComboboxReflash();
            } 
        }
    }
}
