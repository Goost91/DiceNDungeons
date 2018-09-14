using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Entities
{
    public class PlayerScript : Entity
    {
        public float moveSpeed;

        // Use this for initialization
        void Start()
        {
            var targetTile = LevelManager.Instance.floorMap.WorldToCell(transform.position);
            positionFix = new Vector3(spriteRenderer.bounds.extents.x, 0);
            transform.position = LevelManager.Instance.floorMap.CellToWorld(targetTile) + positionFix;
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            MoveDirection dir = MoveDirection.None;

            if (Input.GetKeyDown(KeyCode.W))
            {
                dir = MoveDirection.North;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                dir = MoveDirection.South;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                dir |= MoveDirection.West;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                dir |= MoveDirection.East;
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = LevelManager.Instance.floorMap.WorldToCell(worldPoint);

                var path = FindPath(position);
                TryMove(path);
            }

            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = LevelManager.Instance.enemyMap.WorldToCell(worldPoint);

                if (LevelManager.Instance.enemyMap.HasTile(position))
                {
                    var enemyTile = LevelManager.Instance.enemyMap.GetTile<EnemyTile>(position);
                    enemyTile.component.Die();
                }
            }

            if (dir == MoveDirection.None || moves <= 0) return;

            if (Move(dir))
            {
                moves--;
                GameManager.Instance.UpdateUI();
            }
        }

        public void EndTurn()
        {
            moves = maxMoves;
        }
    }
}