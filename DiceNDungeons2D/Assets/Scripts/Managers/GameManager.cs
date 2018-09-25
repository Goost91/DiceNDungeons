using System.Collections;
using System.Collections.Generic;
using Entities;
using Skills;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : Manager<GameManager>
{
    public GameObject playerGO;
    public PlayerScript player;
    
    public int playerMoveDistance;
    public bool showMoveDistance;

    public Dictionary<SkillType, Skill> skills = new Dictionary<SkillType, Skill>
    {
        {SkillType.Fireball, new SkillFireball()}, 
        {SkillType.Healing, new SkillHealing()}, 
        {SkillType.Movement, new SkillMovement()}, 
        {SkillType.Melee, new SkillMelee()}, 
    };

    public Camera cam;

    public GameObject uiCanvas;
    
    public void Start()
    {
        if (playerGO == null)
        {
            playerGO = FindObjectOfType<PlayerScript>().gameObject;
        }
        player = playerGO.GetComponent<PlayerScript>();
        UpdateUI();
    }
 
    public void UpdateUI()
    {
        Transform hpVal = uiCanvas.transform.Find("Panel/Stats/HP LG/Value");
        hpVal.GetComponent<TextMeshProUGUI>().text = $"{player.hp} / {player.maxHp}";
        Transform movesVal = uiCanvas.transform.Find("Panel/Stats/Moves LG/Value");
        movesVal.GetComponent<TextMeshProUGUI>().text = $"{player.moves} / {player.maxMoves}";
    }

    public void EndTurn()
    {
        player.EndTurn();
        StartCoroutine(ProcessEnemies());
        UpdateUI();
        
        foreach (DiceScript diceScript in FindObjectsOfType<DiceScript>())
        {
            diceScript.Reset();
        }

    }

    public IEnumerator ProcessEnemies()
    {
        foreach (Entity e in LevelManager.Instance.activeEnemies)
        {
            e.DoTurn();
            
        }

        foreach (Entity e in LevelManager.Instance.activeEnemies)
        {
            while (!e.done)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
