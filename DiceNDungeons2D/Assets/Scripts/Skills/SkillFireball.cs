using System;
using Entities;
using Managers;
using UI;
using UnityEngine;

namespace Skills
{
    [Serializable]
    public class SkillFireball : Skill
    {
        public bool UseOnTarget(Entity self, Vector3Int pos)
        {

            SkillPanel skillPanel = UIManager.Instance.FindSkill(SkillType.Fireball);
            var strength = skillPanel.GetDie(DiceType.Strength).value;
            var aoe = skillPanel.GetDie(DiceType.AreaOfEffect)?.value != null;

            if (aoe)
            {
                Vector3Int[] toCheck =
                {
                    pos,
                    pos + MoveDirection.North.ToVec3Int(),
                    pos + MoveDirection.West.ToVec3Int(),
                    pos + MoveDirection.South.ToVec3Int(),
                    pos + MoveDirection.East.ToVec3Int()
                };
                
                foreach (Vector3Int v in toCheck)
                {
                    var entityAt = LevelManager.Instance.GetEntitiesAt(v);
                    foreach (Entity entity in entityAt)
                    {
                        entity.ModifyHp(strength);
                    }
                }
            }
            else
            {
                LevelManager.Instance.GetEntityAt(pos).ModifyHp(strength);
            }

            return true;
        }

        public bool UseOnSelf(Entity self)
        {
            throw new NotImplementedException();
        }
    }
}