using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTile : Tile
{
	public EnemyTileComponent component;
	public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
	{
		base.GetTileData(location, tileMap, ref tileData);
		component = tileData.gameObject.GetComponent<EnemyTileComponent>();
		component.tile = this;
		component.position = location;
	}
	

#if UNITY_EDITOR
	[MenuItem("Assets/Create/Enemy Tile")]
	public static void CreateAnimatedTile()
	{
		string path = EditorUtility.SaveFilePanelInProject("Save Enemy Tile", "New Enemy Tile", "asset",
			"Save Enemy Tile", "Assets");
		if (path == "")
			return;

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EnemyTile>(), path);
	}
#endif
}
