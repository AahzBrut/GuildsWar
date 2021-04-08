using Components;
using Unity.Entities;

namespace Systems
{
    [UpdateBefore(typeof(DealSpawnSystem))]
    public class DealRemoveSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var calendarQuery = GetEntityQuery(ComponentType.ReadOnly<TimeTable>());
            var calendar = GetComponent<TimeTable>(calendarQuery.GetSingletonEntity());

            if (calendar.Turn != 0) return;

            var entityCommandBuffer = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            Entities
                .WithAll<Deal>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    entityCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}