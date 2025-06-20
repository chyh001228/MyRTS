using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Enemy());
        }
    }
}

public struct Enemy :IComponentData
{

}