using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class AltruistSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .WithAll<Altruist>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Altruist) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, true);
                    if (deal.SecondMerchantType == MerchantTypes.Altruist) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, true);
                }).ScheduleParallel();
        }
    }
}