using UnityEngine;

namespace UI
{
    public class DiceHolder : MonoBehaviour
    {
        public DiceType type;
        public DiceLimit limit;
        
        public DiceScript GetDie()
        {
            return transform.GetComponentInChildren<DiceScript>();
        }
    }
}