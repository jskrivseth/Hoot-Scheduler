using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Data.Common;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;

namespace Scheduler
{
    public partial class Scheduler : Form
    {
        private static String _hootdFolder;
        #region Data members

        //_eventsList is the BindingList that the primary DataGridView binds to
        BindingList<ScheduledEvent> _eventsList;
        //_repeatEvents is a temporary storage to hold events to repeat
        List<ScheduledEvent> _repeatEvents;

        //_sourceSelection is used to persist the selected range of days during "Copy Events" operations
        SelectionRange _sourceSelection;
        //_repeatDestination is used to persist the select range of days during "Repeat" operations
        SelectionRange _repeatDestination;

        //eventsSerializer will be used to serialize and deserilize XML representations of the _eventsList
        XmlSerializer _eventsSerializer;

        #endregion

        #region Constructor and init methods

        public Scheduler()
        {
            InitializeComponent();
            SetupHootdProcess();
            SetupEventsData();
            fswAudioBrowser.Path = _hootdFolder;
            fswAudioBrowser.EnableRaisingEvents = true;
            SetupRecordingsData();
            Utilities.CreateMyDocumentsShortcut("Hootd", _hootdFolder);
            wmpPlayer.settings.autoStart = false;
        }

        private void SetupHootdProcess()
        {
            _hootdFolder = Application.StartupPath + "\\hootd";
            hootdProcess.StartInfo.FileName = "java";
            hootdProcess.StartInfo.Arguments = "-jar hootd.jar";
            hootdProcess.StartInfo.WorkingDirectory = _hootdFolder;
            hootdProcess.StartInfo.UseShellExecute = true;
        }

        // SetupEventsData()
        // --------------------------
        // Description: Initialize a BindingList, List, and XMLSerializer for the program
        // Begin binding the primary DataGridView to _eventsList
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void SetupEventsData()
        {
            _eventsList = new BindingList<ScheduledEvent>();
            _repeatEvents = new List<ScheduledEvent>();

            dgvScheduledEvents.AutoGenerateColumns = false;
            dgvScheduledEvents.DataSource = _eventsList;
            //define a new XmlSerializer and set the root attribute to "EventsList"
            _eventsSerializer = new XmlSerializer(typeof(BindingList<ScheduledEvent>), new XmlRootAttribute("EventsList"));
        }

        private void SetupRecordingsData()
        {
            dgvRecordingsList.AutoGenerateColumns = false;
            dgvRecordingsList.DataSource = Utilities.GetDocuments(_hootdFolder, "Recording*.wav");
        }

        #endregion

        #region Data utilities

