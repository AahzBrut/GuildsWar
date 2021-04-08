using Unity.Entities;
using Unity.Mathematics;
using Utils;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct Deal : IComponentData
    {
        public int Turn;
        public int FirstMerchantId;
        public Random FirstMerchantRandom;
        public MerchantTypes FirstMerchantType;
        public bool FirstMerchantCleanDeal;
        public int FirstMerchantGain;
        public int SecondMerchantId;
        public Random SecondMerchantRandom;
        public MerchantTypes SecondMerchantType;
        public bool SecondMerchantCleanDeal;
        public int SecondMerchantGain;
    }
}