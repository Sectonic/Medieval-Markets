using UnityEngine;
using System.Collections.Generic;

public class BuildingHelper {

    private int[,] pathMap;
    private float emptyPlots;
    private int minX, maxX, minY, maxY;

    public BuildingHelper(int[,] pathMap, float emptyPlots) {
        this.pathMap = pathMap;
        this.emptyPlots = emptyPlots;
        minX = int.MaxValue;
        maxX = int.MinValue;
        minY = int.MaxValue;
        maxY = int.MinValue;
    }

    public void GetBoundsAndRemoveRandomPlots(List<Vector2Int> buildingLocations) {

        for (int j = buildingLocations.Count - 1; j >= 0; j--) {
            Vector2Int buildingLocation = buildingLocations[j];

            if (Random.value < emptyPlots) {
                buildingLocations.RemoveAt(j);
                continue;
            }

            if (buildingLocation.x < minX) minX = buildingLocation.x;
            if (buildingLocation.x > maxX) maxX = buildingLocation.x;
            if (buildingLocation.y < minY) minY = buildingLocation.y;
            if (buildingLocation.y > maxY) maxY = buildingLocation.y;

            // 3 means OFFICIALLY going to be a building
            pathMap[buildingLocation.x, buildingLocation.y] = 3;
        }

    }

    public void FindStretches(List<(Vector2Int, bool)> castleStretches) {

        // Find Horizontal Stretches
        for (int y = minY; y <= maxY; y += 2) {
            int currentHorizontalStretch = 0;
            for (int x = minX; x <= maxX; x += 2) {
                if (pathMap[x, y] == 3) {
                    currentHorizontalStretch += 1;
                    if (currentHorizontalStretch >= 3) {
                        castleStretches.Add((new Vector2Int(x, y), false));
                    }
                } else {
                    currentHorizontalStretch = 0;
                }
            }
        }

        // Find Vertical Stretches
        for (int x = minX; x <= maxX; x += 2) {
            int currentVerticalStretch = 0;
            for (int y = minY; y <= maxY; y += 2) {
                if (pathMap[x, y] == 3) {
                    currentVerticalStretch += 1;
                    if (currentVerticalStretch >= 3) {
                        castleStretches.Add((new Vector2Int(x, y), true));
                    }
                } else {
                    currentVerticalStretch = 0;
                }
            }
        }

    }

    public static bool IsPositionUsed(bool isVertical, Vector2Int pos, HashSet<Vector2Int> usedPoints) {
        if (isVertical) {
            return usedPoints.Contains(pos) ||
                usedPoints.Contains(new Vector2Int(pos.x, pos.y - 2)) ||
                usedPoints.Contains(new Vector2Int(pos.x, pos.y - 4));
        } else {
            return usedPoints.Contains(pos) ||
                usedPoints.Contains(new Vector2Int(pos.x - 2, pos.y)) ||
                usedPoints.Contains(new Vector2Int(pos.x - 4, pos.y));
        }
    }

    public static void AddUsedPoints(bool isVertical, Vector2Int pos, HashSet<Vector2Int> usedPoints) {
        if (isVertical) {
            usedPoints.Add(pos);
            usedPoints.Add(new Vector2Int(pos.x, pos.y - 2));
            usedPoints.Add(new Vector2Int(pos.x, pos.y - 4));
        } else {
            usedPoints.Add(pos);
            usedPoints.Add(new Vector2Int(pos.x - 2, pos.y));
            usedPoints.Add(new Vector2Int(pos.x - 4, pos.y));
        }
    }

    public static Vector2Int GetBuildingPosition(bool isVertical, Vector2Int pos) {
        return isVertical ? new Vector2Int(pos.x, pos.y - 2) : new Vector2Int(pos.x - 2, pos.y);
    }

    public void ResetBounds() {
        minX = int.MaxValue;
        maxX = int.MinValue;
        minY = int.MaxValue;
        maxY = int.MinValue;
    }

}