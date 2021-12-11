using System;

namespace UtilityFunctions.Data
{
    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never = 0,
        Yearly = 1,
        Monthly = 2,
        Weekly = 3,
        Daily = 4,
        Hourly = 5,
        Always = 6
    }
}
