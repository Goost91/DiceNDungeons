using Entities;
using UnityEngine;

namespace Skills
{
    public interface Skill
    {
        bool UseOnTarget(Entity self, Vector3Int pos);

        bool UseOnSelf(Entity self);
    }
}