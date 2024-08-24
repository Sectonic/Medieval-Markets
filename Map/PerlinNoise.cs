using UnityEngine;

public static class PerlinNoise
{

    public static int[,] GeneratePerlinMap(int mapWidth, int mapHeight, float scale)
    {

        int[,] perlinMap = new int[mapWidth, mapHeight];

        float offsetX = Random.Range(0f, 100000f);
        float offsetY = Random.Range(0f, 100000f);
        float centerX = mapWidth / 2f;
        float centerY = mapHeight / 2f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float distanceX = (centerX - x) * (centerX - x);
                float distanceY = (centerY - y) * (centerY - y);
                float distanceMultiplier = Mathf.Sqrt(distanceX + distanceY) / centerX;

                float xCoord = (float)x / mapWidth * scale + offsetX;
                float yCoord = (float)y / mapHeight * scale + offsetY;
                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) - distanceMultiplier;

                perlinMap[x, y] = DetermineTileIndex(noiseValue);
            }
        }

        return perlinMap;
    }

    private static int DetermineTileIndex(float noiseValue)
    {
        if (noiseValue < 0.001f) return 0;
        if (noiseValue < 0.075f) return 1;
        if (noiseValue < 0.45f) return 2;
        return 3;
    }

}