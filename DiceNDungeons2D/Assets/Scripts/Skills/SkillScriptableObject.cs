using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Skills/Data", order = 1)]
public class SkillScriptableObject : ScriptableObject
{
    public string internalName = "New Skill";
    public string title = "Skill";
    [TextArea] public string description;
    public string target;
    public SkillType type;
    public DiceType[] dice;
}
