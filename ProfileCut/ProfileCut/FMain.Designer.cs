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
            this.labeOpt = new System.Windows.Forms.Label();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panelAppCommands = new System.Windows.Forms.Panel();
            this.buttonMarkOpimization = new System.Windows.Forms.Button();
            this.splitContainerAwesomium = new System.Windows.Forms.SplitContainer();
            this.listBoxOptimizations = new System.Windows.Forms.ListBox();
            this.webControlDetails = new Awesomium.Windows.Forms.WebControl(this.components);
            this.buttonHide = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAwesomium)).BeginInit();
            this.splitContainerAwesomium.Panel1.SuspendLayout();
            this.splitContainerAwesomium.Panel2.SuspendLayout();
            this.splitContainerAwesomium.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelNavigator
            // 
            this.panelNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNavigator.Location = new System.Drawing.Point(217, 484);
            this.panelNavigator.Name = "panelNavigator";
            this.panelNavigator.Size = new System.Drawing.Size(460, 38);
            this.panelNavigator.TabIndex = 4;
            // 
            // labeOpt
            // 
            this.labeOpt.AutoSize = true;
            this.labeOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labeOpt.Location = new System.Drawing.Point(14, 14);
            this.labeOpt.Name = "labeOpt";
            this.labeOpt.Size = new System.Drawing.Size(98, 16);
            this.labeOpt.TabIndex = 6;
            this.labeOpt.Text = "Оптимизации";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackgroundImage = global::ProfileCut.Resource.refresh2;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(169, 3);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(40, 38);
            this.buttonRefresh.TabIndex = 7;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // panelAppCommands
            // 
            this.panelAppCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAppCommands.Location = new System.Drawing.Point(683, 484);
            this.panelAppCommands.Name = "panelAppCommands";
            this.panelAppCommands.Size = new System.Drawing.Size(76, 38);
            this.panelAppCommands.TabIndex = 9;
            // 
            // buttonMarkOpimization
            // 
            this.buttonMarkOpimization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMarkOpimization.BackgroundImage = global::ProfileCut.Resource.check;
            this.buttonMarkOpimization.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonMarkOpimization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMarkOpimization.Location = new System.Drawing.Point(7, 484);
            this.buttonMarkOpimization.Name = "buttonMarkOpimization";
            this.buttonMarkOpimization.Size = new System.Drawing.Size(40, 38);
            this.buttonMarkOpimization.TabIndex = 10;
            this.buttonMarkOpimization.UseVisualStyleBackColor = true;
            this.buttonMarkOpimization.Click += new System.EventHandler(this.buttonMarkOptimization_Click);
            // 
            // splitContainerAwesomium
            // 
            this.splitContainerAwesomium.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerAwesomium.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerAwesomium.Location = new System.Drawing.Point(1, 3);
            this.splitContainerAwesomium.Name = "splitContainerAwesomium";
            // 
            // splitContainerAwesomium.Panel1
            // 
            this.splitContainerAwesomium.Panel1.Controls.Add(this.listBoxOptimizations);
            this.splitContainerAwesomium.Panel1.Controls.Add(this.buttonRefresh);
            this.splitContainerAwesomium.Panel1.Controls.Add(this.labeOpt);
            // 
            // splitContainerAwesomium.Panel2
            // 
            this.splitContainerAwesomium.Panel2.Controls.Add(this.webControlDetails);
            this.splitContainerAwesomium.Size = new System.Drawing.Size(758, 475);
            this.splitContainerAwesomium.SplitterDistance = 212;
            this.splitContainerAwesomium.TabIndex = 11;
            // 
            // listBoxOptimizations
            // 
            this.listBoxOptimizations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxOptimizations.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxOptimizations.FormattingEnabled = true;
            this.listBoxOptimizations.IntegralHeight = false;
            this.listBoxOptimizations.ItemHeight = 25;
            this.listBoxOptimizations.Location = new System.Drawing.Point(6, 47);
            this.listBoxOptimizations.Name = "listBoxOptimizations";
            this.listBoxOptimizations.Size = new System.Drawing.Size(203, 425);
            this.listBoxOptimizations.TabIndex = 1;
            this.listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            // 
            // webControlDetails
            // 
            this.webControlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControlDetails.Location = new System.Drawing.Point(0, 0);
            this.webControlDetails.Size = new System.Drawing.Size(542, 475);
            this.webControlDetails.TabIndex = 0;
            this.webControlDetails.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // buttonHide
            // 
            this.buttonHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonHide.BackgroundImage = global::ProfileCut.Resource.arrows;
            this.buttonHide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHide.Location = new System.Drawing.Point(170, 484);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(40, 38);
            this.buttonHide.TabIndex = 12;
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 534);
            this.Controls.Add(this.buttonHide);
            this.Controls.Add(this.splitContainerAwesomium);
            this.Controls.Add(this.buttonMarkOpimization);
            this.Controls.Add(this.panelAppCommands);
            this.Controls.Add(this.panelNavigator);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FMain";
            this.Text = "Распил профиля";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.splitContainerAwesomium.Panel1.ResumeLayout(false);
            this.splitContainerAwesomium.Panel1.PerformLayout();
            this.splitContainerAwesomium.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAwesomium)).EndInit();
            this.splitContainerAwesomium.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelNavigator;
        private System.Windows.Forms.Label labeOpt;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Panel panelAppCommands;
        private System.Windows.Forms.Button buttonMarkOpimization;
        private System.Windows.Forms.SplitContainer splitContainerAwesomium;
        private System.Windows.Forms.ListBox listBoxOptimizations;
        private Awesomium.Windows.Forms.WebControl webControlDetails;
        private System.Windows.Forms.Button buttonHide;
    }
}