        private void exportSchedule(String dstFile, Boolean confirm)
        {
            validateAudioFiles();
            foreach (DataGridViewRow dgvr in dgvScheduledEvents.Rows)
            {
                if (dgvr.Cells["AudioFile"].ErrorText == "File not found")
                {
                    if (MessageBox.Show("One or more play events had missing audio files.\nContinue?", "Missing audio files", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        break;
                    else
                        return;
                }
            }

            ArrayList schedulePackage = new ArrayList();
            try { File.Move("schedule.xml", "schedule.xml.bk"); }
            catch (System.IO.IOException) { }
            TextWriter textWriter = new StreamWriter(@"schedule.xml");
            _eventsSerializer.Serialize(textWriter, _eventsList);
            textWriter.Close();
            schedulePackage.Add("schedule.xml");

            using (WritingScheduleProgressDialog progressDialog = new WritingScheduleProgressDialog())
            {
                ProgressBar progressBar = (ProgressBar)progressDialog.Controls.Find("progressBar1", false)[0];
                progressDialog.StartPosition = FormStartPosition.Manual;
                progressDialog.Location = MousePosition;
                progressDialog.Show();
                foreach (ScheduledEvent thisEvent in _eventsList)
                {
                    if (!schedulePackage.Contains(thisEvent.AudioFileFullPath))
                    {
                        schedulePackage.Add(thisEvent.AudioFileFullPath);
                    }
                }

                string[] filenames = schedulePackage.ToArray(typeof(String)) as String[];
                if (confirm && File.Exists(dstFile))
                {
                    if (MessageBox.Show(dstFile + " already exists. Do you want to overwrite it?", "Overwrite existing schedule?", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
                compressFiles(filenames, dstFile, 0, progressBar);
                File.Delete("schedule.xml");
                try { File.Move("schedule.xml.bk", "schedule.xml"); }
                catch (System.IO.IOException) { }
                progressDialog.Close();
            }
        }

        private void CleanupOldSamples()
        {
            try
            {
                System.IO.DirectoryInfo samplesFolder = new System.IO.DirectoryInfo(_hootdFolder);
                FileInfo[] oldSamples = samplesFolder.GetFiles("*.wav");
                foreach (FileInfo oldSample in oldSamples)
                {
                    if (!oldSample.Name.StartsWith("Recording"))
                        oldSample.Delete();
                }
            }
            catch
            { }
        }

        // validateAudioFilesInTree(TreeNodeCollection)
        // --------------------------
        // Description: Recursively traverse the TreeView and make sure the audio file 
        // specified in a TreeNode's tag exists. If the file does not exist, mark the TreeNode red
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void validateAudioFilesInTree(TreeNodeCollection treeNodes)
        {
            for (int i = 0; i < treeNodes.Count; i++)
            {
                TreeNode treeNode = treeNodes[i];
                String filepath = treeNode.Tag.ToString();
                if (filepath != "Category")
                {
                    if (File.Exists(filepath))
                    {
                        treeNode.ForeColor = Color.Black;
                    }
                    else
                    {
                        treeNode.ForeColor = Color.Red;
                        treeNode.EnsureVisible();
                    }
                }
                if (treeNode.Nodes.Count > 0)
                {
                    validateAudioFilesInTree(treeNode.Nodes);
                }
            }
        }

        // validateAudioFilesInGridView()
        // --------------------------
        // Description: Loop through the rows of events in the GridView and verify that the audio file exists
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void validateAudioFilesInGridView()
        {
            foreach (DataGridViewRow dataRow in dgvScheduledEvents.Rows)
            {
                ScheduledEvent thisEvent = (ScheduledEvent)dataRow.DataBoundItem;
                if (!String.IsNullOrEmpty(thisEvent.AudioFileFullPath))
                {
                    if (File.Exists(thisEvent.AudioFileFullPath))
                    {
                        dataRow.ErrorText = "";
                    }
                    else
                    {
                        dataRow.ErrorText = "File not found";
                    }
                }
            }
        }

        // validateAudioFiles()
        // --------------------------
        // Description: Wrapper to call validateAudioFilesInGridView() and validateAudioFilesInTree(TreeNodeCollection)
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void validateAudioFiles()
        {
            validateAudioFilesInTree(tvAvailableAudioFiles.Nodes);
            validateAudioFilesInGridView();
        }

        // compressFiles(string[], string, int, ProgressBar)
        // --------------------------
        // Description: Finds the list of files, opens them and compresses them into a single output file
        // Reports progress to a ProgressBar
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        public void compressFiles(string[] filenames, string outputFileName, int compressionLevel, ProgressBar progress)
        {
            try
            {
                File.Delete(outputFileName);
            }
            catch
            {
                MessageBox.Show("Could not delete the old schedule.zip");
            }
            FileStream fsOut = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            using (ZipOutputStream s = new ZipOutputStream(fsOut))
            {
                s.SetLevel(compressionLevel); // 0-9, 9 being the highest compression
                s.UseZip64 = UseZip64.Dynamic; // Enable dynamic Zip64 support. Create a Zip32 archive for files < 4gb
                byte[] buffer = new byte[4096];
                progress.Maximum = filenames.Length;
                progress.Step = 1;
                foreach (String file in filenames)
                {
                    if (file != "" && File.Exists(file))
                    {
                        progress.PerformStep();
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;

                        s.PutNextEntry(entry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            buffer = new byte[fs.Length];
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            }
                            while (sourceBytes > 0);
                        }
                    }
                }
                s.Finish();
                s.Close();
            }
            fsOut.Close();
        }

        #endregion

        #region Overlapping events

        // findAnOverlappingEvent(ScheduledEvent)
        // --------------------------
        // Description: Compare this new event with every other event to see if they overlap
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private ScheduledEvent findAnOverlappingEvent(ScheduledEvent newEvent)
        {
            foreach (ScheduledEvent thisEvent in _eventsList)
            {
                // if our new row overlaps another row, return that row
                if (thisEvent.overlaps(newEvent))
                {
                    return thisEvent;
                }
            }
            // There may not be any rows in _eventsList, so check to see if this row overlaps itself
            if (newEvent.overlaps(newEvent))
            {
                return newEvent;
            }
            // No overlap was detected. Return null
            else
                return null;
        }

        #endregion

        #region Form calls

        // popupCalendar(DataGridView, DataGridViewCellEventArgs)
        // --------------------------
        // Description: Pops the DateTimePopup Form in the screen location 
        // where the user clicked and passes in the value of the 
        // existing Date/Time from the cell clicked by the user
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void popupCalendar(DataGridView dgv, DataGridViewCellEventArgs e)
        {
            using (DateTimePopup dtp = new DateTimePopup(dgv, e))
            {
                dtp.StartPosition = FormStartPosition.Manual;
                Point datePopupLocation = Scheduler.MousePosition;
                datePopupLocation.X -= 10;
                datePopupLocation.Y -= 10;
                dtp.Location = datePopupLocation;
                dtp.ShowDialog();
            }
        }
        #endregion

        #region Display selected events

        // displayAllEvents()
        // --------------------------
        // Description: Loop through all rows in the events list and set Visible = true
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void displayAllEvents()
        {
            foreach (DataGridViewRow dgvr in dgvScheduledEvents.Rows)
            {
                dgvr.Visible = true;
            }
        }

        // displaySelectedDateRangeEventsOnly(SelectionRange)
        // --------------------------
        // Description: Loop through all rows in the events list and set Visible = false if they do not 
        // have a date/time that occurs within the specified SelectionRange
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private void displaySelectedDateRangeEventsOnly(SelectionRange range)
        {
            // The selection range spans no time on the final day of the selection.
            // Say we chose 01/01 - 01/05, the DateTime objects in the SelectionRange
            // Go from 01/01/2010 00:00:00 - 01/05/2010 00:00:00, so they do not include 
            // any actual time from the final day. Add a day to it and make it span 
            // 01/01/2010 00:00:00 - 01/05/2010 23:59:59
            range.End = range.End.AddDays(1);

            //Suspend databinding on dgvScheduledEvents so we can hide rows that may be in various edit states.
            CurrencyManager cm = (CurrencyManager)BindingContext[dgvScheduledEvents.DataSource];
            cm.SuspendBinding();

            foreach (DataGridViewRow dgvr in dgvScheduledEvents.Rows)
            {
                if (eventOccursOnRange(dgvr, range))
                {
                    dgvr.Visible = true;
                }
                else
                {
                    dgvr.Visible = false;
                }
            }
            cm.ResumeBinding();
        }

        #endregion

        #region Event repeats

        // eventOccursOnRange(DataGridViewRow, SelectionRange)
        // --------------------------
        // Description: Gets the DataBoundItem (ScheduledEvent) for each DataRow. 
        // Determines if the ScheduledEvent occurs within the specified SelectionRange. 
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private bool eventOccursOnRange(DataGridViewRow dataRow, SelectionRange range)
        {
            ScheduledEvent theEvent = (ScheduledEvent)dataRow.DataBoundItem;

            return (theEvent.StartPlay >= range.Start && theEvent.StartPlay <= range.End ||
                     theEvent.StartRecord >= range.Start && theEvent.StartRecord <= range.End);
        }

        // repeatSingleDaysEvents(DateTime, DateTime)
        // --------------------------
        // Description: Repeats all events from one day to another. It does this by 
        // calling duplicateSingleDaysEvents(DateTime, List<T>). This clones the events
        // for a given day, giving them a new GUID and stores them in the specified List<T>.
        // We then move the ScheduledEvent to the new day and add it to the _eventsList. 
        // --------------------------
        // Calls to: ScheduledEvent
        // Called by: Scheduler
        // -------------------------- 
        private bool repeatSingleDaysEvents(DateTime srcDate, DateTime dstDate)
        {
            //Clear the "clipboard" list of events in DataSet _repeatEvents
            _repeatEvents.Clear();
            //Fill the "clipboard" with a single day's events
            duplicateSingleDaysEvents(srcDate, _repeatEvents);

            Boolean overlapsOccurred = false;

            //For each event found for the day
            foreach (ScheduledEvent thisEvent in _repeatEvents)
            {
                //Move the event to the desired destintation day
                thisEvent.moveEvent(dstDate);

                //Search for any overlapping event. If an overlap occurs, don't add the row. 
                if (findAnOverlappingEvent(thisEvent) == null)
                {
                    //add the row to the evenstList
                    _eventsList.Add(thisEvent);
                }
                else
                {
                    overlapsOccurred = true;
                }
            }

            if (overlapsOccurred)
            {
                return false;
            }
            return true;
        }

        // duplicateSingleDaysEvents(DateTime, List<ScheduledEvent>)
        // --------------------------
        // Description: Duplicates all ScheduledEvents that occur on a specific day. 
        // These newly created cloned ScheduledEvents are identical to the source
        // event, but with a new GUID. This list of ScheduledEvents are stored in the
        // specified List<ScheduledEvent>
        // --------------------------
        // Calls to: ScheduledEvent
        // Called by: Scheduler
        // -------------------------- 
        private void duplicateSingleDaysEvents(DateTime day, List<ScheduledEvent> listToFill)
        {
            foreach (ScheduledEvent thisEvent in _eventsList)
            {
                if (thisEvent.occursOnDay(day))
                {
                    ScheduledEvent clonedEvent = thisEvent.cloneEvent();
                    listToFill.Add(clonedEvent);
                }
            }
        }

        #endregion

        #region Event handlers

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutHoot aboutBox = new AboutHoot())
            {
                aboutBox.ShowDialog();
            }
        }

        private void chkBoxRecordEnabled_CheckedChanged(object sender, EventArgs e)
        {
            lblCreateEventRecordDuration.Enabled = chkBoxRecordEnabled.Checked;
            dtpCreateEventStartRecord.Enabled = chkBoxRecordEnabled.Checked;
            txtCreateEventRecordDuration.Enabled = chkBoxRecordEnabled.Checked;

        }

        private void chkBoxPlayEnabled_CheckedChanged(object sender, EventArgs e)
        {
            lblCreateEventPlayDuration.Enabled = chkBoxPlayEnabled.Checked;
            dtpCreateEventStartPlay.Enabled = chkBoxPlayEnabled.Checked;
            txtCreateEventPlayDuration.Enabled = chkBoxPlayEnabled.Checked;
        }

        // btnCreateEvent_Click(object, EventArgs)
        // --------------------------
        // Description: Creates a new ScheduledEvent, sets its data members equal to the 
        // user-supplied values, and adds the new ScheduledEvent to the _eventsList
        // --------------------------
        // Calls to: ScheduledEvent
        // Called by: Self
        // -------------------------- 
        private void btnCreateEvent_Click(object sender, EventArgs e)
        {
            if (chkBoxPlayEnabled.Checked || chkBoxRecordEnabled.Checked)
            {
                ScheduledEvent newEvent = new ScheduledEvent();
                //if both play and record are enabled
                if (chkBoxPlayEnabled.Checked)
                {
                    if (tvAvailableAudioFiles.SelectedNode != null)
                    {
                        if (tvAvailableAudioFiles.SelectedNode.Tag.ToString() != "Category")
                        {
                            try { newEvent.StartPlay = Convert.ToDateTime(dtpCreateEventStartPlay.Text); }
                            catch { }
                            newEvent.AudioFile = tvAvailableAudioFiles.SelectedNode.Text;
                            newEvent.AudioFileFullPath = tvAvailableAudioFiles.SelectedNode.Tag.ToString();
                            try { newEvent.PlayDuration = Convert.ToInt32(txtCreateEventPlayDuration.Text); }
                            catch { }
                        }
                        else
                        {
                            MessageBox.Show("ERROR: You selected a category instead of audio file.\n Please select an audio file first.");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("ERROR: You must select an audio file to create a Play event");
                        return;
                    }
                }
                if (chkBoxRecordEnabled.Checked)
                {
                    if (txtCreateEventRecordDuration.Text == "")
                    {
                        MessageBox.Show("You must provide a record duration in order to create a record event");
                        return;
                    }
                    try { newEvent.StartRecord = Convert.ToDateTime(dtpCreateEventStartRecord.Text); }
                    catch { }
                    try { newEvent.RecordDuration = Convert.ToInt32(txtCreateEventRecordDuration.Text); }
                    catch { }
                }

                ScheduledEvent overlappingEvent = findAnOverlappingEvent(newEvent);
                if (overlappingEvent == null)
                {
                    _eventsList.Add(newEvent);
                }
                else if (overlappingEvent == newEvent)
                {
                    MessageBox.Show("This event overlaps itself and cannot be created");
                }
                else
                {
                    MessageBox.Show("The event overlaps event " + _eventsList.IndexOf(overlappingEvent) + " and cannot be created\n");
                }
            }
            else
            {
                MessageBox.Show("You must select play and/or record to create an event.");
            }
        }

        private void btnAddAudioFile_Click(object sender, EventArgs e)
        {
            if (tvAvailableAudioFiles.SelectedNode != null)
            {
                if (tvAvailableAudioFiles.SelectedNode.Tag.ToString() != "Category")
                {
                    tvAvailableAudioFiles.SelectedNode = tvAvailableAudioFiles.SelectedNode.Parent;
                }
                ofdAddAudioFile.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must create and select a category to add audio files.");
            }
        }

        // duplicateAudioFile(String)
        // --------------------------
        // Description: Searches the audio files tree for any matching audio file name. Return true if duplicate found
        // --------------------------
        // Calls to: duplicateTreeString(TreeNodeCollection, String, ref bool)
        // Called by: Scheduler
        // -------------------------- 
        private bool duplicateAudioFile(String newFile)
        {
            bool duplicateFound = false;
            duplicateTreeString(tvAvailableAudioFiles.Nodes, newFile, ref duplicateFound);
            return duplicateFound;
        }

        // duplicateTreeString(TreeNodeCollection, String, ref bool)
        // --------------------------
        // Description: Recursively searches the TreeNodeCollection searching for a node tag equal to String. 
        // If found, sets the referenced boolean to true passed as an arg. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void duplicateTreeString(TreeNodeCollection treeNodes, String value, ref bool duplicateFound)
        {
            for (int i = 0; i < treeNodes.Count; i++)
            {
                if (duplicateFound)
                    break;
                TreeNode treeNode = treeNodes[i];
                String filepath = treeNode.Tag.ToString();
                if (filepath != "Category")
                {
                    if (treeNode.Text == value)
                    {
                        duplicateFound = true;
                    }
                }
                if (treeNode.Nodes.Count > 0 && !duplicateFound)
                {
                    duplicateTreeString(treeNode.Nodes, value, ref duplicateFound);
                }
            }
        }

        // ofdAddAudioFile_FileOk(object, CancelEventArgs)
        // --------------------------
        // Description: 
        // Occurs after the user tries to add an audio file and the OpenFileDialog successfully validates the
        // audio file. We check to make sure there are duplicate filenames before adding the file. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void ofdAddAudioFile_FileOk(object sender, CancelEventArgs e)
        {
            if (tvAvailableAudioFiles.SelectedNode != null)
            {
                for (int i = 0; i < ofdAddAudioFile.FileNames.Length; i++)
                {
                    String filename = ofdAddAudioFile.SafeFileNames[i];
                    String fullpath = ofdAddAudioFile.FileNames[i];
                    if (!duplicateAudioFile(filename))
                    {
                        TreeNode newNode = tvAvailableAudioFiles.SelectedNode.Nodes.Add(filename);
                        newNode.Tag = fullpath;
                    }
                    else
                    {
                        MessageBox.Show("This file '" + filename + "' has the same name as a file already loaded. Please rename the file before adding it.");
                    }
                }
                tvAvailableAudioFiles.SelectedNode.Expand();
            }
            else
            {
                for (int i = 0; i < ofdAddAudioFile.FileNames.Length; i++)
                {
                    TreeNode newNode = tvAvailableAudioFiles.Nodes.Add(ofdAddAudioFile.SafeFileNames[i]);
                    newNode.Tag = ofdAddAudioFile.FileNames[i];
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                hootdProcess.Kill();
            }
            catch { }
            System.Environment.Exit(0);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        // btnCreateCategory_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // Occurs when the user clicks "New Category" near the audio files list. Causes us to pop a PromptBox
        // to get a value from the user, and posts that value as a new node in the TreeView
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void btnCreateCategory_Click(object sender, EventArgs e)
        {
            TreeNode node = tvAvailableAudioFiles.SelectedNode;
            if (node != null)
            {
                //If the user tried to create a "New Category", but an audio file was currently selected in the 
                //tree, let's select the parent category for this group in order to create a new category
                //under it
                if (System.IO.File.Exists(node.Tag.ToString()))
                {
                    tvAvailableAudioFiles.SelectedNode = node.Parent;
                }
            }

            //Decide where to popup the dialog by looking at the current mouse location
            Point popupLocation = new Point(MousePosition.X, MousePosition.Y - 50);

            //If they are adding a sub category, mention this fact in the PromptBox
            if (tvAvailableAudioFiles.SelectedNode != null)
            {
                using (PromptBox pb = new PromptBox("Please enter new " + tvAvailableAudioFiles.SelectedNode.Text + " sub category name.", tvAvailableAudioFiles, popupLocation))
                {
                    pb.ShowDialog();
                }
            }
            else
            {
                using (PromptBox pb = new PromptBox("Please enter category name.", tvAvailableAudioFiles, popupLocation))
                {
                    pb.ShowDialog();
                }
            }
        }

        // tvAvailableAudioFiles_NodeMouseDoubleClick(object, TreeNodeMouseClickEventArgs)
        // --------------------------
        // Description: 
        // Occurs when the user double clicks an item in the audio files TreeView. Plays the audio file
        // if this is an audio file. Otherwise, does nothing. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void tvAvailableAudioFiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvAvailableAudioFiles.SelectedNode != null)
            {
                if (System.IO.File.Exists(tvAvailableAudioFiles.SelectedNode.Tag.ToString()))
                {
                    wmpPlayer.URL = tvAvailableAudioFiles.SelectedNode.Tag.ToString();
                    wmpPlayer.Ctlcontrols.play();
                }
                else if (tvAvailableAudioFiles.SelectedNode.Tag.ToString() != "Category")
                {
                    MessageBox.Show("The audio file cannot be found to play. It was moved, renamed, or deleted.");
                    validateAudioFiles();
                }
            }
        }

        // tvAvailableAudioFiles_MouseClick(object, MouseEventArgs)
        // --------------------------
        // Description: 
        // Occurs everytime the user clicks in the list of audio files. Triggers validation of the files in the list
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void tvAvailableAudioFiles_MouseClick(object sender, MouseEventArgs e)
        {
            tvAvailableAudioFiles.SelectedNode = null;
            tvAvailableAudioFiles.Update();
        }

        // dgvScheduledEvents_CellContentDoubleClick(object, DataGridViewCellEventArgs)
        // --------------------------
        // Description: 
        // Occurs when the user double clicks on the content of a cell
        // Since this could result in a local change to the ScheduledEvent, we first saveState() on the 
        // ScheduledEvent in case we need to revert any changes that occur here. 
        //
        // If the user double clicked on the StartPlay field, popup a calendar (prepopulated with the existing value)
        // to allow the user to edit. If the user double clicked on the AudioFile column, try to update the event
        // with the currently selected audio file on this form (tvAvailableAudioFiles)
        // ... And so on
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void dgvScheduledEvents_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ScheduledEvent eventToModify = (ScheduledEvent)dgvScheduledEvents.CurrentRow.DataBoundItem;
            eventToModify.saveState();
            String columnName = dgvScheduledEvents.Columns[e.ColumnIndex].Name;
            switch (columnName)
            {
                case "StartPlay":
                    //If the user double clicked on the Start Play column
                    if (e.RowIndex > -1)
                    {
                        popupCalendar(dgvScheduledEvents, e);
                    }
                    break;
                case "AudioFile":
                    //If the user double clicked on the Audio file column
                    if (tvAvailableAudioFiles.SelectedNode != null)
                    {
                        if (tvAvailableAudioFiles.SelectedNode.Tag.ToString() != "Category")
                        {
                            ScheduledEvent selectedEvent = (ScheduledEvent)dgvScheduledEvents.Rows[e.RowIndex].DataBoundItem;
                            selectedEvent.AudioFile = tvAvailableAudioFiles.SelectedNode.Text;
                            selectedEvent.AudioFileFullPath = tvAvailableAudioFiles.SelectedNode.Tag.ToString();
                            if (File.Exists(selectedEvent.AudioFileFullPath))
                                dgvScheduledEvents.Rows[e.RowIndex].Cells["AudioFile"].ErrorText = "";
                            else
                                dgvScheduledEvents.Rows[e.RowIndex].Cells["AudioFile"].ErrorText = "File not found";
                        }
                    }
                    break;
                case "StartRecord":
                    //If the user double clicked on the Start Record column
                    if (e.RowIndex > -1)
                    {
                        popupCalendar(dgvScheduledEvents, e);
                    }
                    break;
            }
        }

        private void btnDeleteAudioFile_Click(object sender, EventArgs e)
        {
            if (tvAvailableAudioFiles.SelectedNode != null)
            {
                tvAvailableAudioFiles.SelectedNode.Remove();
            }
        }

        // sendScheduleToDesktopToolStripMenuItem_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // Occurs when the user chooses the menu item "Export Schedule to Desktop"
        // First, we validate the audio files in the TreeView to make sure they are valid. 
        // Next, we create a new list of files we want to add to our .ZIP file
        // We backup the current schedule.xml (because we need to write a new one for the export)
        // And save the current Schedule to XML
        // Next, we popup a ProgressDialog and begin adding files to the .ZIP. 
        // We compress them, replace our temporary schedule.xml with the original
        // Done. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void sendScheduleToDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            validateAudioFiles();
            string usersDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            exportSchedule(usersDesktop + "\\schedule.zip", true);
        }

        // openMyScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        // --------------------------
        // Description: 
        // Tries to open a saved schedule from the user's "My Documents/Scheduler" folder
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void openMyScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvAvailableAudioFiles.Nodes.Clear();
            _eventsList.Clear();
            string personalSchedulerDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Scheduler\\";
            try
            {
                TextReader textReader = new StreamReader(personalSchedulerDirectory + "schedule.xml");
                _eventsList = (BindingList<ScheduledEvent>)_eventsSerializer.Deserialize(textReader);
                textReader.Close();
                TreeViewSerializer.DeserializeTreeView(tvAvailableAudioFiles, personalSchedulerDirectory + "audiofiles.dat");
                dgvScheduledEvents.DataSource = _eventsList;
            }
            catch (System.IO.DirectoryNotFoundException directoryNotFoundException)
            {
                Directory.CreateDirectory(personalSchedulerDirectory);
                MessageBox.Show("No personal schedule found to open.");
            }
            catch (System.IO.FileNotFoundException fileNotFoundException)
            {
                MessageBox.Show("No personal schedule found to open.");
            }
            validateAudioFiles();
        }

        // openSharedScheduleToolStripMenuItem_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // Tries to open a saved schedule from the "ProgramData/Scheduler" folder
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void openSharedScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvAvailableAudioFiles.Nodes.Clear();
            _eventsList.Clear();
            string sharedSchedulerDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Scheduler\\";
            try
            {
                TextReader textReader = new StreamReader(sharedSchedulerDirectory + "schedule.xml");
                _eventsList = (BindingList<ScheduledEvent>)_eventsSerializer.Deserialize(textReader);
                textReader.Close();
                TreeViewSerializer.DeserializeTreeView(tvAvailableAudioFiles, sharedSchedulerDirectory + "audiofiles.dat");
                dgvScheduledEvents.DataSource = _eventsList;
            }
            catch (System.IO.DirectoryNotFoundException directoryNotFoundException)
            {
                Directory.CreateDirectory(sharedSchedulerDirectory);
                MessageBox.Show("No shared schedule found to open.");
            }
            catch (System.IO.FileNotFoundException fileNotFoundException)
            {
                MessageBox.Show("No shared schedule found to open.");
            }
            validateAudioFiles();
        }

