using System;

namespace SharpIM.Client
{
    public class BuddyInfo
    {
        public BuddyInfo()
        {
        }

        public BuddyInfo(string buddy)
        {
            Name = buddy;
            Status = BuddyStatus.Offline;
            SignOnTime = DateTime.MinValue;
        }

        public string Name { get; set; }

        public BuddyStatus Status { get; set; }

        public DateTime SignOnTime { get; set; }
    }
}