using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class FraudSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var turnNumber = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity()).Turn;
            var pause = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity()).Pause;
            if (turnNumber == 0 || pause) return;

            Entities
                .WithAll<Fraud>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != turnNumber) return;
                    if (deal.FirstMerchantType == MerchantTypes.Fraud) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, false);
                    if (deal.SecondMerchantType == MerchantTypes.Fraud) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, false);
                }).ScheduleParallel();
        }
    }
}