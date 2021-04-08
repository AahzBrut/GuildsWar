using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using Utils;

namespace Systems
{
    [UpdateAfter(typeof(UIUpdateSystem))]
    public class MerchantUpdateSystem : SystemBase
    {
        private struct MerchantComparer : IComparer<Merchant>
        {
            public int Compare(Merchant x, Merchant y)
            {
                return y.Money - x.Money;
            }
        }

        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn != 0 || calendar.Pause) return;

            var merchantQuery = GetEntityQuery(ComponentType.ReadOnly<Merchant>());
            var merchants = merchantQuery.ToComponentDataArray<Merchant>(Allocator.TempJob);

            merchants.Sort(new MerchantComparer());

            var replacedMerchants = GetReplacedMerchants(merchants);

            Entities
                .WithoutBurst()
                .ForEach((ref Merchant merchant) =>
                {
                    merchant.Money = 0;
                    if (!replacedMerchants.ContainsKey(merchant.Id)) return;
                    merchant.Type = replacedMerchants[merchant.Id];
                }).Run();

            merchants.Dispose();
        }

        private Dictionary<int, MerchantTypes> GetReplacedMerchants(NativeArray<Merchant> merchants)
        {
            var best = merchants.Slice(0, 14);
            var worst = merchants.Slice(56, 14);

            var replacedMerchants = new Dictionary<int, MerchantTypes>();

            for (var i = 0; i < worst.Length; i++)
            {
                replacedMerchants.Add(worst[i].Id, best[i].Type);
            }

            return replacedMerchants;
        }
    }
}