using System.Linq;
using UI;
using UnityEngine;

namespace Managers
{
    
    public class UIManager : Manager<UIManager>
    {
        public GameObject uiCanvas;
        public GameObject diceHolderPrefab;
        public Transform skillPanelPrefab;

        public SkillPanel AddSkill(SkillType type)
        {
            Transform sp = Instantiate(skillPanelPrefab, uiCanvas.transform.Find("BottomPanel/SkillsPanel/Panel").transform);
            var skillPanel = sp.GetComponent<SkillPanel>();
            skillPanel.data = Resources.Load<SkillScriptableObject>("SkillData/" + type);

            return skillPanel;
        }

        public SkillPanel FindSkill(SkillType type)
        {
            return FindObjectsOfType<SkillPanel>().First(sp => sp.type == type);
        }
    }
}