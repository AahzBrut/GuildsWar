using Components;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    public class DealCloseSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .ForEach((ref Deal deal) =>
                {
                    if (calendar.Turn != deal.Turn) return;
                    if (deal.FirstMerchantCleanDeal && deal.SecondMerchantCleanDeal)
                    {
                        deal.FirstMerchantGain = 4;
                        deal.SecondMerchantGain = 4;
                    }
                    if (!deal.FirstMerchantCleanDeal && deal.SecondMerchantCleanDeal)
                    {
                        deal.FirstMerchantGain = 5;
                        deal.SecondMerchantGain = 1;
                    }
                    if (deal.FirstMerchantCleanDeal && !deal.SecondMerchantCleanDeal)
                    {
                        deal.FirstMerchantGain = 1;
                        deal.SecondMerchantGain = 5;
                    }
                    if (!deal.FirstMerchantCleanDeal && !deal.SecondMerchantCleanDeal)
                    {
                        deal.FirstMerchantGain = 2;
                        deal.SecondMerchantGain = 2;
                    }
                }).ScheduleParallel();
        }
    }
}