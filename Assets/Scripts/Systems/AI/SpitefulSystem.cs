using Components;
using Unity.Entities;
using Utils;

namespace Systems.AI
{
    [UpdateAfter(typeof(DealSpawnSystem))]
    [UpdateBefore(typeof(DealCloseSystem))]
    public class SpitefulSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());
            if (calendar.Turn == 0 || calendar.Pause) return;

            Entities
                .WithoutBurst()
                .WithAll<Spiteful>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn != calendar.Turn) return;
                    if (deal.FirstMerchantType == MerchantTypes.Spiteful && deal.Turn == 1) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, true);
                    if (deal.SecondMerchantType == MerchantTypes.Spiteful && deal.Turn == 1) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, true);
                    if (deal.FirstMerchantType == MerchantTypes.Spiteful && deal.Turn > 1) deal.FirstMerchantCleanDeal = DealError.GetDealType(ref deal.FirstMerchantRandom, !AmIWasCheated(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, true));
                    if (deal.SecondMerchantType == MerchantTypes.Spiteful && deal.Turn > 1) deal.SecondMerchantCleanDeal = DealError.GetDealType(ref deal.SecondMerchantRandom, !AmIWasCheated(deal.Turn, deal.FirstMerchantId, deal.SecondMerchantId, false));
                }).Run();
        }

        private bool AmIWasCheated(int beforeTurn, int firstMerchantId, int secondMerchantId, bool queryFirst)
        {
            var result = true;
            Entities
                .WithAll<Spiteful>()
                .ForEach((ref Deal deal) =>
                {
                    if (deal.Turn >= beforeTurn || deal.FirstMerchantId != firstMerchantId || deal.SecondMerchantId != secondMerchantId) return;

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
    }
}