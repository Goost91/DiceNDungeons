using Entities;
using UnityEngine;

namespace Skills
{
    public class SkillMovement : Skill
    {
        public bool UseOnTarget(Entity self, Vector3Int pos)
        {
            
            if (LevelManager.Instance.grid.tiles[pos.x, pos.y] != 0)
            {
                var path = self.FindPath(pos);
//                    DrawDebugPath(path);
                self.TryMove(path);
                return true;
            }

            return false;
        }

        public bool UseOnSelf(Entity self)
        {
            throw new System.NotImplementedException();
        }
    }
}