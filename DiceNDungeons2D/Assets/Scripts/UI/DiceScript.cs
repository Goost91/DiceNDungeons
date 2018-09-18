using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceScript : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int value;
    public TextMeshProUGUI tmp;

    public int maxValue = 6;

    public bool isDragging = false;

    public Vector2 origin;

    public GameObject dicePanel;
    public GraphicRaycaster gr;
    public Vector3 loc;

    public bool isLocked;
    public bool canRoll;

    public Transform newParent;

    // Use this for initialization
    void Start()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        loc = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        if (!isDragging)
        {
            isDragging = true;
            Debug.Log("Starting drag");
            if (transform.parent.gameObject.name.Contains("DiceSlot"))
            {
                transform.parent = dicePanel.transform;
            }
            else
            {
                origin = transform.position;
            }
        }

        if (eventData.pointerEnter != null && (eventData.pointerEnter.name.Contains("DiceSlot") ||
                                               eventData.pointerEnter.name.Contains("DicePanel")))
        {
            newParent = eventData.pointerEnter.transform;
        }


        transform.position = new Vector3(20, -20, 5) + (Vector3) eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hitInfo = new RaycastHit();
        var worldPos = GetComponent<RectTransform>().rect.position;

        isDragging = false;
        if (transform.parent.gameObject.name.Contains("DiceSlot")) return;

        if (eventData.pointerEnter == null ||
            (newParent != null && newParent.name != "DicePanel" && newParent.GetComponentInChildren<DiceScript>() != null))
        {
            transform.position = origin;
        }
        else
        {
            transform.parent = newParent;
            if (newParent.name.Contains("DiceSlot"))
            {
                isLocked = true;
                var skillPanel = newParent.parent.parent.parent.parent.GetComponent<SkillPanel>();
                GameManager.Instance.player.SendMessage("CheckUiUpdates", skillPanel);
            }
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(transform.parent.GetComponent<RectTransform>(),
            transform.position))
        {
            Debug.Log("Ending Drag");
            transform.position = origin;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging || canRoll) return;

        value = Random.Range(0, maxValue) + 1;
        tmp.text = $"{value}";
    }

    public void Reset()
    {
        isDragging = canRoll = isLocked = false;
        transform.parent = dicePanel.transform;
    }
}