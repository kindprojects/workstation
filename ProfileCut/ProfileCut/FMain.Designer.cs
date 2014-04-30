namespace ProfileCut
{
    partial class FMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.listBoxOptimizations = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelNavigator = new System.Windows.Forms.Panel();
            this.labeOpt = new System.Windows.Forms.Label();
            this.panelAwesomiumWebControl = new System.Windows.Forms.Panel();
            this.webControlDetails = new Awesomium.Windows.Forms.WebControl(this.components);
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panelAppCommands = new System.Windows.Forms.Panel();
            this.buttonCut = new System.Windows.Forms.Button();
            this.panelAwesomiumWebControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxOptimizations
            // 
            this.listBoxOptimizations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxOptimizations.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxOptimizations.FormattingEnabled = true;
            this.listBoxOptimizations.IntegralHeight = false;
            this.listBoxOptimizations.ItemHeight = 25;
            this.listBoxOptimizations.Location = new System.Drawing.Point(12, 49);
            this.listBoxOptimizations.Name = "listBoxOptimizations";
            this.listBoxOptimizations.Size = new System.Drawing.Size(203, 435);
            this.listBoxOptimizations.TabIndex = 0;
            this.listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            // 
            // panelNavigator
            // 
            this.panelNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNavigator.Location = new System.Drawing.Point(221, 490);
            this.panelNavigator.Name = "panelNavigator";
            this.panelNavigator.Size = new System.Drawing.Size(334, 38);
            this.panelNavigator.TabIndex = 4;
            // 
            // labeOpt
            // 
            this.labeOpt.AutoSize = true;
            this.labeOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labeOpt.Location = new System.Drawing.Point(9, 16);
            this.labeOpt.Name = "labeOpt";
            this.labeOpt.Size = new System.Drawing.Size(98, 16);
            this.labeOpt.TabIndex = 6;
            this.labeOpt.Text = "Оптимизации";
            // 
            // panelAwesomiumWebControl
            // 
            this.panelAwesomiumWebControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAwesomiumWebControl.Controls.Add(this.webControlDetails);
            this.panelAwesomiumWebControl.Location = new System.Drawing.Point(221, 5);
            this.panelAwesomiumWebControl.Name = "panelAwesomiumWebControl";
            this.panelAwesomiumWebControl.Size = new System.Drawing.Size(532, 479);
            this.panelAwesomiumWebControl.TabIndex = 8;
            // 
            // webControlDetails
            // 
            this.webControlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControlDetails.Location = new System.Drawing.Point(0, 0);
            this.webControlDetails.Size = new System.Drawing.Size(532, 479);
            this.webControlDetails.TabIndex = 0;
            this.webControlDetails.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackgroundImage = global::ProfileCut.Resource.refresh2;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(175, 5);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(40, 38);
            this.buttonRefresh.TabIndex = 7;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // panelPrinterButtons
            // 
            this.panelAppCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAppCommands.Location = new System.Drawing.Point(561, 490);
            this.panelAppCommands.Name = "panelPrinterButtons";
            this.panelAppCommands.Size = new System.Drawing.Size(192, 38);
            this.panelAppCommands.TabIndex = 9;
            // 
            // buttonCut
            // 
            this.buttonCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCut.BackgroundImage = global::ProfileCut.Resource.check;
            this.buttonCut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonCut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCut.Location = new System.Drawing.Point(12, 490);
            this.buttonCut.Name = "buttonCut";
            this.buttonCut.Size = new System.Drawing.Size(40, 38);
            this.buttonCut.TabIndex = 10;
            this.buttonCut.UseVisualStyleBackColor = true;
            this.buttonCut.Click += new System.EventHandler(this.buttonCut_Click);
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 540);
            this.Controls.Add(this.buttonCut);
            this.Controls.Add(this.panelAppCommands);
            this.Controls.Add(this.panelAwesomiumWebControl);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.labeOpt);
            this.Controls.Add(this.panelNavigator);
            this.Controls.Add(this.listBoxOptimizations);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FMain";
            this.Text = "Распил профиля";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.panelAwesomiumWebControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxOptimizations;        
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelNavigator;
        private System.Windows.Forms.Label labeOpt;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Panel panelAwesomiumWebControl;
        private Awesomium.Windows.Forms.WebControl webControlDetails;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Panel panelAppCommands;
        private System.Windows.Forms.Button buttonCut;
    }
}

