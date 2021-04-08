using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    public class UberSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;
            
            Entities
                .WithoutBurst()
                .WithAll<Uber>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Uber && deal.Turn <= 2) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, true);
                    if (deal.SecondMerchantType == MerchantTypes.Uber && deal.Turn <= 2) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, true);
                    if (deal.FirstMerchantType == MerchantTypes.Uber && deal.Turn == 3) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, !AmICheatedTwice(deal.FirstMerchantId, deal.SecondMerchantId, true));
                    if (deal.SecondMerchantType == MerchantTypes.Uber && deal.Turn == 3) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, !AmICheatedTwice(deal.FirstMerchantId, deal.SecondMerchantId, false));
                    if (deal.FirstMerchantType == MerchantTypes.Uber && deal.Turn > 3) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, !AmICheatedLast3Times(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, true));
                    if (deal.SecondMerchantType == MerchantTypes.Uber && deal.Turn > 3) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, !AmICheatedLast3Times(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, false));
                }).Run();
        }

        private bool AmICheatedTwice(int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = 0;
            Entities
                .WithAll<Uber>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn >= 3 || deal.FirstMerchantId != firstMerchantId || deal.SecondMerchantId != secondMerchantId) return;

                    // IL compiler bug with ternary expressions
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (queryFirst)
                    {
                        if (!deal.SecondMerchantCleanDeal) result++;
                    }
                    else
                    {
                        if(!deal.FirstMerchantCleanDeal) result++;
                    }
                }).Run();
            return result == 2;
        }

        private bool AmICheatedLast3Times(int turn, int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = 0;
            Entities
                .WithAll<Uber>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn >= turn || deal.Turn < turn - 3 || deal.FirstMerchantId != firstMerchantId || deal.SecondMerchantId != secondMerchantId) return;

                    // IL compiler bug with ternary expressions
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (queryFirst)
                    {
                        if (!deal.SecondMerchantCleanDeal) result++;
                    }
                    else
                    {
                        if(!deal.FirstMerchantCleanDeal) result++;
                    }
                }).Run();
            return result >=2;
        }

    }
}