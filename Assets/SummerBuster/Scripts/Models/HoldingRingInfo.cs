using SummerBuster.Gameplay;

namespace SummerBuster.Models
{
    public class HoldingRingInfo
    {
        public RingStack previousRingStack;
        public Ring ring;

        public HoldingRingInfo(RingStack previousRingStack, Ring ring)
        {
            this.previousRingStack = previousRingStack;
            this.ring = ring;
        }
    }
}