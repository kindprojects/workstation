using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Platform2;
using ModuleConnect;

namespace ProfileCut
{
    public partial class FFormat : Form
    {
        public string GeneratedHtml { set; get; }
		public string ExceptionMessage;
        private int _left;
        private int _top;
        private IPObject _formatObject;
        private Label label1;
        private Button buttonCancel;
        private ProgressBar progressBar1;
        private BackgroundWorker backgroundWorkerFormat;
        private string _template;
        private IMHost _host;
        private IMValueGetter _overloads;

		public List<string> parsedNavLevels;
		public List<string> parsedNavCaptions;


        public FFormat(int left, int top, IPObject formatObject, string template, IMHost host, IMValueGetter overloads)
        {
            InitializeComponent();
            
            _left = left;
            _top = top;

            _formatObject = formatObject;
            _template = template;
            _host = host;
            _overloads = overloads;

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
			try
			{
				ExceptionMessage = "";
				GeneratedHtml = "";
				PNavigationInfo navInfo = new PNavigationInfo();
				e.Result = PTemplates.FormatObject(_formatObject, _template, _host, _overloads, backgroundWorkerFormat, navInfo);
				this.parsedNavLevels = navInfo.levels;
				this.parsedNavCaptions = navInfo.captions;
			}
			catch (Exception ex)
			{
				e.Result = "exception:" + ex.Message;
				this.ExceptionMessage = ex.Message;
			}
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            backgroundWorkerFormat.CancelAsync();
        }

        private void backgroundWorkerFormat_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GeneratedHtml = e.Result.ToString();
            this.Close();
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerFormat = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Идет загрузка данных";
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Location = new System.Drawing.Point(106, 82);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 33);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 34);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(260, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.UseWaitCursor = true;
            // 
            // backgroundWorkerFormat
            // 
            this.backgroundWorkerFormat.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerFormat_DoWork);
            this.backgroundWorkerFormat.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerFormat_RunWorkerCompleted);
            // 
            // FFormat
            // 
            this.ClientSize = new System.Drawing.Size(285, 131);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FFormat";
            this.Shown += new System.EventHandler(this.FProgress_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }      
    }
}
