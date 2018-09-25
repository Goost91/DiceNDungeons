using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class SkillPanel : MonoBehaviour

    {
        public SkillScriptableObject data;
        public SkillType? type => data?.type;

        public void Start()
        {
            SetUiData();
        }

        public void SetUiData()
        {
            if (data == null) return;
            transform.Find("Content/Title").GetComponent<TextMeshProUGUI>().text = data.title;
            transform.Find("Content/Description").GetComponent<TextMeshProUGUI>().text = data.description;
            transform.Find("Content/Title").GetComponentInChildren<Toggle>().onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    foreach (SkillPanel skillPanel in FindObjectsOfType<SkillPanel>().Where(sp => sp != this))
                    {
                        skillPanel.transform.GetComponentInChildren<Toggle>().isOn = false;
                    }
                }
            });

            for (var index = 0; index < data.dice.Length; index++)
            {
                DiceType diceType = data.dice[index];
                GameObject dh = Instantiate(UIManager.Instance.diceHolderPrefab,
                    transform.Find("Content/DiceHolderPanel"));

                var diceScript = dh.GetComponentInChildren<DiceHolder>();
                if (diceScript != null)
                {
                    diceScript.type = diceType;
                    diceScript.limit = data.limitations[index];
                }

                Transform find = dh.transform.Find("Description");
                var tmp = find.GetComponent<TextMeshProUGUI>();
                tmp.text = diceType.GetAttribute<DescriptionAttribute>().Description;
            }
        }

        public bool IsSelected()
        {
            var component = GetComponentInChildren<Toggle>();

            return component != null && component.isOn;
        }

        public DiceScript GetDie(DiceType t)
        {
            var diceHolders = GetDice();
            DiceHolder diceHolder = diceHolders.First(die => die.type == t);
            return diceHolder.GetDie();
        }

        public List<DiceHolder> GetDice()
        {
            return transform.Find("Content/DiceHolderPanel").GetComponentsInChildren<DiceHolder>().ToList();
        }
    }
}