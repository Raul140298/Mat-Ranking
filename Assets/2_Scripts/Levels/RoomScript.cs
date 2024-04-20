using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] private List<EnemyModelScript> enemiesInRoom;
    private List<CustomTile> tilesInRoom;
    private int maxEnemiesInRoom;

    public List<EnemyModelScript> EnemiesInRoom
    {
        get { return enemiesInRoom; }
        set { enemiesInRoom = value; }
    }

    public List<CustomTile> TilesInRoom
    {
        get { return tilesInRoom; }
        set { tilesInRoom = value; }
    }

    public int MaxEnemiesInRoom
    {
        get { return maxEnemiesInRoom; }
        set { maxEnemiesInRoom = value; }
    }
}
