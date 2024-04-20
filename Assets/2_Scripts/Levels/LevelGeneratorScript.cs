using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class CustomTile
{
    public int x;
    public int y; 
    public List<CustomTile> neighbours;
    public int nNeighbours;

    public CustomTile(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.nNeighbours = 0;
    }

    public Vector2 Position => new Vector2(x, y);
}

public class LevelGeneratorScript : MonoBehaviour
{
    [System.Serializable]
    public class EnemiesInZone
    {
        public List<EnemyData> enemies;
    }

    [Header("ROOMS")]
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private int minRoomSize;
    [SerializeField] private int maxRoomSize;
    [SerializeField] private int minNumberCells;
    [SerializeField] private GameObject room;

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
    private List<CustomTile> tiles;//room tiles
    private List<CustomTile> halls;
    private List<CustomTile> hallsUnion;
    
    [SerializeField] private GameObject enemy, nextFloor, heart;
    [SerializeField] private GameObject background;
    
    [Header("Collisions")]
    [SerializeField] private Tilemap voidCollisions;
    [SerializeField] private Tilemap roomEdgeCollisions;
    [SerializeField] private Tilemap enemyCollisions;//roomEdgeCollisions = HallsCollision
    
    [SerializeField] private EnemiesInZone[] enemiesInZone;
    [SerializeField] private List<RoomScript> roomsInZone;

    private int[,] mapTile; //0 is void; 1 is room floor; 2 is hall center point; 3 is hall; 4 is 0or1 room
    private int cellHeight;
    private int cellWidth;
    private int nCellsY;
    private int nCellsX;
    private int maxNumberCells;
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

        background.transform.localScale = new Vector3(nCellsX > nCellsY ? nCellsX * 3 : nCellsY * 3, 1f, nCellsX > nCellsY ? nCellsX * 3 : nCellsY * 3) * WorldValues.CELL_SIZE;
        background.transform.position = new Vector3((width) / 2 - 5, (height) / 2 - 5) * WorldValues.CELL_SIZE;

        hallPoints = new CustomTile[nCellsX, nCellsY];

        CreateRooms();

        CreateHalls();

        CreateMap();
        
        FillMap();
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

                if (xBound == 0) continue; //Means the cell will not have a room

                int endX = Mathf.Min(startX + xBound, (cX + 1) * cellWidth - 1);
                int endY = Mathf.Min(startY + yBound, (cY + 1) * cellHeight - 1);

                //Tile from which the halls will be created
                int hallPointX = Random.Range(startX, endX);
                int hallPointY = Random.Range(startY, endY);

                if (xBound <= 1)
                {
                    CustomTile ct = new CustomTile(hallPointX, hallPointY);
                    
                    mapTile[hallPointX, hallPointY] = 4;
                    
                    ct.nNeighbours = Random.Range(1, 4);
                            
                    hallsUnion.Add(ct);
                            
                    hallPoints[cX, cY] = ct;
                    hallPoints[cX, cY].neighbours = new List<CustomTile>();

                    continue;
                }
                
                
                //CREATE PROPERLY A ROOM
                Vector3 newRoomPos = new Vector3((startX + endX - 1) / 2f, (startY + endY - 1) / 2f, 0) * WorldValues.CELL_SIZE;
                RoomScript newRoom = Instantiate(room, newRoomPos, quaternion.identity).GetComponent<RoomScript>();
                newRoom.TilesInRoom = new List<CustomTile>();
                BoxCollider2D bc = newRoom.GetComponent<BoxCollider2D>();
                bc.size = new Vector2(endX - startX, endY - startY) * WorldValues.CELL_SIZE;
                //width  = endX - startX
                //height = endY - startY
                roomsInZone.Add(newRoom);
                newRoom.MaxEnemiesInRoom = Random.Range(1,3);

