using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Utilities;

public class EnemyTileComponent : Entity {
	public EnemyTile tile;

	public void Start()
	{
		base.Start();
		LevelManager.Instance.activeEnemies.Add(this);
		SetZPosition();
	}
	


	public override void Update()
	{
		base.Update();
		if (!done && moves == 0)
		{
			done = true;
			moves = maxMoves;
		}
		SetZPosition();
	}

	public override void DoTurn()
	{
		isActive = GetCurrentPosition().GetDistanceTo(GameManager.Instance.player.tilePos) < 10;
		if (!isActive) return;
		Vector3Int playerTileCoords = LevelManager.Instance.GetPlayerTileCoords();
		var neighbors = LevelManager.Instance.GetNeighbors(playerTileCoords);

		Vector3Int min = tilePos;
		var minDist = 99999999999f;
		foreach (var n in neighbors)
		{
			var dist = n.GetDistanceTo(tilePos, false);
			if (dist < minDist)
			{
				min = n;
				minDist = dist;
			}
		}
		
		var path = FindPath(min);
		TryMove(path);
	}

}
