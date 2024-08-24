using System.Collections.Generic;
using UnityEngine;

public class TileChecker
{
    private int[,] perlinMap;
    private int mapWidth;
    private int mapHeight;

    public TileChecker(int[,] perlinMap, int mapWidth, int mapHeight)
    {
        this.perlinMap = perlinMap;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
    }

    public void updateMap(int[,] new_map) {
        perlinMap = new_map;
    }

    public string CheckSurroundingTileType(string currentTile, int tileIndex, int x, int y)
    {
        bool leftMatch = x > 0 && perlinMap[x - 1, y] == tileIndex;
        bool rightMatch = x < mapWidth - 1 && perlinMap[x + 1, y] == tileIndex;
        bool topMatch = y < mapHeight - 1 && perlinMap[x, y + 1] == tileIndex; 
        bool bottomMatch = y > 0 && perlinMap[x, y - 1] == tileIndex;

        return $"{currentTile}_tile_{(leftMatch ? "1" : "0")}{(topMatch ? "1" : "0")}{(rightMatch ? "1" : "0")}{(bottomMatch ? "1" : "0")}";
    }

    public string CheckDiagonalTileType(string currentTile, int tileIndex, int x, int y)
    {
        bool topLeftMatch = x > 0 && y < mapHeight - 1 && perlinMap[x - 1, y + 1] == tileIndex;
        bool topRightMatch = x < mapWidth - 1 && y < mapHeight - 1 && perlinMap[x + 1, y + 1] == tileIndex;
        bool bottomLeftMatch = x > 0 && y > 0 && perlinMap[x - 1, y - 1] == tileIndex; 
        bool bottomRightMatch = x < mapWidth - 1 && y > 0 && perlinMap[x + 1, y - 1] == tileIndex;

        return $"{currentTile}_tile_{(topLeftMatch ? "1" : "0")}{(topRightMatch ? "1" : "0")}{(bottomRightMatch ? "1" : "0")}{(bottomLeftMatch ? "1" : "0")}";
    }

    public string CombineTileStrings(string currentTile, List<string> tileStrings)
    {
        char[] result = { '0', '0', '0', '0' };

        foreach (string tileString in tileStrings)
        {
            int startIndex = tileString.Length - 4;

            for (int i = 0; i < 4; i++)
            {
                if (tileString[startIndex + i] == '1')
                {
                    result[i] = '1';
                }
            }
        }

        return $"{currentTile}_tile_{new string(result)}";
    }

    public string CheckSurroundingTiles(string currentTile, int[] tileIndexes, int x, int y)
    {
        List<string> tileStrings = new List<string>();

        foreach (int tileIndex in tileIndexes)
        {
            tileStrings.Add(CheckSurroundingTileType(currentTile, tileIndex, x, y));
        }

        return CombineTileStrings(currentTile, tileStrings);
    }

    public char GetSpecificTileStringDirection(int direction, string tileString)
    {
        string directionNumbers = tileString[^4..];
        return directionNumbers[direction];
    }

    public bool arePointsOnGrass(List<Vector2Int> pathPoints) 
    {

        int[,] directions = {
            {0, 1}, {0, -1}, {1, 0}, {-1, 0}, // N, S, E, W
            {1, 1}, {-1, 1}, {1, -1}, {-1, -1} // NE, NW, SE, SW
        };

        foreach (Vector2Int point in pathPoints)
        {   
            int x = point.x;
            int y = point.y;
            
            // Check if the point itself is on grass
            if (x <= 0 || y <= 0 || x >= mapWidth || y >= mapHeight || perlinMap[x, y] != 2)
            {
                return false;
            }

            // Check all adjacent points
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int adjX = x + directions[i, 0];
                int adjY = y + directions[i, 1];

                if (adjX < 0 || adjY < 0 || adjX >= mapWidth || adjY >= mapHeight || perlinMap[adjX, adjY] != 2)
                {
                    return false;
                }
            }
        }

        return true;
    }
}