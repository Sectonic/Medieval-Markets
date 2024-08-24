using UnityEngine;
using System.Collections.Generic;

public class BuildingPlotGenerator {

    private int[,] pathMap;
    private readonly int[,] perlinMap;
    private List<Vector2Int> buildingList;
    private static readonly List<Vector2Int> directions = new List<Vector2Int> {
        Vector2Int.up * 2, Vector2Int.right * 2, Vector2Int.down * 2, Vector2Int.left * 2
    };
    private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
    private static readonly int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

    public BuildingPlotGenerator(int[,] pathMap, int[,] perlinMap) {
        this.pathMap = pathMap;
        this.perlinMap = perlinMap;
        buildingList = new List<Vector2Int>();
    }

    public List<Vector2Int> PlaceBuildingsAroundPath(Vector2Int center, Vector2Int comingFrom) {
        foreach (Vector2Int dir in directions) {
            if (dir.Equals(comingFrom)) continue;

            Vector2Int newCenter = center + dir;
            bool onPath = pathMap[newCenter.x, newCenter.y] == 1;
            if (onPath) {
                PlaceBuildingsAroundPath(newCenter, -dir);
            } else if (
                !dir.Equals(-comingFrom) && 
                perlinMap[newCenter.x, newCenter.y] != 3 && 
                perlinMap[newCenter.x, newCenter.y] != 1 && 
                pathMap[newCenter.x, newCenter.y] != 2 &&
                !IsAdjacentToHill(newCenter)
            ) {
                buildingList.Add(newCenter);

                // 2 means building plot
                pathMap[newCenter.x, newCenter.y] = 2;
            }
        }

        return buildingList;
    }

    private bool IsAdjacentToHill(Vector2Int center) {

        for (int i = 0; i < 8; i++) {
            int nx = center.x + dx[i];
            int ny = center.y + dy[i];

            if (perlinMap[nx, ny] == 3) {
                return true;
            }
        }

        return false;
    }

    public void ResetBuildingList() {
        buildingList = new List<Vector2Int>();
    }
}