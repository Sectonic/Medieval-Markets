using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NatureHelper {

    public static GameObject GetRandomFoliage(Dictionary<GameObject, float> foliageWeights) {

        float totalWeight = 0;
        foreach (float weight in foliageWeights.Values)
        {
            totalWeight += weight;
        }

        float choice = Random.value * totalWeight;
        float cumulativeWeight = 0;

        foreach (var pair in foliageWeights)
        {
            cumulativeWeight += pair.Value;
            if (choice <= cumulativeWeight)
            {
                return pair.Key;
            }
        }

        return null;
    }

    public static TileBase GetRandomWaterFoliage(Dictionary<TileBase, float> foliageWeights) {

        float totalWeight = 0;
        foreach (float weight in foliageWeights.Values)
        {
            totalWeight += weight;
        }

        float choice = Random.value * totalWeight;
        float cumulativeWeight = 0;

        foreach (var pair in foliageWeights)
        {
            cumulativeWeight += pair.Value;
            if (choice <= cumulativeWeight)
            {
                return pair.Key;
            }
        }

        return null;
    }

}