using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct DealSpawner : IComponentData
    {
        public Entity DealPrefab;
    }
}