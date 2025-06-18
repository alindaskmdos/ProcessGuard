using ProcessGuard.Services;

namespace ProcessGuard.Core.Models
{
    public abstract class BlockTargetType
    {
        public string Identifier { get; set; }
        public BlockerTimer BlockerTimer { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BlockTargetType other && Identifier == other.Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier?.GetHashCode() ?? 0;
        }
    }

    public class BlockTargetProcess : BlockTargetType
    {
        public string ProcessName => Identifier;
    }

    public class BlockTargetSite : BlockTargetType
    {
        public string Domain => Identifier;
    }
}