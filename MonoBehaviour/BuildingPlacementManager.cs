using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementManager : MonoBehaviour {


    public static BuildingPlacementManager Instance { get; private set; }


    public event EventHandler OnActiveBuildingTypeSOChanged;


    [SerializeField] private BuildingTypeSO buildingTypeSO;
    [SerializeField] private UnityEngine.Material ghostMaterial;
    [SerializeField] private UnityEngine.Material ghostDisabledMaterial;


    private Transform ghostTransform;
    private Transform ghostTransform_disabled;


    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (ghostTransform != null) {
            ghostTransform.position = MouseWorldPosition.Instance.GetPosition();
            if (ResourceManager.Instance.CanSpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray))
            {
                if (CanPlaceBuilding()) {
                    foreach (MeshRenderer meshRenderer in ghostTransform.GetComponentsInChildren<MeshRenderer>())
                    {
                        meshRenderer.material = ghostMaterial;
                    }
                }
                else
                {
                    foreach (MeshRenderer meshRenderer in ghostTransform.GetComponentsInChildren<MeshRenderer>())
                    {
                        meshRenderer.material = ghostDisabledMaterial;
                    }
                }

            }
        }

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (buildingTypeSO.IsNone()) {
            return;
        }

        if (Input.GetMouseButtonDown(1)) {
            SetActiveBuildingTypeSO(GameAssets.Instance.buildingTypeListSO.none);
        }

        if (Input.GetMouseButtonDown(0)) {
            if (ResourceManager.Instance.CanSpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray)) {
                if (CanPlaceBuilding()) {
                    ResourceManager.Instance.SpendResourceAmount(buildingTypeSO.buildCostResourceAmountArray);

                    Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                    EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
                    EntitiesReferences entitiesReferences = entityQuery.GetSingleton<EntitiesReferences>();

                    //Entity spawnedEntity = entityManager.Instantiate(buildingTypeSO.GetPrefabEntity(entitiesReferences));
                    //entityManager.SetComponentData(spawnedEntity, LocalTransform.FromPosition(mouseWorldPosition));

                    Entity buildingConstructionVisualEntity = entityManager.Instantiate(buildingTypeSO.GetVisualPrefabEntity(entitiesReferences));
                    entityManager.SetComponentData(buildingConstructionVisualEntity, LocalTransform.FromPosition(mouseWorldPosition + new Vector3(0, buildingTypeSO.constructionYOffset, 0)));

                    Entity buildingConstructionEntity = entityManager.Instantiate(entitiesReferences.buildingConstructionPrefabEntity);
                    entityManager.SetComponentData(buildingConstructionEntity, LocalTransform.FromPosition(mouseWorldPosition));
                    entityManager.SetComponentData(buildingConstructionEntity, new BuildingConstruction {
                        buildingType = buildingTypeSO.buildingType,
                        constructionTimer = 0f,
                        constructionTimerMax = buildingTypeSO.buildingConstructionTimerMax,
                        finalPrefabEntity = buildingTypeSO.GetPrefabEntity(entitiesReferences),
                        visualEntity = buildingConstructionVisualEntity,
                        startPosition = mouseWorldPosition + new Vector3(0, buildingTypeSO.constructionYOffset, 0),
                        endPosition = mouseWorldPosition,
                    });

                }
            }
        }
    }

    private bool CanPlaceBuilding() {
        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        CollisionFilter collisionFilter = new CollisionFilter {
            BelongsTo = ~0u,
            CollidesWith = 1u << GameAssets.BUILDINGS_LAYER | 1u << GameAssets.DEFAULT_LAYER,
            GroupIndex = 0,
        };

        UnityEngine.BoxCollider boxCollider = buildingTypeSO.prefab.GetComponent<UnityEngine.BoxCollider>();
        float bonusExtents = 1.1f;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        if (collisionWorld.OverlapBox(
            mouseWorldPosition,
            Quaternion.identity,
            boxCollider.size * .5f * bonusExtents,
            ref distanceHitList,
            collisionFilter)) {
            // Hit something
            return false;
        }

        distanceHitList.Clear();
        if (collisionWorld.OverlapSphere(
            mouseWorldPosition,
            buildingTypeSO.buildingDistanceMin,
            ref distanceHitList,
            collisionFilter)) {
            // Hit something within building radius
            foreach (DistanceHit distanceHit in distanceHitList) {
                if (entityManager.HasComponent<BuildingTypeSOHolder>(distanceHit.Entity)) {
                    BuildingTypeSOHolder buildingTypeSOHolder = entityManager.GetComponentData<BuildingTypeSOHolder>(distanceHit.Entity);
                    if (buildingTypeSOHolder.buildingType == buildingTypeSO.buildingType) {
                        // Same type too close
                        return false;
                    }
                }
                if (entityManager.HasComponent<BuildingConstruction>(distanceHit.Entity)) {
                    BuildingConstruction buildingConstruction = entityManager.GetComponentData<BuildingConstruction>(distanceHit.Entity);
                    if (buildingConstruction.buildingType == buildingTypeSO.buildingType) {
                        // Same type too close
                        return false;
                    }
                }
            }
        }

        if (buildingTypeSO is BuildingResourceHarversterTypeSO buildingResourceHarversterTypeSO) {
            bool hasValidNearbyResourceNodes = false;
            if (collisionWorld.OverlapSphere(
                mouseWorldPosition,
                buildingResourceHarversterTypeSO.harvestDistance,
                ref distanceHitList,
                collisionFilter)) {
                // Hit something within harvest distance
                foreach (DistanceHit distanceHit in distanceHitList) {
                    if (entityManager.HasComponent<ResourceTypeSOHolder>(distanceHit.Entity)) {
                        ResourceTypeSOHolder resourceTypeSOHolder = entityManager.GetComponentData<ResourceTypeSOHolder>(distanceHit.Entity);
                        if (resourceTypeSOHolder.resourceType == buildingResourceHarversterTypeSO.harvestableResourceType) {
                            // Nearby valid resource node
                            hasValidNearbyResourceNodes = true;
                            break;
                        }
                    }
                }
            }
            if (!hasValidNearbyResourceNodes) {
                return false;
            }
        }

        return true;
    }


    public BuildingTypeSO GetActiveBuildingTypeSO() {
        return buildingTypeSO;
    }

    public void SetActiveBuildingTypeSO(BuildingTypeSO buildingTypeSO) {
        this.buildingTypeSO = buildingTypeSO;

        if (ghostTransform != null) {
            Destroy(ghostTransform.gameObject);
        }

        if (!buildingTypeSO.IsNone()) {
            ghostTransform = Instantiate(buildingTypeSO.visualPrefab);
            foreach (MeshRenderer meshRenderer in ghostTransform.GetComponentsInChildren<MeshRenderer>()) {
                meshRenderer.material = ghostMaterial;
            }
        }

        OnActiveBuildingTypeSOChanged?.Invoke(this, EventArgs.Empty);
    }


}