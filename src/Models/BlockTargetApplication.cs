using ProcessGuard.Services;

namespace ProcessGuard.Models
{
    public class BlockTargetApplication
    {
        public bool IsEnabled { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public bool IsBlockedPermanently { get; set; }
        public BlockerTimer? BlockTimer { get; set; } = null;
    }
}