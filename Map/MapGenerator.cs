using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    public Transform playerTransform;

    [Header("Map Generation")]
    public TileSettings tileSettings;
    public Tilemap[] tilemaps;
    public int map_width;
    public int map_height;
    public float scale;

    [Header("Nature Generation")]
    public Transform Nature;
    [Range(0,1)]
    public float foliageProbability;
    public GameObject[] grassFoliage;
    public Tilemap waterFoliageMap;
    public TileBase[] waterFoliage;
    public TileBase[] extraWaterFoliage;
    public GameObject[] sandFoliage;

    [Header("Building Generation")]
    public GameObject housePrefab;
    public GameObject castlePrefab;
    public GameObject towerPrefab;
    [Range(0,1)]
    public float emptyPlots;
    public int castlesPerTown;
    public int towersPerTown;

    [Header("Path Generation")]
    public Rule[] rules;
    public string rootSentence;
    [Range(0,20)]
    public int iterationLimit;
    [Range(0,1)]
    public float changeToIgnoreRule;
    public int length;
    public int lengthRemove;

    int[,] perlin_map;
    int[,] pathMap;
    List<Vector2Int> townCenters;
    TileChecker mapTileChecker;

    void Start()
    {   

        do {
            GenerateMapData();
        } while (!GenerateTownsData());
        PlaceTiles();
        PlaceBuildings();
        PlaceNature();

        Vector2Int randomTownCenter = townCenters[Random.Range(0, 3)];
        Vector3 playerPos = new Vector3(randomTownCenter.x + .5f, randomTownCenter.y - 1.5f, 0);
        playerTransform.position = playerPos;
        playerTransform.gameObject.GetComponent<PlayerMovement>().mapTransform.position = playerPos + Vector3.down * 30;

    }

    private void GenerateMapData()
    {
        perlin_map = PerlinNoise.GeneratePerlinMap(map_width, map_height, scale);
        mapTileChecker = new TileChecker(perlin_map, map_width, map_height);
    }

    private bool GenerateTownsData()
    {
        PathGenerator pathGenerator = new PathGenerator(rules, rootSentence, iterationLimit,
                                                        changeToIgnoreRule, length, lengthRemove);

        int townNum = 3;
        townCenters = new List<Vector2Int>();
        List<Vector2Int> allPaths = new List<Vector2Int>();

        for (int i = 0; i < townNum; i++)
        {

            bool goodTownCenter = false;
            int iterations = 0;
            while (!goodTownCenter)
            {
                if (iterations == 500)
                {
                    return false;
                }

                Vector2Int newTownCenter = new Vector2Int(Random.Range(25, 226), Random.Range(25, 226));
                bool isCenterOnGrass = perlin_map[newTownCenter.x, newTownCenter.y] == 2;
                bool farEnough = IsFarEnough(newTownCenter, townCenters, 50f);
                if (farEnough && isCenterOnGrass)
                {
                    List<Vector2Int> pathPoints = pathGenerator.generateTownPath(newTownCenter);
                    bool isOnGrass = mapTileChecker.arePointsOnGrass(pathPoints);
                    if (isOnGrass)
                    {
                        townCenters.Add(newTownCenter);
                        allPaths.AddRange(pathPoints);
                        goodTownCenter = true;
                    }
                }

                iterations++;
            }

        }

        pathMap = new int[map_width, map_height];
        foreach (Vector2Int point in allPaths)
        {
            pathMap[point.x, point.y] = 1;
        }

        return true;
    }

    private void PlaceTiles()
    {
        TileChecker pathTileChecker = new TileChecker(pathMap, map_width, map_height);

        for (int x = 0; x < map_width; x++)
        {
            for (int y = 0; y < map_height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                int tileIndex = perlin_map[x, y];

                if (pathMap[x, y] == 1)
                {
                    string sandTileName = pathTileChecker.CheckSurroundingTileType("sand", 0, x, y);
                    TileBase sandTile = GetTile(sandTileName);
                    tilemaps[8].SetTile(pos, sandTile);
                }

                switch (tileIndex)
                {
                    case 0:
                        tilemaps[0].SetTile(pos, tileSettings.water_tile);
                        break;

                    case 1:
                        string sandTileName = mapTileChecker.CheckSurroundingTileType("sand", 0, x, y);
                        TileBase sandTile = GetTile(sandTileName);
                        tilemaps[3].SetTile(pos, sandTile);
                        if (sandTileName.Contains("1"))
                        {
                            tilemaps[1].SetTile(pos, tileSettings.water_edge_tile);
                        }
                        break;

                    case 2:
                        string grassTileName = mapTileChecker.CheckSurroundingTileType("grass", 1, x, y);
                        TileBase grassTile = GetTile(grassTileName);
                        tilemaps[4].SetTile(pos, grassTile);
                        if (grassTileName.Contains("1"))
                        {
                            tilemaps[3].SetTile(pos, tileSettings.sand_tile_0000);
                        }
                        break;

                    case 3:
                        string rockTileName = mapTileChecker.CheckSurroundingTileType("rock", 2, x, y);
                        string highGrassName = "grass" + rockTileName.Substring(4);
                        char topGrass = mapTileChecker.GetSpecificTileStringDirection(1, rockTileName);
                        char bottomGrass = mapTileChecker.GetSpecificTileStringDirection(3, rockTileName);
                        if (rockTileName.Contains("1"))
                        {
                            tilemaps[4].SetTile(pos, tileSettings.grass_tile_0000);
                            tilemaps[5].SetTile(pos, tileSettings.terrain_shadow_tile);
                        }

                        if (bottomGrass == '1')
                        {
                            string checkRocksAround = mapTileChecker.CheckSurroundingTileType("elevation", 3, x, y);
                            char checkAboveRock = mapTileChecker.GetSpecificTileStringDirection(1, checkRocksAround);
                            if (checkAboveRock == '1') {
                                char leftGrass = mapTileChecker.GetSpecificTileStringDirection(0, rockTileName);
                                char rightGrass = mapTileChecker.GetSpecificTileStringDirection(2, rockTileName);
                                string elevationTileName = $"elevation_tile_{leftGrass}{rightGrass}";
                                TileBase elevationTile = GetTile(elevationTileName);
                                tilemaps[6].SetTile(pos, elevationTile);
                                tilemaps[7].SetTile(pos, tileSettings.grass_shrub_tile);
                            } else {
                                tilemaps[5].SetTile(pos, null);
                            }
                        }
                        else
                        {
                            if (rockTileName == "rock_tile_0000")
                            {
                                string diagonalTileName = mapTileChecker.CheckDiagonalTileType("rock", 2, x, y);
                                char bottomLeftGrass = mapTileChecker.GetSpecificTileStringDirection(3, diagonalTileName);
                                char bottomRightGrass = mapTileChecker.GetSpecificTileStringDirection(2, diagonalTileName);
                                string finalRockTileName = $"rock_tile_{bottomLeftGrass}{topGrass}{bottomRightGrass}{bottomGrass}";
                                TileBase finalRockTile = GetTile(finalRockTileName);
                                tilemaps[6].SetTile(pos, finalRockTile);
                                highGrassName = "grass" + finalRockTileName.Substring(4);
                            }
                            else
                            {
                                TileBase rockTile = GetTile(rockTileName);
                                tilemaps[6].SetTile(pos, rockTile);
                            }
                            string checkElevationBelow = mapTileChecker.CheckSurroundingTileType("rock", 2, x, y - 1);
                            char elevationBelowBottomGrass = mapTileChecker.GetSpecificTileStringDirection(3, checkElevationBelow);
                            if (elevationBelowBottomGrass == '1')
                            {
                                highGrassName = highGrassName.Substring(0, highGrassName.Length - 1) + "1";
                            }
                            TileBase highGrassTile = GetTile(highGrassName);
                            tilemaps[7].SetTile(pos, highGrassTile);
                        }
                        break;
                }
            }
        }
    }

    private void PlaceBuildings() {

        BuildingHelper helper = new BuildingHelper(pathMap, emptyPlots);
        BuildingPlotGenerator plotGenerator = new BuildingPlotGenerator(pathMap, perlin_map);

        for (int i = 0; i < townCenters.Count; i++) {
            
            Vector2Int center = townCenters[i];
            GameObject townObject = new GameObject("Town " + (i+1));
            Vector3 townPosition = new Vector3(center.x + .5f, center.y + .5f, 0);
            townObject.transform.position = townPosition;

            List<Vector2Int> buildingLocations = plotGenerator.PlaceBuildingsAroundPath(center, Vector2Int.down * 2);
            helper.GetBoundsAndRemoveRandomPlots(buildingLocations);

            // Stretches of building plots, 3 plots = stretch
            List<(Vector2Int pos, bool vertical)> castleStretches = new List<(Vector2Int, bool)>();
            helper.FindStretches(castleStretches);

            HashSet<Vector2Int> usedPoints = new HashSet<Vector2Int>();

            int castlesPlaced = 0;
            while (castlesPlaced < castlesPerTown && castleStretches.Count > 0) {

                int randIndex = Random.Range(0, castleStretches.Count);
                (Vector2Int pos, bool vertical) castleStretch = castleStretches[randIndex];
                Vector2Int castlePos = castleStretch.pos;

                castleStretches.RemoveAt(randIndex);

                if (BuildingHelper.IsPositionUsed(castleStretch.vertical, castlePos, usedPoints)) continue;

                BuildingHelper.AddUsedPoints(castleStretch.vertical, castlePos, usedPoints);
                Vector2Int buildingPos = BuildingHelper.GetBuildingPosition(castleStretch.vertical, castlePos);

                InstantiateBuilding(buildingPos, castlePrefab, townObject, castleStretch.vertical ? 90 : 0);

                castlesPlaced++;

            }

            int towersPlaced = 0;
            while (towersPlaced < towersPerTown) {

                int randIndex = Random.Range(0, buildingLocations.Count);
                Vector2Int towerPos = buildingLocations[randIndex];

                buildingLocations.RemoveAt(randIndex);
                if (usedPoints.Contains(towerPos)) continue;

                usedPoints.Add(new Vector2Int(towerPos.x, towerPos.y));
                InstantiateBuilding(towerPos, towerPrefab, townObject);

                towersPlaced++;

            }

            foreach (Vector2Int location in buildingLocations) {
                if (usedPoints.Contains(location)) continue;
                InstantiateBuilding(location, housePrefab, townObject);
            }

            helper.ResetBounds();
            plotGenerator.ResetBuildingList();

        }

    }

