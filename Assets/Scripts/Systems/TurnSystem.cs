using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    
    public class TurnSystem : SystemBase
    {
        
        // Turns and Years controlling system
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref TimeTable calendar) =>
            {
                if (calendar.Pause) return;
                if (calendar.Turn++ < 10) return;

                
                calendar.Turn = 0;
                calendar.Year++;
            }).Run();
        }
    }
}