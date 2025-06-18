using ProcessGuard.Services;

namespace ProcessGuard.Models
{
    public class BlockTargetSite
    {
        public bool IsEnabled { get; set; }
        public string Domain { get; set; } = string.Empty;
        public bool IsBlockedPermanently { get; set; }
        public BlockerTimer? BlockTimer { get; set; } = null;
    }
}