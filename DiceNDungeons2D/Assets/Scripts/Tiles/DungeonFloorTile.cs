using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonFloorTile : Tile
{
    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        base.GetTileData(location, tileMap, ref tileData);
        tileData.gameObject.GetComponent<DungeonFloorTileComponent>().tile = this;
        tileData.gameObject.GetComponent<DungeonFloorTileComponent>().tilePosition = location;
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Create/Floor Tile")]
    public static void CreateAnimatedTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Floor Tile", "New Floor Tile", "asset",
            "Save Floor Tile", "Assets");
        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DungeonFloorTile>(), path);
    }
#endif
}