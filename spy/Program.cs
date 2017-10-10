using System;
using System.Windows.Forms;

namespace SepcReptile
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form3 fm = new Form3();
            Application.Run(fm);
            fm.database.Clear();
        }
    }
}
