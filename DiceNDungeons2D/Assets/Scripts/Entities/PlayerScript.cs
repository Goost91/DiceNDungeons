using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Pathfinding;
using Skills;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Entities
{
    public class PlayerScript : Entity
    {
        public float moveSpeed;

        public List<SkillType> skillsUsedTurn;
        public List<SkillType> skillsAvailable;

        public GameObject uiCanvas;
        public Dictionary<SkillType, SkillPanel> skillToPanelMap = new Dictionary<SkillType, SkillPanel>();

        // Use this for initialization
        void Start()
        {
            var targetTile = LevelManager.Instance.floorMap.WorldToCell(transform.position);
            positionFix = new Vector3(spriteRenderer.bounds.extents.x, 0);
            transform.position = LevelManager.Instance.floorMap.CellToWorld(targetTile);
            base.Start();
            AddSkill(SkillType.Movement);
            AddSkill(SkillType.Melee);
            AddSkill(SkillType.Fireball);
        }

        void AddSkill(SkillType skill)
        {
            skillsAvailable.Add(skill);
            skillToPanelMap.Add(skill, UIManager.Instance.AddSkill(skill));
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            MoveDirection dir = CheckMovementInput();

            if (done && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = LevelManager.Instance.floorMap.WorldToCell(worldPoint);
                if (LevelManager.Instance.grid.tiles[position.x, position.y] != 0)
                {
                    var path = FindPath(position);
//                    DrawDebugPath(path);
                    TryMove(path);
                }
            }

            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
                Vector3Int position = LevelManager.Instance.floorMap.WorldToCell(worldPoint);

                var skillPanel = FindObjectsOfType<SkillPanel>().First(sp => sp.IsSelected());

                if (skillPanel.type != null && !skillsUsedTurn.Contains(skillPanel.type.Value))
                {
                    Debug.Log("Skillpanels " + skillPanel.type);
                    Skill skill = GameManager.Instance.skills[skillPanel.type.Value];
                    if (skill.UseOnTarget(this, position))
                    {
                        skillsUsedTurn.Add(skillPanel.type.Value);
                    }
                }
            }

            if (dir == MoveDirection.None || moves <= 0) return;

            if (Move(dir))
            {
                moves--;

                GameManager.Instance.UpdateUI();
            }
        }

        private void DrawDebugPath(List<NodeWithCost<Vector3Int>> path)
        {
            var pathWorldPos = path.Select(i => LevelManager.Instance.floorMap.CellToWorld(i.node)).ToList();
            for (var i = 0; i < pathWorldPos.Count; i++)
            {
                Vector3 tile = pathWorldPos[i];
                Vector3 prevTile = LevelManager.Instance.floorMap.CellToWorld(GetCurrentPosition());
                if (i != 0)
                {
                    prevTile = pathWorldPos[i - 1];
                }

                Debug.DrawLine(prevTile, tile, Color.cyan, 10000);
            }
        }

        public void CheckUiUpdates(SkillPanel s)
        {
            switch (s.data.type)
            {
                case SkillType.Movement:
                    DiceScript diceScript = s.GetDie(DiceType.Range);

                    if (diceScript != null)
                    {
                        moves = maxMoves = diceScript.value;
                        GameManager.Instance.UpdateUI();
                    }

                    break;
            }
        }

        private static MoveDirection CheckMovementInput()
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

            return dir;
        }

        public void EndTurn()
        {
            skillsUsedTurn.Clear();
            moves = maxMoves;
            done = true;
        }
    }
}