private void PlaceNature() {

    bool skipNextTile = false;

    float centerX = map_width / 2f;
    float centerY = map_height / 2f;

    Dictionary<GameObject, float> grassWeights = new Dictionary<GameObject, float>
    {
        { grassFoliage[0], 0.2f },
        { grassFoliage[1], 0.2f },
        { grassFoliage[2], 0.3f },
        { grassFoliage[3], 0.4f },
        { grassFoliage[4], 0.2f },
        { grassFoliage[5], 0.4f }
    };

    Dictionary<GameObject, float> sandWeights = new Dictionary<GameObject, float>
    {
        { sandFoliage[0], 0.05f }, 
        { sandFoliage[1], 0.1f }, 
        { sandFoliage[2], 0.2f },  
        { sandFoliage[3], 0.15f }, 
        { sandFoliage[4], 0.2f },
        { sandFoliage[5], 0.3f }, 
        { sandFoliage[6], 0.05f },
        { sandFoliage[7], 0.03f }
    };

    Color hillColor = new Color(154f / 255f, 217f / 255f, 154f / 255f);

    for (int y = 0; y < map_height; y++) {

        for (int x = 0; x < map_width; x++) {

            if (skipNextTile) {
                skipNextTile = false;
                continue; // Skip this tile because the previous one was a large object
            }

            int terrainValue = perlin_map[x, y];

            switch (terrainValue) {
                case 0: // Water

                    float distanceX = (centerX - x) * (centerX - x);
                    float distanceY = (centerY - y) * (centerY - y);
                    float distanceMultiplier = centerX / Mathf.Sqrt(distanceX + distanceY);

                    if (Random.value > foliageProbability * distanceMultiplier / 8f) continue; 

                    Dictionary<TileBase, float> waterWeights = new Dictionary<TileBase, float>
                    {
                        { waterFoliage[0], 0.4f },
                        { waterFoliage[1], 0.3f },
                    };

                    if (x + 1 < map_width && perlin_map[x + 1, y] == 0) {
                        waterWeights.Add(waterFoliage[2], 0.2f);
                        waterWeights.Add(waterFoliage[3], 0.1f);
                    }

                    TileBase waterFol = NatureHelper.GetRandomWaterFoliage(waterWeights);
                    if (!waterFol) continue;

                    waterFoliageMap.SetTile(new Vector3Int(x, y, 0), waterFol);

                    if (waterFol == waterFoliage[2] || waterFol == waterFoliage[3]) {
                        TileBase nextWaterFol = waterFol == waterFoliage[2] ? extraWaterFoliage[0] : extraWaterFoliage[1];
                        waterFoliageMap.SetTile(new Vector3Int(x + 1, y, 0), nextWaterFol);
                        skipNextTile = true;
                    }

                    break;
                case 1: // Sand

                    if (Random.value > foliageProbability / 2f) continue;

                    GameObject sandFol = NatureHelper.GetRandomFoliage(sandWeights);
                    if (sandFol) InstantiateFoliage(x, y, sandFol);

                    break;
                case 2: // Grass

                    if (IsAdjacentToPathOrBuilding(x, y)) continue;
                    if (Random.value > foliageProbability) continue;

                    GameObject grassFol = NatureHelper.GetRandomFoliage(grassWeights);
                    if (grassFol) InstantiateFoliage(x, y, grassFol);

                    break;
                case 3: // Hill

                    string checkGrass = mapTileChecker.CheckSurroundingTileType("rock", 2, x, y);
                    string checkDiagonalGrass = mapTileChecker.CheckDiagonalTileType("rock", 2, x, y);
                    if (checkGrass.Contains("1") || checkDiagonalGrass.Contains("1")) continue;

                    char bottomGrass = mapTileChecker.GetSpecificTileStringDirection(3, checkGrass);
                    if (bottomGrass == '1') continue;

                    if (Random.value > foliageProbability) continue;

                    GameObject hillFol = NatureHelper.GetRandomFoliage(grassWeights);
                    if (hillFol) InstantiateFoliage(x, y, hillFol, hillColor);
                    
                    break;
            }

        }

    }

}

    private void InstantiateBuilding(Vector2Int position, GameObject prefab, GameObject parent, float rotation = 0) {
        Vector3 buildingPosition = new Vector3(position.x + .5f, position.y + .5f, 0);
        Quaternion buildingRotation = Quaternion.Euler(0, 0, rotation);
        GameObject building = Instantiate(prefab, buildingPosition, buildingRotation);
        building.transform.parent = parent.transform;
    }

    private void InstantiateFoliage(int x, int y, GameObject obj) {
        Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0);
        GameObject placedObject = Instantiate(obj, position, Quaternion.identity);
        placedObject.transform.parent = Nature;
    }

    private void InstantiateFoliage(int x, int y, GameObject obj, Color color) {
        Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0);
        GameObject placedObject = Instantiate(obj, position, Quaternion.identity);
        placedObject.GetComponent<SpriteRenderer>().color = color;
        placedObject.transform.parent = Nature;
    }

    private bool IsAdjacentToPathOrBuilding(int x, int y) {
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++) {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < map_width && ny >= 0 && ny < map_height) {
                if (pathMap[nx, ny] == 1 || pathMap[nx, ny] == 3) {
                    return true;
                }
            }
        }

        return false;
    }

    private TileBase GetTile(string name)
    {
        return (TileBase)tileSettings.GetType().GetField(name).GetValue(tileSettings);
    }

    private bool IsFarEnough(Vector2Int newPoint, List<Vector2Int> existingPoints, float minDistance)
    {
        foreach (Vector2Int point in existingPoints)
        {
            if (Vector2Int.Distance(newPoint, point) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

}
