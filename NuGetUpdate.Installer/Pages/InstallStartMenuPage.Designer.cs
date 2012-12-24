namespace NuGetUpdate.Installer.Pages
{
    partial class InstallStartMenuPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._header = new NuGetUpdate.Shared.FormHeader();
            this.formFlowFooter1 = new NuGetUpdate.Shared.FormFlowFooter();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._container = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this._startMenuFolder = new System.Windows.Forms.TextBox();
            this._startMenuFolders = new System.Windows.Forms.ListBox();
            this._createStartMenu = new System.Windows.Forms.CheckBox();
            this._createOnDesktop = new System.Windows.Forms.CheckBox();
            this.formFlowFooter1.SuspendLayout();
            this.panel1.SuspendLayout();
            this._container.SuspendLayout();
            this.SuspendLayout();
            // 
            // _header
            // 
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(419, 47);
            this._header.SubText = "Choose a Start Menu folder for the {0} shortcuts.";
            this._header.TabIndex = 0;
            this._header.Text = "Choose Start Menu Folder";
            // 
            // formFlowFooter1
            // 
            this.formFlowFooter1.Controls.Add(this._cancelButton);
            this.formFlowFooter1.Controls.Add(this._acceptButton);
            this.formFlowFooter1.Location = new System.Drawing.Point(0, 325);
            this.formFlowFooter1.Name = "formFlowFooter1";
            this.formFlowFooter1.Size = new System.Drawing.Size(419, 53);
            this.formFlowFooter1.TabIndex = 2;
            // 
            // _cancelButton
            // 
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Location = new System.Drawing.Point(332, 20);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._acceptButton.Location = new System.Drawing.Point(251, 20);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "<<next>>";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._container);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 47);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12);
            this.panel1.Size = new System.Drawing.Size(419, 278);
            this.panel1.TabIndex = 1;
            // 
            // _container
            // 
            this._container.ColumnCount = 1;
            this._container.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._container.Controls.Add(this.label1, 0, 0);
            this._container.Controls.Add(this._startMenuFolder, 0, 1);
            this._container.Controls.Add(this._startMenuFolders, 0, 2);
            this._container.Controls.Add(this._createStartMenu, 0, 3);
            this._container.Controls.Add(this._createOnDesktop, 0, 4);
            this._container.Dock = System.Windows.Forms.DockStyle.Fill;
            this._container.Location = new System.Drawing.Point(12, 12);
            this._container.Name = "_container";
            this._container.RowCount = 5;
            this._container.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._container.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._container.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._container.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._container.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._container.Size = new System.Drawing.Size(395, 254);
            this._container.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(365, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select the Start Menu folder in which you would like to create the program\'s shor" +
    "tcut. You can also enter a name to create a new folder.";
            // 
            // _startMenuFolder
            // 
            this._startMenuFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this._startMenuFolder.Location = new System.Drawing.Point(3, 47);
            this._startMenuFolder.Name = "_startMenuFolder";
            this._startMenuFolder.Size = new System.Drawing.Size(389, 20);
            this._startMenuFolder.TabIndex = 1;
            // 
            // _startMenuFolders
            // 
            this._startMenuFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this._startMenuFolders.FormattingEnabled = true;
            this._startMenuFolders.IntegralHeight = false;
            this._startMenuFolders.Location = new System.Drawing.Point(3, 73);
            this._startMenuFolders.Name = "_startMenuFolders";
            this._startMenuFolders.Size = new System.Drawing.Size(389, 132);
            this._startMenuFolders.TabIndex = 2;
            this._startMenuFolders.SelectedIndexChanged += new System.EventHandler(this._startMenuFolders_SelectedIndexChanged);
            // 
            // _createStartMenu
            // 
            this._createStartMenu.AutoSize = true;
            this._createStartMenu.Location = new System.Drawing.Point(3, 211);
            this._createStartMenu.Name = "_createStartMenu";
            this._createStartMenu.Size = new System.Drawing.Size(158, 17);
            this._createStartMenu.TabIndex = 3;
            this._createStartMenu.Text = "Create Start Menu shortcuts";
            this._createStartMenu.UseVisualStyleBackColor = true;
            // 
            // _createOnDesktop
            // 
            this._createOnDesktop.AutoSize = true;
            this._createOnDesktop.Location = new System.Drawing.Point(3, 234);
            this._createOnDesktop.Name = "_createOnDesktop";
            this._createOnDesktop.Size = new System.Drawing.Size(174, 17);
            this._createOnDesktop.TabIndex = 4;
            this._createOnDesktop.Text = "Create shortcut on the Desktop";
            this._createOnDesktop.UseVisualStyleBackColor = true;
            // 
            // InstallStartMenuPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.formFlowFooter1);
            this.Controls.Add(this._header);
            this.Name = "InstallStartMenuPage";
            this.Size = new System.Drawing.Size(419, 378);
            this.formFlowFooter1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this._container.ResumeLayout(false);
            this._container.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Shared.FormHeader _header;
        private Shared.FormFlowFooter formFlowFooter1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel _container;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _startMenuFolder;
        private System.Windows.Forms.ListBox _startMenuFolders;
        private System.Windows.Forms.CheckBox _createStartMenu;
        private System.Windows.Forms.CheckBox _createOnDesktop;
    }
}
