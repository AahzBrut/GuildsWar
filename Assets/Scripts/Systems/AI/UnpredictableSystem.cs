using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class UnpredictableSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;
            
            Entities
                .WithAll<Unpredictable>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Unpredictable) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, deal.FirstMerchantRandom.NextBool());
                    if (deal.SecondMerchantType == MerchantTypes.Unpredictable) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, deal.FirstMerchantRandom.NextBool());
                }).ScheduleParallel();
        }
    }
}