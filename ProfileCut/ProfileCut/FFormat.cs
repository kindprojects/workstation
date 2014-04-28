using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Platform2;

namespace ProfileCut
{
    public partial class FFormat : Form
    {
        public string Result { set; get; }
        
        private int _left;
        private int _top;
        private IPObject _formatObject;
        private string _template;

        public FFormat(int left, int top, IPObject formatObject, string template)
        {
            InitializeComponent();
            
            _left = left;
            _top = top;

            _formatObject = formatObject;
            _template = template;

            backgroundWorkerFormat.WorkerSupportsCancellation = true;
        }

        private void FProgress_Shown(object sender, EventArgs e)
        {
            this.Left = _left;
            this.Top = _top;

            backgroundWorkerFormat.RunWorkerAsync();
        }
       
        private void backgroundWorkerFormat_DoWork(object sender, DoWorkEventArgs e)
        {
            Result = "";
            e.Result = _formatObject.Format(_template, backgroundWorkerFormat);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {            
            backgroundWorkerFormat.CancelAsync();
            //Result = "";
            //this.Close();
        }

        private void backgroundWorkerFormat_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = e.Result.ToString();
            this.Close();
        }
    }
}
