using Unity.Entities;
using Utils;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct Merchant : IComponentData
    {
        public MerchantTypes Type;
        public int Id;
        public int Money;
    }
}