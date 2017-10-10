using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SepcReptile
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
