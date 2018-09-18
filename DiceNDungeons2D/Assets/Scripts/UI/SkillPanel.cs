using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillPanel: MonoBehaviour

    {
        public SkillScriptableObject data;

        public void Start()
        {
            transform.Find("Content/Title").GetComponent<TextMeshProUGUI>().text = data.title;
            transform.Find("Content/Description").GetComponent<TextMeshProUGUI>().text = data.description;
            
            
            foreach (DiceType type in data.dice)
            {
                GameObject dh = Instantiate(UIManager.Instance.diceHolderPrefab, transform.Find("Content/DiceHolderPanel"));
                dh.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = type.ToString();
            }
        }
    }
}