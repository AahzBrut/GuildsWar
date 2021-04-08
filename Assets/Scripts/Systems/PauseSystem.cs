using Components;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(UIUpdateSystem))]
    public class PauseSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref TimeTable calendar) =>
            {
                if (calendar.Turn == 0) calendar.Pause = true;
            }).Run();
        }
    }
}