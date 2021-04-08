using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class TricksterSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .WithoutBurst()
                .WithAll<Trickster>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Trickster && deal.Turn == 1) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, true);
                    if (deal.SecondMerchantType == MerchantTypes.Trickster && deal.Turn == 1) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, true);
                    if (deal.FirstMerchantType == MerchantTypes.Trickster && deal.Turn > 1) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, GetLastDealType(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, true));
                    if (deal.SecondMerchantType == MerchantTypes.Trickster && deal.Turn > 1) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, GetLastDealType(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, false));
                }).Run();
        }

        private bool GetLastDealType(int beforeTurn, int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = false;
            var maxTurn = 0;
            Entities
                .WithAll<Trickster>()
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