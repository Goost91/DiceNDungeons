using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : MonoBehaviour
{
    public Tilemap wallMap;
    public Tilemap floorMap;
    public Tilemap objMap;
    public float moveSpeed;

    public SpriteRenderer playerRenderer;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int targetTile = Vector3Int.zero;
        var playerExtentsX = new Vector3(playerRenderer.bounds.extents.x, 0);
        if (Input.GetKeyDown(KeyCode.W))
        {
            //transform.position = transform.position - new Vector3(0,moveSpeed*Time.deltaTime*-1);\
            targetTile = floorMap.WorldToCell(transform.position) + new Vector3Int(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            targetTile = floorMap.WorldToCell(transform.position) + new Vector3Int(0, -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            targetTile = floorMap.WorldToCell(transform.position) + new Vector3Int(-1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            targetTile = floorMap.WorldToCell(transform.position) + new Vector3Int(1, 0, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            
            if (Physics.Raycast(Input.mousePosition, Vector3.left, out hitInfo))
            {
                var tileBase = hitInfo.transform.gameObject.GetComponent<TileBase>();
                Debug.Log(tileBase);
            }
        }

        if (targetTile == Vector3Int.zero || !floorMap.GetTile(targetTile)) return;
        
        transform.position = floorMap.CellToWorld(targetTile) + playerExtentsX;
    }

    private TileBase GetCurrentTile()
    {
        return floorMap.GetTile(floorMap.WorldToCell(transform.position));
    }
}