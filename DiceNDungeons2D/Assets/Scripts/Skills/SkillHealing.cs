using Entities;
using UnityEngine;

namespace Skills
{
    public class SkillHealing : Skill
    {
        public bool UseOnTarget(Entity self, Vector3Int pos)
        {
            LevelManager.Instance.GetEntityAt(pos);

            return true;
        }

        public bool UseOnSelf(Entity self)
        {
            throw new System.NotImplementedException();
            return true;
        }
    }
}