        // saveMyScheduleToolStripMenuItem1_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // Tries to save the schedule to the user's "My Documents/Scheduler" folder
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void saveMyScheduleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            validateAudioFiles();
            string mySchedulerDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Scheduler\\";
            try
            {
                TextWriter textWriter = new StreamWriter(mySchedulerDirectory + "schedule.xml");
                _eventsSerializer.Serialize(textWriter, _eventsList);
                textWriter.Close();
                TreeViewSerializer.SerializeTreeView(tvAvailableAudioFiles, mySchedulerDirectory + "audiofiles.dat");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        // saveSharedScheduleToolStripMenuItem1_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // Tries to save the schedule to the "ProgramData/Scheduler" folder
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void saveSharedScheduleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            validateAudioFiles();
            string sharedSchedulerDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Scheduler\\";
            try
            {
                TextWriter textWriter = new StreamWriter(sharedSchedulerDirectory + "schedule.xml");
                _eventsSerializer.Serialize(textWriter, _eventsList);
                textWriter.Close();
                TreeViewSerializer.SerializeTreeView(tvAvailableAudioFiles, sharedSchedulerDirectory + "audiofiles.dat");
            }
            catch (System.IO.DirectoryNotFoundException directoryNotFoundException)
            {
                Directory.CreateDirectory(sharedSchedulerDirectory);
                TextWriter textWriter = new StreamWriter(sharedSchedulerDirectory + "schedule.xml");
                _eventsSerializer.Serialize(textWriter, _eventsList);
                textWriter.Close();
                TreeViewSerializer.SerializeTreeView(tvAvailableAudioFiles, sharedSchedulerDirectory + "audiofiles.dat");
            }
        }

