using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneratorScript : MonoBehaviour
{
    public class CustomTile
    {
        public int x, y, type;
        public List<CustomTile> neighbours;
        public int nNeighbours;
        public int[] room = new int[4]; //origin, width, height

        public CustomTile(int x, int y, int t)
        {
            this.x = x;
            this.y = y;
            this.type = t;
        }
    }

    [System.Serializable]
    public class EnemiesInZone
    {
        public List<EnemySO> enemies;
    }

    [Header("ROOMS")]
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private int minRoomSize;
    [SerializeField] private int maxRoomSize;
    [SerializeField] private int minNumberCells;

    [Header("References")]
    [SerializeField] private Tilemap map;
    [SerializeField] private Tile collisionTile;
    [SerializeField] private Tile hallTile;
    [SerializeField] private AnimatedTile hallSpikesTile;
    [SerializeField] private Tile[] zone0Tiles;
    [SerializeField] private Tile[] zone1Tiles;
    [SerializeField] private Tile[] zone2Tiles;
    [SerializeField] private Tile[] zone3Tiles;
    [SerializeField] private GameObject player;
    [SerializeField] private CustomTile[,] hallPoints;
    [SerializeField] private List<CustomTile> tiles;//room tiles
    [SerializeField] private List<CustomTile> halls;
    [SerializeField] private List<CustomTile> hallsUnion;
    [SerializeField] private GameObject enemy, nextFloor, heart;
    [SerializeField] private GameObject background;
    
    [Header("Collisions")]
    [SerializeField] private Tilemap voidCollisions;
    [SerializeField] private Tilemap roomEdgeCollisions;
    [SerializeField] private Tilemap enemyCollisions;//roomEdgeCollisions = HallsCollision
    
    [HideInInspector]
    [SerializeField] private EnemiesInZone[] enemiesInZone;

    private int[,] mapTile; //0 is void; 1 is room floor; 2 is hall center point; 3 is hall
    private int cellHeight;
    private int cellWidth;
    private int nCellsY;
    private int nCellsX;
    private int maxNumberCells;
    private int numberOfEnemies;
    private Tile[][] zonesTiles;
    private Tile[] floorTiles;
    private int zoneId;
    private int levelId;
    
    void Awake()
    {
        tiles = new List<CustomTile>();
        halls = new List<CustomTile>();
        hallsUnion = new List<CustomTile>();//tile on a room which is the center of halls

        zonesTiles = new Tile[][] { zone0Tiles, zone1Tiles, zone2Tiles, zone3Tiles };
        zoneId = PlayerLevelInfo.currentZone;
        levelId = PlayerLevelInfo.currentLevel;
    }

    // Start is called before the first frame update
    public void GenerateLevel()
    {
        floorTiles = zonesTiles[zoneId];

        //Creation of tiles structures
        mapTile = new int[width, height];

        //This values can be modified
        cellHeight = height / minNumberCells;
        cellWidth = width / minNumberCells;
        nCellsY = minNumberCells;
        nCellsX = minNumberCells;

        background.transform.localScale = new Vector3(nCellsX > nCellsY ? nCellsX * 3 : nCellsY * 3, 1f, nCellsX > nCellsY ? nCellsX * 3 : nCellsY * 3)*30;
        background.transform.position = new Vector3((width) / 2 - 5, (height) / 2 - 5) * 30;

        hallPoints = new CustomTile[nCellsX, nCellsY];

        CreateRooms();

        CreateHalls();

        CreateMap();
    }

    private int GetRandom(int[] validChoices)
    {
        return validChoices[Random.Range(0, validChoices.Length)];
    }

    private void CreateRooms()
    {
        int numberOf0or1room = minNumberCells + maxNumberCells - 2;

        for (int cY = 0; cY < nCellsY; cY++)
        {
            for (int cX = 0; cX < nCellsX; cX++)
            {
                //Tile from which the room is created
                int startX = Random.Range(cX * cellWidth + 1, (cX) * cellWidth + cellWidth / 2); //1 is the space between cells
                int startY = Random.Range(cY * cellHeight + 1, (cY) * cellHeight + cellHeight / 2);

                //Room boundaries
                int[] boundaries = new int[maxRoomSize - minRoomSize + 2 + 1];// 2: 0,1
                boundaries[0] = 0;
                boundaries[1] = 1;
                for (int i = 2; i < boundaries.Length; i++)
                {
                    boundaries[i] = minRoomSize + i - 2;
                }

                int xBound;
                if (numberOf0or1room > 0)
                {
                    xBound = GetRandom(boundaries);
                    numberOf0or1room--;
                }
                else
                {
                    xBound = Random.Range(minRoomSize, maxRoomSize);
                }
                int yBound = xBound < 2 ? xBound : Random.Range(minRoomSize, maxRoomSize);

                int endX = Mathf.Min(startX + xBound, (cX + 1) * cellWidth - 1);
                int endY = Mathf.Min(startY + yBound, (cY + 1) * cellHeight - 1);

                //Tile from which the halls will be created
                int hallPointX = Random.Range(startX, endX);
                int hallPointY = Random.Range(startY, endY);

                //Fill tiles into Room Tiles and Hall Tiles
                for (int toT = startY; toT < endY; toT++)//to Top of tile
                {
                    for (int toR = startX; toR < endX; toR++)// to Right of tile
                    {
                        if (toR == hallPointX && toT == hallPointY)
                        {
                            if (xBound != 1) mapTile[toR, toT] = 2;
                            else mapTile[toR, toT] = 4;
                            CustomTile ct = new CustomTile(toR, toT, 2);
                            ct.nNeighbours = Random.Range(1, 4);
                            ct.room[0] = startX;//origin x
                            ct.room[1] = startY;//origin y
                            ct.room[2] = endX - startX;//width
                            ct.room[3] = endY - startY;//height
                            hallsUnion.Add(ct);
                            hallPoints[cX, cY] = ct;
                            hallPoints[cX, cY].neighbours = new List<CustomTile>();
                        }
                        else
                        {
                            mapTile[toR, toT] = 1;
                            //If it's not on the edge
                            if (toT != startY && toR != startX && toT != endY - 1 && toR != endX - 1) tiles.Add(new CustomTile(toR, toT, 1));
                        }

                    }
                }
            }
        }
    }

    private void CreateHalls()
    {
        //Conect Halls center points
        for (int y = 0; y < nCellsY; y++)
        {
            for (int x = 0; x < nCellsX; x++) //For each possible Room
            {
                if (hallPoints[x, y] != null)
                {
                    for (int nX = x + 1; nX < nCellsX; nX++)
                    {
                        if (hallPoints[x, y].nNeighbours > 0 && hallPoints[nX, y] != null)
                        {
                            if (hallPoints[nX, y].neighbours.Contains(hallPoints[x, y]))
                            {
                                hallPoints[nX, y].nNeighbours--;
                            }
                            else
                            {
                                hallPoints[x, y].neighbours.Add(hallPoints[nX, y]);//add my neighbour
                                hallPoints[x, y].nNeighbours--;
                            }
                            break;
                        }
                    }

                    for (int nY = y + 1; nY < nCellsY; nY++)
                    {
                        if (hallPoints[x, y].nNeighbours > 0 && hallPoints[x, nY] != null)
                        {
                            if (hallPoints[x, nY].neighbours.Contains(hallPoints[x, y]))
                            {
                                hallPoints[x, nY].nNeighbours--;
                            }
                            else
                            {
                                hallPoints[x, y].neighbours.Add(hallPoints[x, nY]);//add my neighbour
                                hallPoints[x, y].nNeighbours--;
                            }
                            break;
                        }
                    }

                    for (int nX = x - 1; nX >= 0; nX--)
                    {
                        if (hallPoints[x, y].nNeighbours > 0 && hallPoints[nX, y] != null)
                        {
                            if (hallPoints[nX, y].neighbours.Contains(hallPoints[x, y]))
                            {
                                hallPoints[nX, y].nNeighbours--;
                            }
                            else
                            {
                                hallPoints[x, y].neighbours.Add(hallPoints[nX, y]);//add my neighbour
                                hallPoints[x, y].nNeighbours--;
                            }
                            break;
                        }
                    }


                    for (int nY = y - 1; nY >= 0; nY--)
                    {
                        if (hallPoints[x, y].nNeighbours > 0 && hallPoints[x, nY] != null)
                        {
                            if (hallPoints[x, nY].neighbours.Contains(hallPoints[x, y]))
                            {
                                hallPoints[x, nY].nNeighbours--;
                            }
                            else
                            {
                                hallPoints[x, y].neighbours.Add(hallPoints[x, nY]);//add my neighbour
                                hallPoints[x, y].nNeighbours--;
                            }
                            break;
                        }
                    }
                }
            }
        }

        //Create Halls
        foreach (CustomTile hU in hallsUnion)
        {
            foreach (CustomTile n in hU.neighbours)
            {
                if (hU.x < n.x)
                {
                    for (int x = hU.x + 1; x <= n.x; x++)
                    {
                        if (mapTile[x, hU.y] == 0)
                        {
                            mapTile[x, hU.y] = 3;
                            halls.Add(new CustomTile(x, hU.y, 3));
                        }
                    }

                    if (hU.y < n.y)
                    {
                        for (int y = hU.y + 1; y <= n.y; y++)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y, 3));
                            }
                        }
                    }
                    else
                    {
                        for (int y = hU.y - 1; y >= n.y; y--)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y, 3));
                            }
                        }
                    }

                }
                else
                {
                    for (int x = hU.x - 1; x >= n.x; x--)
                    {
                        if (mapTile[x, hU.y] == 0)
                        {
                            mapTile[x, hU.y] = 3;
                            halls.Add(new CustomTile(x, hU.y, 3));
                        }
                    }

                    if (hU.y < n.y)
                    {
                        for (int y = hU.y + 1; y <= n.y; y++)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y, 3));
                            }
                        }
                    }
                    else
                    {
                        for (int y = hU.y - 1; y >= n.y; y--)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y, 3));
                            }
                        }
                    }

                }
            }
        }
    }

    private void CreateMap()
    {
        //Set tiles in Level/Dungeon grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (mapTile[x, y] == 0)
                {
                    voidCollisions.SetTile(new Vector3Int(x, y, 0), collisionTile);
                    enemyCollisions.SetTile(new Vector3Int(x, y, 0), collisionTile);
                }

                if (mapTile[x, y] == 3)
                {
                    roomEdgeCollisions.SetTile(new Vector3Int(x, y, 0), hallSpikesTile);
                    enemyCollisions.SetTile(new Vector3Int(x, y, 0), hallTile);
                }

                if (mapTile[x, y] > 0 && mapTile[x, y] != 3)// 1,2,3 
                {
                    map.SetTile(new Vector3Int(x, y, 0), floorTiles[Random.Range(0, floorTiles.Length)]);
                }
            }
        }

        FillMap(); //Fill the map with enemys, player and a stair
    }

    private void FillEnemies()
    {
        //numberOfEnemies = Random.Range(hallsUnion.Count - (3 - levelId), hallsUnion.Count);

        numberOfEnemies = 1;
        
        for (int i = 0; enemiesInZone[zoneId].enemies.Count > 0 && i < numberOfEnemies && hallsUnion.Count > 0; i++)
        {
            //Instantiate one enemy
            int auxTile = Random.Range(0, hallsUnion.Count - 1);
            //Asign a random enemy data of the zone to our enemy instantiated
            int auxEnemyData = Random.Range(0, enemiesInZone[zoneId].enemies.Count);
            EnemySO data = enemiesInZone[zoneId].enemies[auxEnemyData];
            GameObject auxEnemy = Instantiate(enemy, new Vector3(hallsUnion[auxTile].x + data.offset, hallsUnion[auxTile].y + 0.25f + data.offset, 0) * WorldValues.CELL_SIZE, Quaternion.identity);
            auxEnemy.GetComponent<EnemyScript>().EnemyData = data;
            //Asign the animator
            auxEnemy.GetComponent<Animator>().runtimeAnimatorController = data.animator;
            //Finally, initialize the data
            auxEnemy.GetComponent<EnemyScript>().InitEnemyData();

            //Create room edges
            auxEnemy.GetComponent<EnemyScript>().RoomEdgesPosition = new Vector2(hallsUnion[auxTile].room[0] - 0.5f, hallsUnion[auxTile].room[1] - 0.25f) * WorldValues.CELL_SIZE;
            auxEnemy.GetComponent<EnemyScript>().RoomEdgesSize = new Vector2(hallsUnion[auxTile].room[2], hallsUnion[auxTile].room[3]) * WorldValues.CELL_SIZE;
            auxEnemy.GetComponent<EnemyScript>().RoomEdgesEnd = new Vector2(hallsUnion[auxTile].room[0] - 0.5f + hallsUnion[auxTile].room[2], hallsUnion[auxTile].room[1] - 0.25f + hallsUnion[auxTile].room[3]) * WorldValues.CELL_SIZE;

            //Remove his tile from the array to avoid repetitions
            hallsUnion.Remove(hallsUnion[auxTile]);
        }
    }

    private void FillMap()
    {
        //Instantiate Player
        int aux = Random.Range(0, tiles.Count);
        player.transform.position = new Vector3Int(tiles[aux].x, tiles[aux].y, 0) * WorldValues.CELL_SIZE;
        /*tiles.Remove(tiles[aux]);

        //Instantiate Next Floor Stairs
        aux = Random.Range(0, tiles.Count);
        Instantiate(nextFloor, new Vector3(tiles[aux].x, tiles[aux].y + 0.25f, 0) * WorldValues.CELL_SIZE, Quaternion.identity);
        tiles.Remove(tiles[aux]);

        //Instantiate Heart
        aux = Random.Range(0, tiles.Count);
        Instantiate(heart, new Vector3(tiles[aux].x - 0.25f, tiles[aux].y, 0) * WorldValues.CELL_SIZE, Quaternion.identity);
        tiles.Remove(tiles[aux]);*/

        //Remove rooms 1
        foreach (var room in hallsUnion.ToList())
        {
            if (mapTile[room.x, room.y] == 4)
            {
                hallsUnion.Remove(room);
            }
        }

        //Instantiate Enemys
        FillEnemies();

        //Destroy this game object, because at this point is useless.
        Destroy(this.gameObject);
    }

    public EnemiesInZone[] EnemiesUsedInZone
    {
        get { return enemiesInZone; }

        set { enemiesInZone = value; }
    }
}