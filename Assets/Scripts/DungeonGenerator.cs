using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public Vector2Int dungeonSize = new Vector2Int(5, 5);
    public float roomSize = 12f;

    [Header("Room Pool")]
    public List<RoomDefinition> roomPrefabs;

    private Dictionary<Vector2Int, RoomDefinition> placedRooms =
        new Dictionary<Vector2Int, RoomDefinition>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < dungeonSize.x; x++)
        {
            for (int z = 0; z < dungeonSize.y; z++)
            {
                Vector2Int gridPos = new Vector2Int(x, z);

                RoomDefinition room = GetRandomRoom();
                placedRooms.Add(gridPos, room);

                Vector3 worldPos = GridToWorld(gridPos);
                Instantiate(room.prefab, worldPos, Quaternion.identity, transform);
            }
        }
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            gridPos.x * roomSize,
            0f,
            gridPos.y * roomSize
        );
    }

    RoomDefinition GetRandomRoom()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Count)];
    }
}