        // mntCalendarDateBrowser_DateChanged(object, DateRangeEventArgs)
        // --------------------------
        // Description: 
        // Occurs whenever the selected range of days changes on the calendar control
        // If we are viewing only selected days, 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void mntCalendarDateBrowser_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (radBtnViewSelectedDateOnly.Checked)
            {
                displaySelectedDateRangeEventsOnly(mntCalendarDateBrowser.SelectionRange);
            }
        }

        private void radBtnViewSelectedDateOnly_CheckedChanged(object sender, EventArgs e)
        {
            displaySelectedDateRangeEventsOnly(mntCalendarDateBrowser.SelectionRange);
        }

        private void radBtnViewAllEvents_CheckedChanged(object sender, EventArgs e)
        {
            displayAllEvents();
        }

        // txtCreateEventPlayDuration_KeyPress(object, KeyPressEventArgs)
        // --------------------------
        // Description: 
        // Occurs whenever the user types a value int he PlayDuration field of the Create Event area. 
        // This causes anything other than a digit or control character to be ignored when typing here
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void txtCreateEventPlayDuration_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) ^ Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // txtCreateEventRecordDuration_KeyPress(object, KeyPressEventArgs)
        // --------------------------
        // Description: 
        // Occurs whenever the user types a value int he RecordDuration field of the Create Event area. 
        // This causes anything other than a digit or control character to be ignored when typing here
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void txtCreateEventRecordDuration_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) ^ Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvScheduledEvents_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) ^ Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // dgvScheduledEvents_EditingControlShowing(object, DataGridViewEditingControlShowingEventArgs)
        // --------------------------
        // Description: 
        // Occurs whenever the user tries to edit the PlayDuration or RecordDuration fields on the DataGridView
        // Attaches event handlers to listen for keystrokes. If the user types anything other than a digit, the
        // keypress event is ignored. 
        // We also save the state of the event before the change occurs in case we need to revert the change due
        // to an overlap.
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void dgvScheduledEvents_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ScheduledEvent eventToModify = (ScheduledEvent)dgvScheduledEvents.CurrentRow.DataBoundItem;
            eventToModify.saveState();
            int columnIndex = dgvScheduledEvents.CurrentCell.ColumnIndex;
            switch (dgvScheduledEvents.Columns[columnIndex].Name)
            {
                case "PlayDuration":
                    if (!String.IsNullOrEmpty(dgvScheduledEvents.CurrentRow.Cells["StartPlay"].Value.ToString()))
                    {
                        e.Control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextboxNumeric_KeyPress);
                    }
                    else
                    {
                        e.Control.Hide();
                    }
                    break;

                case "RecordDuration":
                    if (!String.IsNullOrEmpty(dgvScheduledEvents.CurrentRow.Cells["StartRecord"].Value.ToString()))
                    {
                        e.Control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextboxNumeric_KeyPress);
                    }
                    else
                    {
                        e.Control.Hide();
                    }
                    break;

                default:
                    break;
            }
        }

        // TextboxNumeric_KeyPress(object, KeyPressEventArgs)
        // --------------------------
        // Description: 
        // Called to handle keypress events. Handles validation of keypresses, 
        // and only allows numbers and Ctrl events (ctrl-c, ctrl-v) 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void TextboxNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) ^ Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // btnCopyEvents_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // When the user clicks "Copy Events", we save the range of days they selected to _sourceSelection
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void btnCopyEvents_Click(object sender, EventArgs e)
        {
            _sourceSelection = mntCalendarDateBrowser.SelectionRange;
        }

        // btnRepeatEvents_Click(object, EventArgs)
        // --------------------------
        // Description: 
        // When the user clicks "Repeat" to repeat a range of events they selected, 
        // we first validate the source and destination ranges are the same number of days. 
        // Then, for each day in the range, we copy events from the source day to it's equivalent
        // position in the destination sequence. If any overlaps occur, make note of this 
        // and notify the user that these events were not repeated.
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void btnRepeatEvents_Click(object sender, EventArgs e)
        {
            if (_sourceSelection != null)
            {
                _repeatDestination = mntCalendarDateBrowser.SelectionRange;
                int daysCopied = (_sourceSelection.End - _sourceSelection.Start).Days;
                int daysToRepeat = (mntCalendarDateBrowser.SelectionEnd - mntCalendarDateBrowser.SelectionStart).Days;
                String overlappingDays = "";

                if (daysCopied != daysToRepeat)
                {
                    MessageBox.Show("You selected " + (daysCopied + 1) + " days to copy, but only " + (daysToRepeat + 1) +
                                    " to repeat. Be sure to select the same number of days to copy/repeat.");
                    return;
                }
                DateTime dstDay = _repeatDestination.Start;
                for (DateTime srcDay = _sourceSelection.Start; srcDay <= _sourceSelection.End; srcDay = srcDay.AddDays(1))
                {
                    if (!repeatSingleDaysEvents(srcDay, dstDay))
                    {
                        overlappingDays += dstDay.Month + "/" + dstDay.Day + "\t";
                    }
                    dstDay = dstDay.AddDays(1);
                }
                mntCalendarDateBrowser.SelectionRange = _repeatDestination;
                if (!String.IsNullOrEmpty(overlappingDays))
                {
                    MessageBox.Show("Overlapping events were found on the following days:\n" + overlappingDays + "\n\nThese events were not repeated.");
                }
            }
            else
            {
                MessageBox.Show("You must select a range of events and copy them first.");
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfdSaveScheduleAs.ShowDialog();
        }

        private void sfdSaveScheduleAs_FileOk(object sender, CancelEventArgs e)
        {
            validateAudioFiles();
            TextWriter textWriter = new StreamWriter(sfdSaveScheduleAs.FileName);
            _eventsSerializer.Serialize(textWriter, _eventsList);
            textWriter.Close();
            TreeViewSerializer.SerializeTreeView(tvAvailableAudioFiles, sfdSaveScheduleAs.FileName + "_audiofiles.dat");
        }

        private void savedScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdOpenSavedSchedule.ShowDialog();
        }

        private void ofdOpenSavedSchedule_FileOk(object sender, CancelEventArgs e)
        {
            tvAvailableAudioFiles.Nodes.Clear();
            _eventsList.Clear();
            try
            {
                TextReader textReader = new StreamReader(ofdOpenSavedSchedule.FileName);
                _eventsList = (BindingList<ScheduledEvent>)_eventsSerializer.Deserialize(textReader);
                textReader.Close();
                TreeViewSerializer.DeserializeTreeView(tvAvailableAudioFiles, ofdOpenSavedSchedule.FileName + "_audiofiles.dat");
                dgvScheduledEvents.DataSource = _eventsList;
            }
            catch (FileNotFoundException fnfException)
            {
                MessageBox.Show(fnfException.Message);
            }
            validateAudioFiles();
        }

        // dgvScheduledEvents_RowPostPaint(object, DataGridViewRowPostPaintEventArgs)
        // --------------------------
        // Description: 
        // Occurs after each row is painted by the DataGridView. Here, we need to add the 
        // row number to the row "header" by drawing it manually, since the DataGridView
        // control does not offer this functionality. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void dgvScheduledEvents_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //this method overrides the DataGridView's RowPostPaint event 
            //in order to automatically draw numbers on the row header cells

            //store a string representation of the row number in 'strRowNumber'
            string strRowNumber = (e.RowIndex).ToString();

            Brush b = SystemBrushes.ControlText;
            SizeF size = e.Graphics.MeasureString(strRowNumber, this.Font);

            e.Graphics.DrawString(strRowNumber, this.Font, b, e.RowBounds.Location.X + 15, e.RowBounds.Location.Y + ((e.RowBounds.Height - size.Height) / 2));
        }

        // dgvScheduledEvents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        // --------------------------
        // Description: 
        // Occurs every time a cell value is actually changed in the DataGridView. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        private void dgvScheduledEvents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_eventsList != null)
            {
                String editedColumnName = dgvScheduledEvents.Columns[e.ColumnIndex].Name;
                ScheduledEvent modifiedEvent = (ScheduledEvent)dgvScheduledEvents.Rows[e.RowIndex].DataBoundItem;
                ScheduledEvent overlappingEvent = findAnOverlappingEvent(modifiedEvent);
                if (overlappingEvent != null)
                {
                    MessageBox.Show("This change would overlap event " + _eventsList.IndexOf(overlappingEvent) + "\n No change has been made");
                    modifiedEvent.restoreState();
                }
            }
        }

        private void dgvRecordingsList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (System.IO.File.Exists(dgvRecordingsList.CurrentRow.Cells["FullName"].Value.ToString()))
            {
                wmpAudioBrowser.URL = dgvRecordingsList.CurrentRow.Cells["FullName"].Value.ToString();
                wmpAudioBrowser.Ctlcontrols.play();
            }
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            SetupRecordingsData();
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            SetupRecordingsData();
        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            SetupRecordingsData();
        }

        private void fileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            SetupRecordingsData();
        }

        private void dgvRecordingsList_DragLeave(object sender, EventArgs e)
        {
            StringCollection paths = new StringCollection();
            paths.Add(dgvRecordingsList.CurrentRow.Cells["FullName"].Value.ToString());
            Clipboard.SetFileDropList(paths);
        }

        private void btnDeleteRecordings_Click(object sender, EventArgs e)
        {
            if (dgvRecordingsList.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("This will delete all selected recordings. Are you sure?", "Delete Recordings?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                foreach (DataGridViewRow recording in dgvRecordingsList.SelectedRows)
                {
                    String filename = "";
                    try
                    {
                        FileInfo audioFile = (FileInfo)recording.DataBoundItem;
                        filename = audioFile.Name;
                        if (audioFile.FullName == wmpAudioBrowser.URL)
                        {
                            wmpAudioBrowser.URL = null;
                        }
                        audioFile.Delete();
                    }
                    catch
                    {
                        MessageBox.Show("Error deleting file " + filename);
                    }
                }
            }
            else
                MessageBox.Show("You must select recording(s) first. Click on the tab next to each row to select the row.");
        }

        private void Scheduler_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                hootdProcess.Kill();
            }
            catch { }
        }

        private void btnCopyRecordings_Click(object sender, EventArgs e)
        {
            if (dgvRecordingsList.SelectedRows.Count > 0)
            {
                StringCollection paths = new StringCollection();
                foreach (DataGridViewRow selectedRow in dgvRecordingsList.SelectedRows)
                {
                    paths.Add(selectedRow.Cells["FullName"].Value.ToString());
                }
                if (paths.Count > 0)
                {
                    Clipboard.SetFileDropList(paths);
                }
            }
            else
                MessageBox.Show("You must select recording(s) first. Click on the tab next to each row to select the row.");

        }

        private void chkBoxProcessSchedule_CheckedChanged(object sender, EventArgs e)
        {
            CleanupOldSamples();
            if (chkBoxProcessSchedule.Checked)
            {
                foreach (ScheduledEvent thisEvent in _eventsList)
                {
                    thisEvent.startProcessing();
                }
                dgvScheduledEvents.Enabled = false;
                dgvScheduledEvents.ForeColor = Color.Gray;
                btnCreateEvent.Enabled = false;
                btnRepeatEvents.Enabled = false;
                settingsToolStripMenuItem.Enabled = false;
                chkBoxProcessSchedule.Text = "Running...";
                exportSchedule(_hootdFolder + "\\schedule.zip", false);
                if (Properties.Settings.Default.displayHootd)
                {
                    hootdProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                }
                else
                {
                    hootdProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                try
                {
                    hootdProcess.Start();
                }
                catch
                {
                    chkBoxProcessSchedule.Checked = false;
                    MessageBox.Show("Could not start Java. Please verify that Java is installed.");
                }
            }
            else
            {
                foreach (ScheduledEvent thisEvent in _eventsList)
                {
                    thisEvent.stopProcessing();
                }
                dgvScheduledEvents.Enabled = true;
                btnCreateEvent.Enabled = true;
                btnRepeatEvents.Enabled = true;
                settingsToolStripMenuItem.Enabled = true;
                dgvScheduledEvents.ForeColor = Color.Black;
                chkBoxProcessSchedule.Text = "Stopped...";
                try
                {
                    hootdProcess.Kill();
                }
                catch
                { }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ControlPanel cp = new ControlPanel())
            {
                cp.ShowDialog();
            }
            
        }

        private void wmpPlayer_OpenStateChange(object sender, AxWMPLib._WMPOCXEvents_OpenStateChangeEvent e)
        {
            if (wmpPlayer.currentMedia.duration > 0)
            {
                int duration = (int)Math.Ceiling(wmpPlayer.currentMedia.duration);
                txtCreateEventPlayDuration.Text = duration.ToString();
            }
        }

        private void tvAvailableAudioFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvAvailableAudioFiles.SelectedNode.Tag.ToString() != "Category")
            {
                wmpPlayer.URL = tvAvailableAudioFiles.SelectedNode.Tag.ToString();
            }
        }

        private void hootdProcess_Exited(object sender, EventArgs e)
        {
            chkBoxProcessSchedule.Checked = false;
            dgvScheduledEvents.Enabled = true;
            btnCreateEvent.Enabled = true;
            btnRepeatEvents.Enabled = true;
            dgvScheduledEvents.ForeColor = Color.Black;
        }
        #endregion
    }
}
