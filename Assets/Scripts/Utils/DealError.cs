using Unity.Mathematics;

namespace Utils
{
    public static class DealError
    {
        public static bool GetDealType(ref Random random, bool dealType)
        {
            return random.NextFloat() > .05f ? dealType : !dealType;
        }
    }
}