namespace Scheduler
{
    partial class ControlPanel
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
            this.txtBoxPreEventWakeSeconds = new System.Windows.Forms.TextBox();
            this.gbWakeSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDefaults = new System.Windows.Forms.Button();
            this.gbHootDaemon = new System.Windows.Forms.GroupBox();
            this.chkBoxDisplayHootProcess = new System.Windows.Forms.CheckBox();
            this.gbWakeSettings.SuspendLayout();
            this.gbHootDaemon.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtBoxPreEventWakeSeconds
            // 
            this.txtBoxPreEventWakeSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxPreEventWakeSeconds.Location = new System.Drawing.Point(166, 19);
            this.txtBoxPreEventWakeSeconds.Name = "txtBoxPreEventWakeSeconds";
            this.txtBoxPreEventWakeSeconds.Size = new System.Drawing.Size(94, 20);
            this.txtBoxPreEventWakeSeconds.TabIndex = 0;
            // 
            // gbWakeSettings
            // 
            this.gbWakeSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWakeSettings.Controls.Add(this.label1);
            this.gbWakeSettings.Controls.Add(this.txtBoxPreEventWakeSeconds);
            this.gbWakeSettings.Location = new System.Drawing.Point(9, 5);
            this.gbWakeSettings.Name = "gbWakeSettings";
            this.gbWakeSettings.Size = new System.Drawing.Size(266, 52);
            this.gbWakeSettings.TabIndex = 2;
            this.gbWakeSettings.TabStop = false;
            this.gbWakeSettings.Text = "Wake Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pre-event wake (seconds)";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSave.Location = new System.Drawing.Point(104, 114);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(199, 114);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDefaults
            // 
            this.btnDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDefaults.Location = new System.Drawing.Point(8, 114);
            this.btnDefaults.Name = "btnDefaults";
            this.btnDefaults.Size = new System.Drawing.Size(75, 23);
            this.btnDefaults.TabIndex = 4;
            this.btnDefaults.Text = "Defaults";
            this.btnDefaults.UseVisualStyleBackColor = true;
            this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
            // 
            // gbHootDaemon
            // 
            this.gbHootDaemon.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbHootDaemon.Controls.Add(this.chkBoxDisplayHootProcess);
            this.gbHootDaemon.Location = new System.Drawing.Point(12, 63);
            this.gbHootDaemon.Name = "gbHootDaemon";
            this.gbHootDaemon.Size = new System.Drawing.Size(263, 45);
            this.gbHootDaemon.TabIndex = 5;
            this.gbHootDaemon.TabStop = false;
            this.gbHootDaemon.Text = "Schedule Processor";
            // 
            // chkBoxDisplayHootProcess
            // 
            this.chkBoxDisplayHootProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxDisplayHootProcess.AutoSize = true;
            this.chkBoxDisplayHootProcess.Location = new System.Drawing.Point(63, 19);
            this.chkBoxDisplayHootProcess.Name = "chkBoxDisplayHootProcess";
            this.chkBoxDisplayHootProcess.Size = new System.Drawing.Size(194, 17);
            this.chkBoxDisplayHootProcess.TabIndex = 0;
            this.chkBoxDisplayHootProcess.Text = "Display schedule processor window";
            this.chkBoxDisplayHootProcess.UseVisualStyleBackColor = true;
            // 
            // ControlPanel
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 149);
            this.ControlBox = false;
            this.Controls.Add(this.gbHootDaemon);
            this.Controls.Add(this.btnDefaults);
            this.Controls.Add(this.gbWakeSettings);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.MinimumSize = new System.Drawing.Size(300, 185);
            this.Name = "ControlPanel";
            this.Text = "Control Panel";
            this.gbWakeSettings.ResumeLayout(false);
            this.gbWakeSettings.PerformLayout();
            this.gbHootDaemon.ResumeLayout(false);
            this.gbHootDaemon.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxPreEventWakeSeconds;
        private System.Windows.Forms.GroupBox gbWakeSettings;
        private System.Windows.Forms.Button btnDefaults;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbHootDaemon;
        private System.Windows.Forms.CheckBox chkBoxDisplayHootProcess;
    }
}