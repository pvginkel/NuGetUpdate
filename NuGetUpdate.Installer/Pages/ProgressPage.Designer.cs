namespace NuGetUpdate.Installer.Pages
{
    partial class ProgressPage
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._progressLabel = new NuGetUpdate.Shared.PathLabel();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._showDetails = new System.Windows.Forms.Button();
            this._progressListBox = new System.Windows.Forms.ListBox();
            this.formFlowFooter1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _header
            // 
            this._header.Location = new System.Drawing.Point(0, 0);
            this._header.Name = "_header";
            this._header.Size = new System.Drawing.Size(574, 47);
            this._header.SubText = "<<mode subtext>>";
            this._header.TabIndex = 0;
            this._header.Text = "<<mode>>";
            // 
            // formFlowFooter1
            // 
            this.formFlowFooter1.Controls.Add(this.button1);
            this.formFlowFooter1.Controls.Add(this.button2);
            this.formFlowFooter1.Location = new System.Drawing.Point(0, 403);
            this.formFlowFooter1.Name = "formFlowFooter1";
            this.formFlowFooter1.Size = new System.Drawing.Size(574, 53);
            this.formFlowFooter1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(487, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button2.Location = new System.Drawing.Point(406, 20);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Next >";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 47);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12);
            this.panel1.Size = new System.Drawing.Size(574, 356);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._progressLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._progressBar, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._showDetails, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._progressListBox, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(550, 332);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _progressLabel
            // 
            this._progressLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressLabel.Location = new System.Drawing.Point(3, 3);
            this._progressLabel.Name = "_progressLabel";
            this._progressLabel.Size = new System.Drawing.Size(544, 13);
            this._progressLabel.TabIndex = 0;
            // 
            // _progressBar
            // 
            this._progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressBar.Location = new System.Drawing.Point(3, 22);
            this._progressBar.Maximum = 1000;
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(544, 18);
            this._progressBar.TabIndex = 1;
            // 
            // _showDetails
            // 
            this._showDetails.Location = new System.Drawing.Point(3, 46);
            this._showDetails.Name = "_showDetails";
            this._showDetails.Size = new System.Drawing.Size(98, 23);
            this._showDetails.TabIndex = 2;
            this._showDetails.Text = "&Show details";
            this._showDetails.UseVisualStyleBackColor = true;
            this._showDetails.Click += new System.EventHandler(this._showDetails_Click);
            // 
            // _progressListBox
            // 
            this._progressListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._progressListBox.FormattingEnabled = true;
            this._progressListBox.IntegralHeight = false;
            this._progressListBox.Location = new System.Drawing.Point(3, 75);
            this._progressListBox.Name = "_progressListBox";
            this._progressListBox.Size = new System.Drawing.Size(544, 254);
            this._progressListBox.TabIndex = 3;
            // 
            // ProgressPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.formFlowFooter1);
            this.Controls.Add(this._header);
            this.Name = "ProgressPage";
            this.Size = new System.Drawing.Size(574, 456);
            this.ParentChanged += new System.EventHandler(this.ProgressPage_ParentChanged);
            this.formFlowFooter1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Shared.FormHeader _header;
        private Shared.FormFlowFooter formFlowFooter1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private NuGetUpdate.Shared.PathLabel _progressLabel;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _showDetails;
        private System.Windows.Forms.ListBox _progressListBox;
    }
}
