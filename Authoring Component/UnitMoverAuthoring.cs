using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring: MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    public class Baker : Baker<UnitMoverAuthoring> //�決��Dots���
    {
        public override void Bake(UnitMoverAuthoring authoring) //�̶��÷�������Bake����
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); //����ʵ�ʶ��������ϸ�ѡ��TransformUsageFlags����
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