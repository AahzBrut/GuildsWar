using Components;
using MonoBeh;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(PauseSystem))]
    public class ResumeSystem : SystemBase
    {
            protected override void OnUpdate()
        {
            if (!ResumeController.IsResume()) return;
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref TimeTable calendar) =>
            {
                calendar.Pause = false;
            }).Run();

            ResumeController.ResetResume();
        }
    }
}