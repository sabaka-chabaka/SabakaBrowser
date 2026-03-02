using System;

namespace SabakaBrowser
{
    public class HistoryItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime VisitTime { get; set; }
    }
}