using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring: MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    public class Baker : Baker<UnitMoverAuthoring> //烘焙成Dots组件
    {
        public override void Bake(UnitMoverAuthoring authoring) //固定用法，重载Bake方法
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); //根据实际对象作用严格选择TransformUsageFlags类型
            AddComponent(entity, new UnitMover
            {
                moveSpeed = authoring.moveSpeed, 
                rotateSpeed = authoring.rotateSpeed

            });
        }
    }
}

public struct UnitMover : IComponentData
{
    public float moveSpeed;
    public float rotateSpeed;
    public float3 targetPosition;
    public bool isMoving;
}