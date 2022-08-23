using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nkv.Automator
{
    public partial class LogTextAreaControl : UserControl
    {
        public LogTextAreaControl()
        {
            InitializeComponent();
        }
        public void SetLogText(string msg, Color color,bool clearPrior = false)
        {
            if (clearPrior)
            {
                richTextBox1.Clear();
            }

            richTextBox1.SelectionColor = color;
            richTextBox1.SelectedText = " " + msg + "\r\n";
            richTextBox1.Update();
        }
    }
}
