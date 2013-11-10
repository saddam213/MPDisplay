using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MessageFramework.DataObjects
{
    public class APITVGuide
    {
        public APITVGuideMessageType MessageType { get; set; }
        public APITvGuideMessage TvGuideMessage { get; set; }
        public List<APIRecording> RecordingMessage { get; set; }
        public string GuideGroup { get; set; }
    }

    public enum APITVGuideMessageType
    {
        TvGuide,
        Recordings,
        TvGuideGroup
    }

    public class APITvGuideMessage
    {
        public int BatchId { get; set; }
        public int BatchNumber { get; set; }
        public int BatchCount { get; set; }
        public APIChannel Channel { get; set; }
    }


    public class APIChannel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public APIImage Logo { get; set; }
        public int SortOrder { get; set; }
        public bool IsRadio { get; set; }
        public List<string> Groups { get; set; }
        public List<APIProgram> Programs { get; set; }
    }

    public class APIProgram
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsScheduled { get; set; }
    }

    public class APIRecording
    {
        public int ChannelId { get; set; }
        public int ProgramId { get; set; }

        
    }
}
