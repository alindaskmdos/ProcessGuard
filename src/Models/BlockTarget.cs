using System.Collections.Specialized;
using blocker.Services;

namespace blocker.Models
{
    public class BlockTargetSite
    {
        public bool IsEnabled { get; set; }

        public string Domain { get; set; } = string.Empty;


        public bool IsBlockedPermanently { get; set; }
        public BlockerTimer? BlockTimer { get; set; } = null;
    }

    public class BlockTargetApplication
    {
        public bool IsEnabled { get; set; }

        public string ProcessName { get; set; } = string.Empty;

        public bool IsBlockedPermanently { get; set; }
        public BlockerTimer? BlockTimer { get; set; } = null;
    }
}