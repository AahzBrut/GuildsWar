using System;
using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateAfter(typeof(TurnSystem))]
    public class DealSpawnSystem : SystemBase
    {
        private static Random _random = new Random(256);

        private static readonly IDictionary<MerchantTypes, Action<EntityManager, Entity>> Mappings =
            new Dictionary<MerchantTypes, Action<EntityManager, Entity>>
            {
                {MerchantTypes.Altruist, (entityManager, entity) => entityManager.AddComponent<Altruist>(entity)},
                {MerchantTypes.Fraud, (entityManager, entity) => entityManager.AddComponent<Fraud>(entity)},
                {MerchantTypes.Unpredictable,(entityManager, entity) => entityManager.AddComponent<Unpredictable>(entity)},
                {MerchantTypes.Trickster, (entityManager, entity) => entityManager.AddComponent<Trickster>(entity)},
                {MerchantTypes.Spiteful, (entityManager, entity) => entityManager.AddComponent<Spiteful>(entity)},
                {MerchantTypes.Quirky, (entityManager, entity) => entityManager.AddComponent<Quirky>(entity)},
                {MerchantTypes.Uber, (entityManager, entity) => entityManager.AddComponent<Uber>(entity)}
            };

        // Spawn deals for all merchants for current year in turn 0
        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn != 0 || calendar.Pause) return;

            var dealManagerQuery = GetEntityQuery(ComponentType.ReadOnly<DealSpawner>());
            var dealPrefab = GetComponent<DealSpawner>(dealManagerQuery.GetSingletonEntity()).DealPrefab;

            var merchantQuery = GetEntityQuery(ComponentType.ReadOnly<Merchant>());
            var merchants = merchantQuery.ToComponentDataArray<Merchant>(Allocator.TempJob);

            Entities
                .WithStructuralChanges()
                .ForEach((in Merchant merchant) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    for (var i = 0; i < merchants.Length; i++)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        if (merchants[i].Id <= merchant.Id) continue;

                        var numDeals = _random.NextInt(5, 11);
                        for (var j = 1; j <= numDeals; j++)
                        {
                            var newDeal = EntityManager.Instantiate(dealPrefab);
                            EntityManager.SetComponentData(newDeal, new Deal
                            {
                                Turn = j,
                                FirstMerchantId = merchant.Id,
                                FirstMerchantRandom = new Random(_random.NextUInt()),
                                FirstMerchantType = merchant.Type,
                                // ReSharper disable once AccessToDisposedClosure
                                SecondMerchantId = merchants[i].Id,
                                SecondMerchantRandom = new Random(_random.NextUInt()),
                                // ReSharper disable once AccessToDisposedClosure
                                SecondMerchantType = merchants[i].Type
                            });
                            Mappings[merchant.Type].Invoke(EntityManager, newDeal);
                            // ReSharper disable once AccessToDisposedClosure
                            Mappings[merchants[i].Type].Invoke(EntityManager, newDeal);
                        }
                    }
                }).Run();
            merchants.Dispose();
        }
    }
}