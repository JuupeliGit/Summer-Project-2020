using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct ThemeSprites
{
    public Sprite room;
    public Sprite[] doors;

    public Sprite GetRoom
    {
        get { return room; }
    }

    public Sprite[] GetDoors
    {
        get { return doors; }
    }
}

[CreateAssetMenu(fileName = "New Theme", menuName = "Theme")]
public class FloorTheme : ScriptableObject
{
    // Holds all the room sprites and the music.
    // RoomManager changes sprites per room/floors based on these.
    [SerializeField] private string tuneName = null;
    [SerializeField] private float volume = 0.15f;

    [SerializeField] private SpawnTable spawnTable = null;

    [SerializeField] ThemeSprites[] themeSprites = null;


    public Sprite GetRoom(int i)
    {
        return themeSprites[i].GetRoom;
    }

    public Sprite[] GetDoors(int i)
    {
        return themeSprites[i].GetDoors;
    }

    public string GetTune
    {
        get { return tuneName; }
    }

    public float GetVolume
    {
        get { return volume; }
    }

    public SpawnTable GetSpawnTable
    {
        get { return spawnTable; }
    }

    public int GetThemesLength
    {
        get { return themeSprites.Length; }
    }
}
