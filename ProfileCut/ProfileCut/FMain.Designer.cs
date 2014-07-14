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
            this.timerListOptimizationsRefresh = new System.Windows.Forms.Timer(this.components);
            this.labeOpt = new System.Windows.Forms.Label();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonMarkOpimization = new System.Windows.Forms.Button();
            this.splitContainerAwesomium = new System.Windows.Forms.SplitContainer();
            this.listBoxOptimizations = new System.Windows.Forms.ListBox();
            this.webControlDetails = new Awesomium.Windows.Forms.WebControl(this.components);
            this.buttonHide = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.buttonOptRemove = new System.Windows.Forms.Button();
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.panelOptimizationButtons = new System.Windows.Forms.Panel();
            this.panelNavigator = new System.Windows.Forms.Panel();
            this.panelAppCommands = new System.Windows.Forms.Panel();
            this.panelExitButton = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAwesomium)).BeginInit();
            this.splitContainerAwesomium.Panel1.SuspendLayout();
            this.splitContainerAwesomium.Panel2.SuspendLayout();
            this.splitContainerAwesomium.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.panelOptimizationButtons.SuspendLayout();
            this.panelExitButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerListOptimizationsRefresh
            // 
            this.timerListOptimizationsRefresh.Tick += new System.EventHandler(this.timer1_Tick);
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
            // buttonMarkOpimization
            // 
            this.buttonMarkOpimization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMarkOpimization.BackgroundImage = global::ProfileCut.Resource.check;
            this.buttonMarkOpimization.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonMarkOpimization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMarkOpimization.Location = new System.Drawing.Point(3, 1);
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
            this.splitContainerAwesomium.Size = new System.Drawing.Size(741, 439);
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
            this.listBoxOptimizations.Size = new System.Drawing.Size(203, 387);
            this.listBoxOptimizations.TabIndex = 1;
            this.listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            // 
            // webControlDetails
            // 
            this.webControlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webControlDetails.Location = new System.Drawing.Point(3, 3);
            this.webControlDetails.Size = new System.Drawing.Size(519, 433);
            this.webControlDetails.TabIndex = 0;
            this.webControlDetails.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // buttonHide
            // 
            this.buttonHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonHide.BackgroundImage = global::ProfileCut.Resource.arrows;
            this.buttonHide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHide.Location = new System.Drawing.Point(160, 1);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(40, 38);
            this.buttonHide.TabIndex = 12;
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(6, 1);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(85, 38);
            this.btnExit.TabIndex = 13;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // buttonOptRemove
            // 
            this.buttonOptRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOptRemove.BackgroundImage = global::ProfileCut.Resource.remove;
            this.buttonOptRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonOptRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOptRemove.Location = new System.Drawing.Point(49, 1);
            this.buttonOptRemove.Name = "buttonOptRemove";
            this.buttonOptRemove.Size = new System.Drawing.Size(40, 38);
            this.buttonOptRemove.TabIndex = 14;
            this.buttonOptRemove.UseVisualStyleBackColor = true;
            this.buttonOptRemove.Click += new System.EventHandler(this.buttonOptRemove_Click);
            // 
            // tableLayoutPanelButtons
            // 
            this.tableLayoutPanelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelButtons.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelButtons.ColumnCount = 4;
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelButtons.Controls.Add(this.panelOptimizationButtons, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelNavigator, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelAppCommands, 2, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelExitButton, 3, 0);
            this.tableLayoutPanelButtons.Location = new System.Drawing.Point(1, 448);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            this.tableLayoutPanelButtons.RowCount = 1;
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.Size = new System.Drawing.Size(741, 50);
            this.tableLayoutPanelButtons.TabIndex = 15;
            // 
            // panelOptimizationButtons
            // 
            this.panelOptimizationButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelOptimizationButtons.Controls.Add(this.buttonMarkOpimization);
            this.panelOptimizationButtons.Controls.Add(this.buttonOptRemove);
            this.panelOptimizationButtons.Controls.Add(this.buttonHide);
            this.panelOptimizationButtons.Location = new System.Drawing.Point(4, 4);
            this.panelOptimizationButtons.Name = "panelOptimizationButtons";
            this.panelOptimizationButtons.Size = new System.Drawing.Size(204, 42);
            this.panelOptimizationButtons.TabIndex = 0;
            // 
            // panelNavigator
            // 
            this.panelNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNavigator.Location = new System.Drawing.Point(215, 4);
            this.panelNavigator.Name = "panelNavigator";
            this.panelNavigator.Size = new System.Drawing.Size(320, 42);
            this.panelNavigator.TabIndex = 1;
            // 
            // panelAppCommands
            // 
            this.panelAppCommands.Location = new System.Drawing.Point(542, 4);
            this.panelAppCommands.Name = "panelAppCommands";
            this.panelAppCommands.Size = new System.Drawing.Size(94, 42);
            this.panelAppCommands.TabIndex = 2;
            // 
            // panelExitButton
            // 
            this.panelExitButton.Controls.Add(this.btnExit);
            this.panelExitButton.Location = new System.Drawing.Point(643, 4);
            this.panelExitButton.Name = "panelExitButton";
            this.panelExitButton.Size = new System.Drawing.Size(94, 42);
            this.panelExitButton.TabIndex = 3;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 500);
            this.Controls.Add(this.tableLayoutPanelButtons);
            this.Controls.Add(this.splitContainerAwesomium);
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
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.panelOptimizationButtons.ResumeLayout(false);
            this.panelExitButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerListOptimizationsRefresh;
        private System.Windows.Forms.Label labeOpt;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonMarkOpimization;
        private System.Windows.Forms.SplitContainer splitContainerAwesomium;
        private System.Windows.Forms.ListBox listBoxOptimizations;
        private System.Windows.Forms.Button buttonHide;
		private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button buttonOptRemove;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.Panel panelOptimizationButtons;
        private System.Windows.Forms.Panel panelNavigator;
        private System.Windows.Forms.Panel panelAppCommands;
        private System.Windows.Forms.Panel panelExitButton;
        private Awesomium.Windows.Forms.WebControl webControlDetails;
    }
}

