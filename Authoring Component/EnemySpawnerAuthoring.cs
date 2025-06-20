using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{


    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int nearbyEnemyAmountMax;
    public float nearbyEnemyAmountDistance;


    public class Baker : Baker<EnemySpawnerAuthoring>
    {

        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                nearbyEnemyAmountMax = authoring.nearbyEnemyAmountMax,
                nearbyEnemyAmountDistance = authoring.nearbyEnemyAmountDistance,
            });
        }
    }


}


public struct EnemySpawner : IComponentData
{

    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int nearbyEnemyAmountMax;
    public float nearbyEnemyAmountDistance;

}