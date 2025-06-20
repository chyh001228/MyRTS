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
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),//GameObject 转实体
                scale = authoring.VisualScale,//烘焙成独立组件，改变选中标记的大小，=0为不显示
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
