using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class QuirkySystem : SystemBase
    {
        private static readonly bool[] Pattern = {true, false, true, true};

        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .WithoutBurst()
                .WithAll<Quirky>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Quirky && deal.Turn <= 4)
                        deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, Pattern[deal.Turn-1]);
                    if (deal.SecondMerchantType == MerchantTypes.Quirky && deal.Turn <= 4)
                        deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, Pattern[deal.Turn - 1]);
                    if (deal.FirstMerchantType == MerchantTypes.Quirky && deal.Turn > 4)
                    {
                        if (AmIWasCheated(5, deal.FirstMerchantId, deal.SecondMerchantId, true))
                        {
                            deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, false);
                        }
                        else
                        {
                            deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, GetLastDealType(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, true));
                        }
                    }
                    if (deal.SecondMerchantType == MerchantTypes.Quirky && deal.Turn > 4)
                    {
                        if (AmIWasCheated(5, deal.FirstMerchantId, deal.SecondMerchantId, false))
                        {
                            deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, false);
                        }
                        else
                        {
                            deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, GetLastDealType(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, false));
                        }
                    }
                }).Run();
        }

        private bool AmIWasCheated(int beforeTurn, int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = true;
            Entities
                .WithAll<Quirky>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn >= beforeTurn || deal.FirstMerchantId != firstMerchantId ||
                        deal.SecondMerchantId != secondMerchantId) return;
                    // IL compiler bug with ternary expressions
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (queryFirst)
                    {
                        result &= deal.SecondMerchantCleanDeal;
                    }
                    else
                    {
                        result &= deal.FirstMerchantCleanDeal;
                    }
                }).Run();
            
            return !result;
        }
        
        private bool GetLastDealType(int beforeTurn, int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = false;
            var maxTurn = 0;
            Entities
                .WithAll<Quirky>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn >= beforeTurn || deal.Turn <= maxTurn || deal.FirstMerchantId != firstMerchantId || deal.SecondMerchantId != secondMerchantId) return;
                    maxTurn = deal.Turn;
                    // IL compiler bug with ternary expressions
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (queryFirst)
                    {
                        result = deal.SecondMerchantCleanDeal;
                    }
                    else
                    {
                        result = deal.FirstMerchantCleanDeal;
                    }
                }).Run();

            return result;
        }

    }
}