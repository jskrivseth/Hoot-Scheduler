using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Threading;


namespace Scheduler
{
    [XmlRoot("ScheduledEvent")]
    public class ScheduledEvent : INotifyPropertyChanged
    {
        private const String dateFormatString = "yyyy-MM-dd HH:mm:ss";

        private DateTime startPlay;
        private DateTime startRecord;
        private int playDuration;
        private int recordDuration;
        private String audioFile;
        private String audioFileFullPath;
        private String guid;

        private ScheduledEvent previousState;

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        public DateTime StartPlay
        {
            get
            { return startPlay; }
            set
            {
                DateTime.TryParse(value.ToString(), out startPlay);
                this.NotifyPropertyChanged("StartPlay");
            }
        }

        [XmlElement("StartPlay")]
        public String StartPlayString
        {
            get
            {
                if (this.startPlay == DateTime.MinValue)
                {
                    return "";
                }
                else
                    return String.Format("{0:" + dateFormatString + "}", this.StartPlay);
            }
            set
            {
                DateTime.TryParse(value.ToString(), out startPlay);
                this.NotifyPropertyChanged("StartPlayString");
            }
        }

        [XmlIgnore]
        public DateTime StartRecord
        {
            get
            { return startRecord; }
            set
            {
                DateTime.TryParse(value.ToString(), out startRecord);
                this.NotifyPropertyChanged("StartRecord");
            }
        }

        [XmlElement("StartRecord")]
        public String StartRecordString
        {
            get
            {
                if (this.startRecord == DateTime.MinValue)
                {
                    return "";
                }
                else
                    return String.Format("{0:" + dateFormatString + "}", this.startRecord);
            }
            set
            {
                DateTime.TryParse(value.ToString(), out startRecord);
                this.NotifyPropertyChanged("StartRecordString");
            }
        }

        [XmlIgnore]
        public int PlayDuration
        {
            get
            { return playDuration; }
            set
            { Int32.TryParse(value.ToString(), out playDuration); this.NotifyPropertyChanged("PlayDuration"); }
        }

        [XmlElement("PlayDuration")]
        public String PlayDurationString
        {
            get
            {
                if (this.playDuration == 0)
                {
                    return "";
                }
                else
                    return this.playDuration.ToString();
            }
            set
            {
                Int32.TryParse(value.ToString(), out this.playDuration); this.NotifyPropertyChanged("PlayDurationString");
            }
        }

        public DateTime StopPlay
        {
            get
            { return startPlay.AddSeconds(playDuration); }
        }

        [XmlIgnore]
        public int RecordDuration
        {
            get
            { return recordDuration; }
            set
            { Int32.TryParse(value.ToString(), out recordDuration); this.NotifyPropertyChanged("RecordDuration"); }
        }

        [XmlElement("RecordDuration")]
        public String RecordDurationString
        {
            get
            {
                if (this.recordDuration == 0)
                {
                    return "";
                }
                else
                    return this.recordDuration.ToString();
            }
            set
            {
                Int32.TryParse(value.ToString(), out this.recordDuration); this.NotifyPropertyChanged("RecordDurationString");
            }
        }

        public DateTime StopRecord
        {
            get
            { return startRecord.AddSeconds(recordDuration); }
        }

        public String AudioFile
        {
            get
            { return audioFile; }
            set
            { audioFile = value; this.NotifyPropertyChanged("AudioFile"); }
        }

        public String AudioFileFullPath
        {
            get
            { return audioFileFullPath; }
            set
            { audioFileFullPath = value; this.NotifyPropertyChanged("AudioFileFullPath"); }
        }

        public String GUID
        {
            get
            { return guid; }
            set
            { guid = value; this.NotifyPropertyChanged("GUID"); }
        }

        public ScheduledEvent()
        {
            startPlay = new DateTime();
            startRecord = new DateTime();
            audioFile = "";
            audioFileFullPath = "";
            playDuration = 0;
            recordDuration = 0;
            GUID = Guid.NewGuid().ToString();
            previousState = (ScheduledEvent)this.MemberwiseClone();
        }

