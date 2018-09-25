using System;
using Entities;
using Managers;
using UI;
using UnityEngine;

namespace Skills
{
    public class SkillMelee : Skill
    {
        public bool UseOnTarget(Entity self, Vector3Int pos)
        {
            
            Entity e = LevelManager.Instance.GetEntityAt(pos);
            SkillPanel skillPanel = UIManager.Instance.FindSkill(SkillType.Melee);
            var str = skillPanel.GetDie(DiceType.Strength).value;

            e.ModifyHp(str);

            return true;

            throw new NotImplementedException();
        }

        public bool UseOnSelf(Entity self)
        {
            throw new NotImplementedException();
        }
    }
}