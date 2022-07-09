namespace NuGetUpdate.Demo
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._resetVersion = new System.Windows.Forms.Button();
            this._checkForUpdates = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._startUpdate = new System.Windows.Forms.Button();
            this._startUpdateSilently = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _resetVersion
            // 
            this._resetVersion.Location = new System.Drawing.Point(56, 41);
            this._resetVersion.Name = "_resetVersion";
            this._resetVersion.Size = new System.Drawing.Size(160, 23);
            this._resetVersion.TabIndex = 0;
            this._resetVersion.Text = "Reset version";
            this._resetVersion.UseVisualStyleBackColor = true;
            this._resetVersion.Click += new System.EventHandler(this._resetVersion_Click);
            // 
            // _checkForUpdates
            // 
            this._checkForUpdates.Location = new System.Drawing.Point(56, 70);
            this._checkForUpdates.Name = "_checkForUpdates";
            this._checkForUpdates.Size = new System.Drawing.Size(160, 23);
            this._checkForUpdates.TabIndex = 1;
            this._checkForUpdates.Text = "Check for updates";
            this._checkForUpdates.UseVisualStyleBackColor = true;
            this._checkForUpdates.Click += new System.EventHandler(this._checkForUpdates_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._resetVersion, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._checkForUpdates, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._startUpdate, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._startUpdateSilently, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(272, 193);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // _startUpdate
            // 
            this._startUpdate.Location = new System.Drawing.Point(56, 99);
            this._startUpdate.Name = "_startUpdate";
            this._startUpdate.Size = new System.Drawing.Size(160, 23);
            this._startUpdate.TabIndex = 2;
            this._startUpdate.Text = "Start update";
            this._startUpdate.UseVisualStyleBackColor = true;
            this._startUpdate.Click += new System.EventHandler(this._startUpdate_Click);
            // 
            // _startUpdateSilently
            // 
            this._startUpdateSilently.Location = new System.Drawing.Point(56, 128);
            this._startUpdateSilently.Name = "_startUpdateSilently";
            this._startUpdateSilently.Size = new System.Drawing.Size(160, 23);
            this._startUpdateSilently.TabIndex = 2;
            this._startUpdateSilently.Text = "Start update silently";
            this._startUpdateSilently.UseVisualStyleBackColor = true;
            this._startUpdateSilently.Click += new System.EventHandler(this._startUpdateSilently_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 193);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "NuGet Update Demo";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _resetVersion;
        private System.Windows.Forms.Button _checkForUpdates;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button _startUpdate;
        private System.Windows.Forms.Button _startUpdateSilently;
    }
}

