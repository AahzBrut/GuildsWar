using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct TimeTable : IComponentData
    {
        public int Turn;
        public int Year;
        public bool Pause;
    }
}