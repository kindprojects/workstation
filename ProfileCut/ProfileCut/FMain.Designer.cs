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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelNavigator = new System.Windows.Forms.Panel();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panelPrinterButtons = new System.Windows.Forms.Panel();
            this.backgroundWorkerFormat = new System.ComponentModel.BackgroundWorker();
            this.labeOpt = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.listBoxOptimizations = new System.Windows.Forms.ListBox();
            this.webControlAwesomium = new Awesomium.Windows.Forms.WebControl(this.components);
            this.buttonHideOptimizationsList = new System.Windows.Forms.Button();
            this.buttonCut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelNavigator
            // 
            this.panelNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNavigator.Location = new System.Drawing.Point(224, 451);
            this.panelNavigator.Name = "panelNavigator";
            this.panelNavigator.Size = new System.Drawing.Size(390, 38);
            this.panelNavigator.TabIndex = 4;
            // 
            // panelPrinterButtons
            // 
            this.panelPrinterButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPrinterButtons.Location = new System.Drawing.Point(620, 451);
            this.panelPrinterButtons.Name = "panelPrinterButtons";
            this.panelPrinterButtons.Size = new System.Drawing.Size(106, 38);
            this.panelPrinterButtons.TabIndex = 9;
            // 
            // labeOpt
            // 
            this.labeOpt.AutoSize = true;
            this.labeOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labeOpt.Location = new System.Drawing.Point(9, 11);
            this.labeOpt.Name = "labeOpt";
            this.labeOpt.Size = new System.Drawing.Size(98, 16);
            this.labeOpt.TabIndex = 8;
            this.labeOpt.Text = "Оптимизации";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(3, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.labeOpt);
            this.splitContainer.Panel1.Controls.Add(this.buttonRefresh);
            this.splitContainer.Panel1.Controls.Add(this.listBoxOptimizations);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webControlAwesomium);
            this.splitContainer.Size = new System.Drawing.Size(723, 440);
            this.splitContainer.SplitterDistance = 220;
            this.splitContainer.TabIndex = 13;
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonRefresh.BackgroundImage = global::ProfileCut.Resource.refresh2;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(177, 0);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(40, 38);
            this.buttonRefresh.TabIndex = 9;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // listBoxOptimizations
            // 
            this.listBoxOptimizations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxOptimizations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxOptimizations.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxOptimizations.FormattingEnabled = true;
            this.listBoxOptimizations.IntegralHeight = false;
            this.listBoxOptimizations.ItemHeight = 25;
            this.listBoxOptimizations.Location = new System.Drawing.Point(3, 44);
            this.listBoxOptimizations.Name = "listBoxOptimizations";
            this.listBoxOptimizations.Size = new System.Drawing.Size(214, 398);
            this.listBoxOptimizations.TabIndex = 12;
            this.listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            // 
            // webControl1
            // 
            this.webControlAwesomium.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControlAwesomium.Location = new System.Drawing.Point(0, 0);
            this.webControlAwesomium.Size = new System.Drawing.Size(499, 440);
            this.webControlAwesomium.TabIndex = 0;
            this.webControlAwesomium.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // buttonHideOptimizationsList
            // 
            this.buttonHideOptimizationsList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonHideOptimizationsList.BackgroundImage = global::Platform.Properties.Resources.arrows;
            this.buttonHideOptimizationsList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHideOptimizationsList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHideOptimizationsList.Location = new System.Drawing.Point(178, 451);
            this.buttonHideOptimizationsList.Name = "buttonHideOptimizationsList";
            this.buttonHideOptimizationsList.Size = new System.Drawing.Size(40, 38);
            this.buttonHideOptimizationsList.TabIndex = 14;
            this.buttonHideOptimizationsList.UseVisualStyleBackColor = true;
            this.buttonHideOptimizationsList.Click += new System.EventHandler(this.buttonHideOptimizationsList_Click);
            // 
            // buttonCut
            // 
            this.buttonCut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCut.BackgroundImage = global::ProfileCut.Resource.check;
            this.buttonCut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonCut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCut.Location = new System.Drawing.Point(3, 451);
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
            this.ClientSize = new System.Drawing.Size(737, 501);
            this.Controls.Add(this.buttonHideOptimizationsList);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.buttonCut);
            this.Controls.Add(this.panelPrinterButtons);
            this.Controls.Add(this.panelNavigator);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FMain";
            this.Text = "Распил профиля";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelNavigator;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Panel panelPrinterButtons;
        private System.Windows.Forms.Button buttonCut;
        private System.ComponentModel.BackgroundWorker backgroundWorkerFormat;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Label labeOpt;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox listBoxOptimizations;
        private Awesomium.Windows.Forms.WebControl webControlAwesomium;
        private System.Windows.Forms.Button buttonHideOptimizationsList;        
    }
}