        public ScheduledEvent(DataRow row)
        {
            startPlay = new DateTime();
            startRecord = new DateTime();
            DateTime.TryParse(row.Field<String>("StartPlay"), out startPlay);
            audioFile = row.Field<String>("AudioFile");
            audioFileFullPath = row.Field<String>("AudioFileFullPath");
            DateTime.TryParse(row.Field<String>("StartRecord"), out startRecord);
            Int32.TryParse(row.Field<String>("PlayDuration"), out playDuration);
            Int32.TryParse(row.Field<String>("RecordDuration"), out recordDuration);
            GUID = row.Field<String>("GUID");
            previousState = (ScheduledEvent)this.MemberwiseClone();
        }

        public ScheduledEvent(DataGridViewRow row)
        {
            startPlay = new DateTime();
            startRecord = new DateTime();
            DateTime.TryParse(row.Cells["StartPlay"].Value.ToString(), out startPlay);
            audioFile = row.Cells["AudioFile"].FormattedValue.ToString();
            audioFileFullPath = row.Cells["AudioFileFullPath"].Value.ToString();
            DateTime.TryParse(row.Cells["StartRecord"].Value.ToString(), out startRecord);
            Int32.TryParse(row.Cells["PlayDuration"].Value.ToString(), out playDuration);
            Int32.TryParse(row.Cells["RecordDuration"].Value.ToString(), out recordDuration);
            GUID = row.Cells["GUID"].Value.ToString();
            previousState = (ScheduledEvent)this.MemberwiseClone();
        }

        // saveState()
        // --------------------------
        // Description: Duplicates this event as a new object in order to save 
        // the current state. 
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        public void saveState()
        {
            previousState = (ScheduledEvent)this.MemberwiseClone();
        }

        // restoreState()
        // --------------------------
        // Description: Restores the state of the object to the 
        // previously stored state
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        public void restoreState()
        {
            this.StartPlay = previousState.StartPlay;
            this.StartRecord = previousState.StartRecord;
            this.AudioFile = previousState.AudioFile;
            this.AudioFileFullPath = previousState.AudioFileFullPath;
            this.PlayDuration = previousState.PlayDuration;
            this.RecordDuration = previousState.RecordDuration;
            this.GUID = previousState.GUID;
        }

        // cloneEvent()
        // --------------------------
        // Description: Duplicates this event as a new object. 
        // Sets the GUID of the clone to some new GUID
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        public ScheduledEvent cloneEvent()
        {
            ScheduledEvent newEvent = (ScheduledEvent)this.MemberwiseClone();
            newEvent.GUID = Guid.NewGuid().ToString();
            return newEvent;
        }

        // overlaps(ScheduledEvent)
        // --------------------------
        // Description: Determines if this event overlaps another
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        public bool overlaps(ScheduledEvent compareEvent)
        {
            /* determine if play or record overlaps */
            bool playOverlaps = false;
            bool recordOverlaps = false;
            bool playOverlapsRecord = false;
            bool recordOverlapsPlay = false;
            bool overlapsItself = false;

            if (this.GUID != compareEvent.GUID)
            {
                playOverlapsRecord = dateTimeOverlaps(this.StartPlay, this.StopPlay, compareEvent.StartRecord, compareEvent.StopRecord);
                recordOverlapsPlay = dateTimeOverlaps(this.StartRecord, this.StopRecord, compareEvent.StartPlay, compareEvent.StopPlay);
                playOverlaps = dateTimeOverlaps(this.StartPlay, this.StopPlay, compareEvent.StartPlay, compareEvent.StopPlay);
                recordOverlaps = dateTimeOverlaps(this.StartRecord, this.StopRecord, compareEvent.StartRecord, compareEvent.StopRecord);
            }
            else
            {
                overlapsItself = dateTimeOverlaps(this.StartPlay, this.StopPlay, this.StartRecord, this.StopRecord);
            }

            if (playOverlaps || recordOverlaps || playOverlapsRecord || recordOverlapsPlay || overlapsItself)
            {
                return true;
            }

            return false;
        }

        public Boolean occursOnDay(DateTime day)
        {
            if (this.StartPlay >= day && this.StartPlay < day.AddDays(1) ||
                 this.StartRecord >= day && this.StartRecord < day.AddDays(1))
            {
                return true;
            }
            return false;
        }

        public bool isRecordEvent()
        {
            if (this.StartRecord != DateTime.MinValue)
                return true;
            else
                return false;
        }