                //Fill tiles into Room Tiles and Hall Tiles
                for (int toT = startY; toT < endY; toT++)//to Top of tile
                {
                    for (int toR = startX; toR < endX; toR++)// to Right of tile
                    {
                        CustomTile ct = new CustomTile(toR, toT);
                        
                        if (toR == hallPointX && toT == hallPointY)
                        {
                            mapTile[toR, toT] = 2;
                            
                            ct.nNeighbours = Random.Range(1, 4);
                            
                            hallsUnion.Add(ct);
                            
                            hallPoints[cX, cY] = ct;
                            hallPoints[cX, cY].neighbours = new List<CustomTile>();
                        }
                        else
                        {
                            mapTile[toR, toT] = 1;
                            
                            //If it's not on the edge
                            if (toT != startY && toR != startX && toT != endY - 1 && toR != endX - 1)
                            {
                                tiles.Add(ct);
                            }
                        }
                        
                        newRoom.TilesInRoom.Add(ct);
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
                            halls.Add(new CustomTile(x, hU.y));
                        }
                    }

                    if (hU.y < n.y)
                    {
                        for (int y = hU.y + 1; y <= n.y; y++)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y));
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
                                halls.Add(new CustomTile(n.x, y));
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
                            halls.Add(new CustomTile(x, hU.y));
                        }
                    }

                    if (hU.y < n.y)
                    {
                        for (int y = hU.y + 1; y <= n.y; y++)
                        {
                            if (mapTile[n.x, y] == 0)
                            {
                                mapTile[n.x, y] = 3;
                                halls.Add(new CustomTile(n.x, y));
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
                                halls.Add(new CustomTile(n.x, y));
                            }
                        }
                    }

                }
            }
        }
        
        //Remove rooms 1
        foreach (var room in hallsUnion.ToList())
        {
            if (mapTile[room.x, room.y] == 4)
            {
                hallsUnion.Remove(room);
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
                else if (mapTile[x, y] == 3)
                {
                    roomEdgeCollisions.SetTile(new Vector3Int(x, y, 0), hallSpikesTile);
                    enemyCollisions.SetTile(new Vector3Int(x, y, 0), hallTile);
                }
                else// 1,2,4
                {
                    map.SetTile(new Vector3Int(x, y, 0), floorTiles[Random.Range(0, floorTiles.Length)]);
                }
            }
        }
    }
    
    private void FillMap()
    {
        FillPlayer();
        
        FillEnemies();
        
        /*
        //Instantiate Next Floor Stairs
        aux = Random.Range(0, tiles.Count);
        Instantiate(nextFloor, new Vector3(tiles[aux].x, tiles[aux].y + 0.25f, 0) * WorldValues.CELL_SIZE, Quaternion.identity);
        tiles.Remove(tiles[aux]);

        //Instantiate Heart
        aux = Random.Range(0, tiles.Count);
        Instantiate(heart, new Vector3(tiles[aux].x - 0.25f, tiles[aux].y, 0) * WorldValues.CELL_SIZE, Quaternion.identity);
        tiles.Remove(tiles[aux]);
        */

        //Destroy this game object, because at this point is useless.
        Destroy(this.gameObject);
    }

    private void FillPlayer()
    {
        int randRoom = Random.Range(0, roomsInZone.Count);
        int randTile = Random.Range(0, roomsInZone[randRoom].TilesInRoom.Count);
        player.transform.position = roomsInZone[randRoom].TilesInRoom[randTile].Position * WorldValues.CELL_SIZE;
        
        roomsInZone[randRoom].TilesInRoom.RemoveAt(randTile);
    }

    private void FillEnemies()
    {
        while(true)
        {
            if (roomsInZone.Count == 0) break;
            
            int randRoom = Random.Range(0, roomsInZone.Count);
            RoomScript room = roomsInZone[randRoom];

            if (room.MaxEnemiesInRoom <= 0)
            {
                roomsInZone.Remove(room);
                continue;
            }
            
            int randTile = Random.Range(0, room.TilesInRoom.Count);
            
            int auxEnemyData = Random.Range(0, enemiesInZone[zoneId].enemies.Count);
            EnemyData data = enemiesInZone[zoneId].enemies[auxEnemyData];
            
            EnemyModelScript enemyModel = Instantiate(enemy, room.transform).GetComponent<EnemyModelScript>();
            enemyModel.transform.position = room.TilesInRoom[randTile].Position * WorldValues.CELL_SIZE;
            enemyModel.EnemyData = data;
            enemyModel.InitEnemyData();
            room.EnemiesInRoom.Add(enemyModel);
            
            room.MaxEnemiesInRoom--;
        }
    }

    public EnemiesInZone[] EnemiesUsedInZone
    {
        get { return enemiesInZone; }

        set { enemiesInZone = value; }
    }
}
