using Unity.Entities;
using UnityEngine;

class SelectedAuthoring : MonoBehaviour
{

    public GameObject visualGameObject;
    public float VisualScale;
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected{
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),//GameObject תʵ��
                scale = authoring.VisualScale,//�決�ɶ���������ı�ѡ�б�ǵĴ�С��=0Ϊ����ʾ
            }); 
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float scale;

    public bool onSelected;
    public bool onDeselected;
}
