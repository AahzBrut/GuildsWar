using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(DealCloseSystem))] 
    public class MerchantGainSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .WithoutBurst()
                .ForEach((ref Merchant merchant) =>
                {
                    var turnMoney = GetSumOfDeals(merchant.Id, calendar.Turn);
                    merchant.Money += turnMoney;
                })
                .Run();
        }

        private int GetSumOfDeals(int merchantId, int turnId)
        {
            var result = 0;
            Entities
                .ForEach((in Deal deal) =>
                {
                    if (deal.FirstMerchantId == merchantId && deal.Turn == turnId) result += deal.FirstMerchantGain;
                    if (deal.SecondMerchantId == merchantId && deal.Turn == turnId) result += deal.SecondMerchantGain;
                })
                .Run();
            return result;
        }
    }
}