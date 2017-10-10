using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace spy
{
    public partial class regtest : Form
    {
        public regtest()
        {
            InitializeComponent();
        }

        private void regtest_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Regex rt = new Regex(textBox1.Text);
            if(rt.IsMatch(textBox2.Text))
            {
               
            }
        }
    }
}
