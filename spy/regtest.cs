using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SepcReptile
{
    public partial class Regtest : Form
    {
        public Regtest()
        {
            InitializeComponent();
        }

        private void regtest_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rt = new Regex(textBox1.Text);
            if(rt.IsMatch(textBox2.Text))
            {
               
            }
        }
    }
}
