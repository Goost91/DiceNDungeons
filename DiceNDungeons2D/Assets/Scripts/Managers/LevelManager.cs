using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : Manager<LevelManager> {
	public Tilemap floorMap;
	public Tilemap objMap;
	public Tilemap wallMap;
	public Tilemap enemyMap;
	
	public EnemyTileComponent[] enemies;
	public SquareGrid grid;

	public List<Entity> activeEnemies;

	public void Start()
	{
		Instantiate(enemies[0], FloorTileToWorld(new Vector3Int(11,14,0)), Quaternion.identity);
		
		grid = FloorMapToSquareGrid();
	}
   
	public Vector3Int GetPlayerTileCoords()
	{
		return floorMap.WorldToCell(GameManager.Instance.playerGO.transform.position);
	}

	public Vector3 FloorTileToWorld(Vector3Int tile)
	{
		return floorMap.CellToWorld(tile);
	}

	public SquareGrid FloorMapToSquareGrid()
	{
		SquareGrid sg = new SquareGrid(0,0,15,25);

		for(int x = 0; x < sg.width; x++)
		{
			for (int y = 0; y < sg.height; y++)
			{
				var tile = floorMap.GetTile(new Vector3Int(x, y, 0));
				if (tile != null)
				{
					sg.tiles[x, y] = TileType.Floor;
				}
				else
				{
					sg.tiles[x, y] = TileType.Wall;
				}
			}
		}
		
		return sg;
	}

}
