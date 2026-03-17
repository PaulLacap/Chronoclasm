using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room")]
public class RoomDefinition : ScriptableObject
{
    public GameObject prefab;

    [Header("Openings")]
    public bool openNorth;
    public bool openEast;
    public bool openSouth;
    public bool openWest;
}
