using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Pathfinding;
using ProcGen;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : Manager<LevelManager>
{
    public Tilemap floorMap;
    public Tilemap objMap;
    public Tilemap wallMap;
    public Tilemap enemyMap;

    public EnemyTileComponent[] enemies;
    public SquareGrid grid;

    public List<Entity> activeEnemies;
    public DungeonCreator generator;
    public GameObject[] wallTiles;
    public GameObject[] floorTiles;

    public void Start()
    {
        grid = new SquareGrid(0,0,100,100);

        generator = GetComponent<DungeonCreator>();
        generator.GenerateDungeon();

        InstantiateFloorTiles();
        InstantiateEnemies();
        //InstantiateOuterWalls();
        //grid = FloorMapToSquareGrid();
        grid.tiles = generator.tiles;
        GameManager.Instance.player.SetGridPosition(generator.playerStartPos);
    }

    private void InstantiateEnemies()
    {
        foreach (Vector3Int spawnPos in generator.enemySpawns)
        {
            Instantiate(enemies[0], FloorTileToWorld(spawnPos) + Vector3.back, Quaternion.identity);
        }
    }

    public Vector3Int GetPlayerTileCoords()
    {
        return floorMap.WorldToCell(GameManager.Instance.playerGO.transform.position);
    }

    public Vector3 FloorTileToWorld(Vector3Int tile)
    {
        return floorMap.CellToWorld(tile);
    }

    public void InstantiateFloorTiles()
    {
        for (int i = 0; i < generator.tiles.GetLength(0); i++)
        {
            for (int j = 0; j < generator.tiles.GetLength(1); j++)
            {
                if (generator.tiles[i,j] == TileType.Floor)
                {
                    // ... and instantiate a floor tile for it.
                    //InstantiateFromArray(floorTiles, i, j);
                    floorMap.SetTile(new Vector3Int(i,j,0), Resources.Load<DungeonFloorTile>("FloorTile_1"));
                }
            }
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
    }

    void InstantiateOuterWalls()
    {
        // The outer walls are one unit left, right, up and down from the board.
        float leftEdgeX = -1f;
        float rightEdgeX = generator.columns + 0f;
        float bottomEdgeY = -1f;
        float topEdgeY = generator.rows + 0f;

        // Instantiate both vertical walls (one on each side).
        InstantiateVerticalOuterWall(leftEdgeX, bottomEdgeY, topEdgeY);
        InstantiateVerticalOuterWall(rightEdgeX, bottomEdgeY, topEdgeY);

        // Instantiate both horizontal walls, these are one in left and right from the outer walls.
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, bottomEdgeY);
        InstantiateHorizontalOuterWall(leftEdgeX + 1f, rightEdgeX - 1f, topEdgeY);
    }


    void InstantiateVerticalOuterWall(float xCoord, float startingY, float endingY)
    {
        // Start the loop at the starting value for Y.
        float currentY = startingY;

        // While the value for Y is less than the end value...
        while (currentY <= endingY)
        {
            // ... instantiate an outer wall tile at the x coordinate and the current y coordinate.
            InstantiateFromArray(wallTiles, xCoord, currentY);

            currentY++;
        }
    }


    void InstantiateHorizontalOuterWall(float startingX, float endingX, float yCoord)
    {
        // Start the loop at the starting value for X.
        float currentX = startingX;

        // While the value for X is less than the end value...
        while (currentX <= endingX)
        {
            // ... instantiate an outer wall tile at the y coordinate and the current x coordinate.
            InstantiateFromArray(wallTiles, currentX, yCoord);

            currentX++;
        }
    }

    public SquareGrid FloorMapToSquareGrid()
    {
        SquareGrid sg = new SquareGrid(0, 0, 15, 25);

        for (int x = 0; x < sg.width; x++)
        {
            for (int y = 0; y < sg.height; y++)
            {
                var tile = floorMap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    sg.tiles[x, y] = TileType.Floor;
                }
                else
                {
                    sg.tiles[x, y] = TileType.Wall;
                }
            }
        }

        return sg;
    }

    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int coords)
    {
        return grid.Neighbors(coords);
    }

    public Entity GetEntityAt(Vector3Int pos)
    {
        return activeEnemies.FirstOrDefault(e => e.GetCurrentPosition() == pos);
    }
    
    public IEnumerable<Entity> GetEntitiesAt(Vector3Int pos)
    {
        return activeEnemies.Where(e => e.GetCurrentPosition() == pos);
    }
}