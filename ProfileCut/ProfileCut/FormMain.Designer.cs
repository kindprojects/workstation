namespace ProfileCut
{
    partial class FormMain
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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerOptimization = new System.Windows.Forms.SplitContainer();
            this.labeOpt = new System.Windows.Forms.Label();
            this.listBoxOptimizations = new System.Windows.Forms.ListBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.webControlDetails = new Awesomium.Windows.Forms.WebControl(this.components);
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.panelAppCommands = new System.Windows.Forms.Panel();
            this.panelNavigator = new System.Windows.Forms.Panel();
            this.panelOptButtons = new System.Windows.Forms.Panel();
            this.buttonHide = new System.Windows.Forms.Button();
            this.buttonOptRemove = new System.Windows.Forms.Button();
            this.buttonMarkOpimization = new System.Windows.Forms.Button();
            this.panelExitButton = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.timerListOptimizationsRefresh = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOptimization)).BeginInit();
            this.splitContainerOptimization.Panel1.SuspendLayout();
            this.splitContainerOptimization.Panel2.SuspendLayout();
            this.splitContainerOptimization.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.panelOptButtons.SuspendLayout();
            this.panelExitButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerOptimization);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tableLayoutPanelButtons);
            this.splitContainerMain.Size = new System.Drawing.Size(702, 497);
            this.splitContainerMain.SplitterDistance = 443;
            this.splitContainerMain.TabIndex = 0;
            // 
            // splitContainerOptimization
            // 
            this.splitContainerOptimization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerOptimization.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerOptimization.IsSplitterFixed = true;
            this.splitContainerOptimization.Location = new System.Drawing.Point(0, 0);
            this.splitContainerOptimization.Name = "splitContainerOptimization";
            // 
            // splitContainerOptimization.Panel1
            // 
            this.splitContainerOptimization.Panel1.Controls.Add(this.labeOpt);
            this.splitContainerOptimization.Panel1.Controls.Add(this.listBoxOptimizations);
            this.splitContainerOptimization.Panel1.Controls.Add(this.buttonRefresh);
            // 
            // splitContainerOptimization.Panel2
            // 
            this.splitContainerOptimization.Panel2.Controls.Add(this.webControlDetails);
            this.splitContainerOptimization.Size = new System.Drawing.Size(702, 443);
            this.splitContainerOptimization.SplitterDistance = 220;
            this.splitContainerOptimization.TabIndex = 0;
            // 
            // labeOpt
            // 
            this.labeOpt.AutoSize = true;
            this.labeOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labeOpt.Location = new System.Drawing.Point(3, 14);
            this.labeOpt.Name = "labeOpt";
            this.labeOpt.Size = new System.Drawing.Size(98, 16);
            this.labeOpt.TabIndex = 10;
            this.labeOpt.Text = "Оптимизации";
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
            this.listBoxOptimizations.Size = new System.Drawing.Size(208, 397);
            this.listBoxOptimizations.TabIndex = 9;
            this.listBoxOptimizations.SelectedIndexChanged += new System.EventHandler(this.listBoxOptimizations_SelectedIndexChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.BackgroundImage = global::ProfileCut.Resource.refresh2;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.Location = new System.Drawing.Point(174, 3);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(40, 38);
            this.buttonRefresh.TabIndex = 8;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // webControlDetails
            // 
            this.webControlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControlDetails.Location = new System.Drawing.Point(0, 0);
            this.webControlDetails.Size = new System.Drawing.Size(478, 443);
            this.webControlDetails.TabIndex = 1;
            this.webControlDetails.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            // 
            // tableLayoutPanelButtons
            // 
            this.tableLayoutPanelButtons.ColumnCount = 4;
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelButtons.Controls.Add(this.panelAppCommands, 2, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelNavigator, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelOptButtons, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.panelExitButton, 3, 0);
            this.tableLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelButtons.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            this.tableLayoutPanelButtons.RowCount = 1;
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.Size = new System.Drawing.Size(702, 50);
            this.tableLayoutPanelButtons.TabIndex = 0;
            // 
            // panelAppCommands
            // 
            this.panelAppCommands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAppCommands.Location = new System.Drawing.Point(505, 3);
            this.panelAppCommands.Name = "panelAppCommands";
            this.panelAppCommands.Size = new System.Drawing.Size(94, 44);
            this.panelAppCommands.TabIndex = 3;
            // 
            // panelNavigator
            // 
            this.panelNavigator.AutoSize = true;
            this.panelNavigator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNavigator.Location = new System.Drawing.Point(223, 3);
            this.panelNavigator.Name = "panelNavigator";
            this.panelNavigator.Size = new System.Drawing.Size(276, 44);
            this.panelNavigator.TabIndex = 2;
            // 
            // panelOptButtons
            // 
            this.panelOptButtons.Controls.Add(this.buttonHide);
            this.panelOptButtons.Controls.Add(this.buttonOptRemove);
            this.panelOptButtons.Controls.Add(this.buttonMarkOpimization);
            this.panelOptButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOptButtons.Location = new System.Drawing.Point(3, 3);
            this.panelOptButtons.Name = "panelOptButtons";
            this.panelOptButtons.Size = new System.Drawing.Size(214, 44);
            this.panelOptButtons.TabIndex = 0;
            // 
            // buttonHide
            // 
            this.buttonHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonHide.BackgroundImage = global::ProfileCut.Resource.arrows;
            this.buttonHide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHide.Location = new System.Drawing.Point(171, 3);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(40, 38);
            this.buttonHide.TabIndex = 16;
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
            // 
            // buttonOptRemove
            // 
            this.buttonOptRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOptRemove.BackgroundImage = global::ProfileCut.Resource.remove;
            this.buttonOptRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonOptRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOptRemove.Location = new System.Drawing.Point(49, 3);
            this.buttonOptRemove.Name = "buttonOptRemove";
            this.buttonOptRemove.Size = new System.Drawing.Size(40, 38);
            this.buttonOptRemove.TabIndex = 15;
            this.buttonOptRemove.UseVisualStyleBackColor = true;
            this.buttonOptRemove.Click += new System.EventHandler(this.buttonOptRemove_Click);
            // 
            // buttonMarkOpimization
            // 
            this.buttonMarkOpimization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMarkOpimization.BackgroundImage = global::ProfileCut.Resource.check;
            this.buttonMarkOpimization.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonMarkOpimization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMarkOpimization.Location = new System.Drawing.Point(3, 3);
            this.buttonMarkOpimization.Name = "buttonMarkOpimization";
            this.buttonMarkOpimization.Size = new System.Drawing.Size(40, 38);
            this.buttonMarkOpimization.TabIndex = 11;
            this.buttonMarkOpimization.UseVisualStyleBackColor = true;
            this.buttonMarkOpimization.Click += new System.EventHandler(this.buttonMarkOptimization_Click);
            // 
            // panelExitButton
            // 
            this.panelExitButton.Controls.Add(this.btnExit);
            this.panelExitButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelExitButton.Location = new System.Drawing.Point(605, 3);
            this.panelExitButton.Name = "panelExitButton";
            this.panelExitButton.Size = new System.Drawing.Size(94, 44);
            this.panelExitButton.TabIndex = 1;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(6, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(85, 38);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // timerListOptimizationsRefresh
            // 
            this.timerListOptimizationsRefresh.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 497);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "FormMain";
            this.Text = "FMain2";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerOptimization.Panel1.ResumeLayout(false);
            this.splitContainerOptimization.Panel1.PerformLayout();
            this.splitContainerOptimization.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOptimization)).EndInit();
            this.splitContainerOptimization.ResumeLayout(false);
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelButtons.PerformLayout();
            this.panelOptButtons.ResumeLayout(false);
            this.panelExitButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerOptimization;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.Panel panelOptButtons;
        private System.Windows.Forms.Button buttonMarkOpimization;
        private System.Windows.Forms.Button buttonOptRemove;
        private System.Windows.Forms.Button buttonHide;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ListBox listBoxOptimizations;
        private System.Windows.Forms.Panel panelExitButton;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelNavigator;
        private System.Windows.Forms.Panel panelAppCommands;
        private System.Windows.Forms.Label labeOpt;
        private Awesomium.Windows.Forms.WebControl webControlDetails;
        private System.Windows.Forms.Timer timerListOptimizationsRefresh;
    }
}