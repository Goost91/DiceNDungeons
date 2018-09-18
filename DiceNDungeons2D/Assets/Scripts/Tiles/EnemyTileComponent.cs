using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class EnemyTileComponent : Entity {
	public EnemyTile tile;
	public Vector3Int position;

	public void Start()
	{
		base.Start();
		LevelManager.Instance.activeEnemies.Add(this);
	}
	
	public void Die()
	{
		LevelManager.Instance.enemyMap.SetTile(position, null);
	}

	public override void DoTurn()
	{
		var path = FindPath(LevelManager.Instance.GetPlayerTileCoords());
		TryMove(path);
	}
}
