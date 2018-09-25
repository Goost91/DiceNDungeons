using System;
using System.Collections;
using System.Collections.Generic;
using Skills;
using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Data", order = 1)]
public class SkillScriptableObject : ScriptableObject
{
    public string internalName = "New Skill";
    public string title = "Skill";
    [TextArea] public string description;
    public bool isRanged;
    public SkillType type;
    public DiceType[] dice;
    public DiceLimit[] limitations;

    public Dictionary<DiceType, DiceLimit> dices;

    public bool castOnSelf;
}
