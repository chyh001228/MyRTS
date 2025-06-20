using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class UnitMovementManager : MonoBehaviour
{
    public static UnitMovementManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static NativeArray<float3> MovementMode(string modeName, float3 targetPosition, int unitsCount)
    {
        NativeArray<float3> result = new NativeArray<float3>(unitsCount, Allocator.Temp);
        if (modeName == "circle")
        {
            result = GenerateMovementPositionArray(targetPosition, unitsCount);
        }

        return result;
    }

    private static NativeArray<float3> GenerateMovementPositionArray(float3 targetPosition, int unitsCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(unitsCount, Allocator.Temp);
        if (unitsCount == 0)
        {
            return positionArray;
        }
        positionArray[0] = targetPosition;
        if (unitsCount == 1)
        {
            return positionArray;
        }

        float ringSize = 2f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < unitsCount)
        {
            int ringPositionCount = 4 + ring * 4;

            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (math.PI2/ ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if (positionIndex >= unitsCount)
                {
                    break;
                }

                
            }
            ring++;
        }

        return positionArray;
    }
}
