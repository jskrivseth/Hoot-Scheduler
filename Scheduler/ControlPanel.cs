using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scheduler
{
    public partial class ControlPanel : Form
    {
        public ControlPanel()
        {
            InitializeComponent();
            loadSettings();
        }

        private void loadSettings()
        {
            this.txtBoxPreEventWakeSeconds.Text = Properties.Settings.Default.wakeBeforeEvent.ToString();
            this.chkBoxDisplayHootProcess.Checked = Properties.Settings.Default.displayHootd;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Int32 preeventWakeSeconds = 0;
            if (!Int32.TryParse(txtBoxPreEventWakeSeconds.Text, out preeventWakeSeconds))
            {
                MessageBox.Show("Could not save the value you entered. Please enter a valid positive integer.");
                return;
            }
            else
            {
                Properties.Settings.Default.wakeBeforeEvent = preeventWakeSeconds;
            }
            Properties.Settings.Default.displayHootd = chkBoxDisplayHootProcess.Checked;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnDefaults_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