        public bool isPlayEvent()
        {
            if (this.StartPlay != DateTime.MinValue)
                return true;
            else
                return false;
        }

        // moveEvent(DateTime)
        // --------------------------
        // Description: Moves the event to the specified day. If both play and record
        // are valid values, the move will be relative to the play time
        // --------------------------
        // Calls to: Self
        // Called by: Scheduler
        // -------------------------- 
        public void moveEvent(DateTime dstDate)
        {
            int offset = 0;
            if (isPlayEvent() && isRecordEvent())
            {
                if (this.StartPlay <= this.StartRecord)
                {
                    offset = (int)Math.Ceiling((dstDate - this.StartPlay).TotalDays);
                }
                else
                {
                    offset = (int)Math.Ceiling((dstDate - this.StartRecord).TotalDays);
                }
                this.StartPlay = this.StartPlay.AddDays(offset);
                this.StartRecord = this.StartRecord.AddDays(offset);
            }
            else if (isPlayEvent())
            {
                offset = (int)Math.Ceiling((dstDate - this.StartPlay).TotalDays);
                this.StartPlay = this.StartPlay.AddDays(offset);
            }
            else if (isRecordEvent())
            {
                offset = (int)Math.Ceiling((dstDate - this.StartRecord).TotalDays);
                this.StartRecord = this.StartRecord.AddDays(offset);
            }
        }

        // dateTimeOverlaps(DateTime,DateTime,DateTime,DateTime)
        // --------------------------
        // Description: Determines if the specified time ranges overlap
        // --------------------------
        // Calls to: Self
        // Called by: Self
        // -------------------------- 
        private bool dateTimeOverlaps(DateTime srcStart, DateTime srcEnd, DateTime cmpStart, DateTime cmpEnd)
        {
            if (srcStart != new DateTime() && srcEnd != new DateTime() && cmpStart != new DateTime() && cmpEnd != new DateTime())
            {
                return (
                  (cmpStart >= srcStart && cmpStart < srcEnd) ||
                  (cmpEnd <= srcEnd && cmpEnd > srcStart) ||
                  (cmpStart <= srcStart && cmpEnd >= srcEnd)
                 );
            }
            else
            {
                return false;
            }
        }

        // startProcessing()
        // --------------------------
        // Description: Adds a wake timer to the ThreadPool if the event occurs in the future
        // --------------------------
        // Calls to: WakeUp
        // Called by: Scheduler
        // -------------------------- 
        public void startProcessing()
        {
            try
            {
                if (startPlay >= DateTime.Now)
                {
                    DateTime wakeTime = startPlay.AddSeconds(-Properties.Settings.Default.wakeBeforeEvent);
                    ThreadPool.QueueUserWorkItem(WakeUp.ThreadPoolCallback, wakeTime);
                }
            }
            catch { }
            try
            {
                if (startRecord >= DateTime.Now)
                {
                    DateTime wakeTime = startRecord.AddSeconds(-Properties.Settings.Default.wakeBeforeEvent);
                    ThreadPool.QueueUserWorkItem(WakeUp.ThreadPoolCallback, wakeTime);
                }
            }
            catch { }
        }

        // startProcessing()
        // --------------------------
        // Description: Removes a wake timer by asking it to cancel, if it exists.
        // Since this causes the thread to exit, the thread is cleared from the ThreadPool
        // --------------------------
        // Calls to: WakeUp
        // Called by: Scheduler
        // -------------------------- 
        public void stopProcessing()
        {
            try
            {
                if (startPlay != DateTime.MinValue)
                {
                    DateTime wakeTime = StartPlay.AddSeconds(-Properties.Settings.Default.wakeBeforeEvent);
                    WakeUp.CancelWakeUpTime(wakeTime);
                }
            }
            catch { }
            try
            {
                if (startRecord != DateTime.MinValue)
                {
                    DateTime wakeTime = StartRecord.AddSeconds(-Properties.Settings.Default.wakeBeforeEvent);
                    WakeUp.CancelWakeUpTime(wakeTime);
                }
            }
            catch { }
        }

        // NotifyPropertyChanged(string)
        // --------------------------
        // Description: Notifies a DataBinder that a property has changed
        // --------------------------
        // Calls to: Any DataBound object
        // Called by: Self
        // -------------------------- 
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

    }
}
