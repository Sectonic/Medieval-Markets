using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileSettings", menuName = "ScriptableObjects/TileSettings", order = 1)]
public class TileSettings : ScriptableObject
{
    [Header("Water Tiles")]
    public TileBase water_tile;
    public TileBase water_edge_tile;

    [Header("Grass Tiles")]
    public TileBase grass_tile_1100;
    public TileBase grass_tile_0100;
    public TileBase grass_tile_0110;
    public TileBase grass_tile_1000;
    public TileBase grass_tile_0000;
    public TileBase grass_tile_0010;
    public TileBase grass_tile_1001;
    public TileBase grass_tile_0001;
    public TileBase grass_tile_0011;
    public TileBase grass_tile_1110;
    public TileBase grass_tile_1010;
    public TileBase grass_tile_1011;
    public TileBase grass_tile_1101;
    public TileBase grass_tile_0101;
    public TileBase grass_tile_0111;
    public TileBase grass_tile_1111;
    public TileBase grass_shrub_tile;

    [Header("Sand Tiles")]
    public TileBase sand_tile_1100;
    public TileBase sand_tile_0100;
    public TileBase sand_tile_0110;
    public TileBase sand_tile_1000;
    public TileBase sand_tile_0000;
    public TileBase sand_tile_0010;
    public TileBase sand_tile_1001;
    public TileBase sand_tile_0001;
    public TileBase sand_tile_0011;
    public TileBase sand_tile_1110;
    public TileBase sand_tile_1010;
    public TileBase sand_tile_1011;
    public TileBase sand_tile_1101;
    public TileBase sand_tile_0101;
    public TileBase sand_tile_0111;
    public TileBase sand_tile_1111;
    public TileBase sand_shrub_tile;

    [Header("Shadow Tile")]
    public TileBase terrain_shadow_tile;

    [Header("Rock Tiles")]
    public TileBase rock_tile_1100;
    public TileBase rock_tile_0100;
    public TileBase rock_tile_0110;
    public TileBase rock_tile_1000;
    public TileBase rock_tile_0000;
    public TileBase rock_tile_0010;
    public TileBase rock_tile_1001;
    public TileBase rock_tile_0001;
    public TileBase rock_tile_0011;
    public TileBase rock_tile_1110;
    public TileBase rock_tile_1010;
    public TileBase rock_tile_1011;
    public TileBase rock_tile_1101;
    public TileBase rock_tile_0101;
    public TileBase rock_tile_0111;
    public TileBase rock_tile_1111;

    [Header("Elevation Tiles")]
    public TileBase elevation_tile_10;
    public TileBase elevation_tile_01;
    public TileBase elevation_tile_11;
    public TileBase elevation_tile_00;

}