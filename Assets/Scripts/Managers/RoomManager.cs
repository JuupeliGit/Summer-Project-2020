using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    // PLAYER MOVEMENT
    private Vector2Int playerPos;
    private bool canMove = true;
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    [SerializeField] private Animator sweepAnimatorRoom = null;
    [SerializeField] private Animator sweepAnimatorFloor = null;

    // ROOM
    private int floor;

    [SerializeField] private BaseItemStats[] startingItems = null;

    [SerializeField] private GameObject[] doorObjects = null;
    [SerializeField] private SpriteRenderer roomRenderer = null;
    [SerializeField] private GameObject[] encounterObjects = null;
    [SerializeField] private GameObject[] itemObjects = null;

    [SerializeField] private GameObject coin = null;
    private List<GameObject> coins = new List<GameObject>();

    [SerializeField] private Keyword[] roomTargetKeywords = null;

    // EVENTS
    public delegate void OnPlayerMove(Vector2Int playerPos);
    public event OnPlayerMove onPlayerMove;

    //THEME
    [SerializeField] FloorTheme[] themes = null;
    [SerializeField] private int floorsPerTheme = 2;
    [SerializeField] TMP_Text floorInfo = null;
    private FloorTheme currentTheme;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        FindObjectOfType<DungeonGeneration>().onGenerateDungeon += GetDungeonLayout;
    }

    // Called after a new floor is generated.
    private void GetDungeonLayout()
    {
        // Clear the Event Log
        EventLog.instance.Clear();

        floor++;

        // Get current theme based on the floor number.
        currentTheme = themes[((floor - 1) / floorsPerTheme) % themes.Length];

        // Play music.
        SoundManager.instance.PlayMusic(currentTheme.GetTune, currentTheme.GetVolume, 0.5f, 0.5f);
        
        //SpawnTable
        EncounterManager.instance.SetSpawnTable = currentTheme.GetSpawnTable;

        // Reset player position to the center of the dungeon.
        playerPos = new Vector2Int(2, 2);

        // Spawn starting items if it's the first floor.
        if (floor == 1)
        {
            for (int i = 0; i < startingItems.Length; i++)
            {
                Item newItem = new Item(startingItems[i], 0);

                Vector2 randomPos = new Vector2(Random.Range(0f, 3f), -1.6f - 0.3f * i);
                newItem.position = randomPos;

                CurrentRoom.SetItem(newItem, i);
            }

            EventLog.instance.Print("equip stick");
            EventLog.instance.Print("take apple");
            ChangeFloor();
        }
        // Play sound &  transition is it's not the first floor.
        else
        {
            SoundManager.instance.PlaySound("transition_floor", 1f, 1f, 0.2f);
            sweepAnimatorFloor.SetTrigger("ChangeFloor");

            // This function is called while the transition is still blocking the view.
            Invoke("ChangeFloor", 0.5f);
        }
    }

    // Update UI and other info.
    private void ChangeFloor()
    {
        // Update minimap.
        onPlayerMove(playerPos);

        // Update text at the top of the screen.
        floorInfo.text = "Floor " + floor + " " + currentTheme.name;

        UpdateRoomInfo();
    }

    // Set sprites for the room and the doors based on current theme.
    private void SetRoomSprites()
    {
        int i = CurrentRoom.GetVariationIndex(currentTheme.GetThemesLength);

        //Doors
        Sprite[] doorSprites = currentTheme.GetDoors(i);
        for (int j = 0; j < 4; j++)
        {
            doorObjects[j].GetComponent<SpriteRenderer>().sprite = doorSprites[j];
        }

        //Room
        roomRenderer.sprite = currentTheme.GetRoom(i);
    }

    // Called when moving to another room.
    public void MovePlayer(int i)
    {
        if (canMove)
            StartCoroutine("MoveToRoom", directions[i]);
    }

    // Move player from one room to another if the new room exists.
    private IEnumerator MoveToRoom(Vector2Int dir)
    {
        DungeonRoom[,] rooms = DungeonGeneration.instance.GetRoomLayout;

        // Make sure that the index is in range & the room exists.
        if (!rooms.IndexOutOfRange(playerPos + dir) && !rooms.CheckForValue(playerPos + dir, null))
        {
            // Disable player movement.
            canMove = false;

            SoundManager.instance.PlaySound("transition_room", 0.2f, 1.5f, 0.2f);

            // Player the corrent transition animation.
            if (dir == Vector2Int.up)
                sweepAnimatorRoom.SetTrigger("Up");
            else if (dir == Vector2Int.down)
                sweepAnimatorRoom.SetTrigger("Down");
            else if (dir == Vector2Int.left)
                sweepAnimatorRoom.SetTrigger("Left");
            else if (dir == Vector2Int.right)
                sweepAnimatorRoom.SetTrigger("Right");

            // Wait for 0.25 seconds before changing anything in the room.
            yield return new WaitForSeconds(0.25f);

            // Set new player position.
            playerPos += dir;

            // Entering a room removes the question mark on the minimap.
            CurrentRoom.isMystery = false;

            // If the new room is a shop, then player shop music, else play current music from current theme.
            if (CurrentRoom.encounterType == EncounterType.Shop) 
                SoundManager.instance.PlayMusic("shop", 0.75f, 0.5f, 0f);
            else
                SoundManager.instance.PlayMusic(currentTheme.GetTune, currentTheme.GetVolume, 0.5f, 0f);

            // Update minimap.
            onPlayerMove(playerPos);

            UpdateRoomInfo();

            // Start what ever encounter is in this room.
            EncounterManager.instance.BeginEncounter();

            //yield return new WaitForSeconds(0.25f);
        }
    }

    private void UpdateRoomInfo()
    {
        // Hide all item & interactable object sprites and nametags when changing rooms.
        CleanRoom();

        // Show all items, enemies, doors and other room specific sprites.
        ShowDoors();
        ShowItems();
        SetRoomSprites();

        // Update keyword list with on all interactable object in the current room.
        CurrentRoomTargetKeywords = ListCurrentRoomTargets();
    }

    // Spawn an item on the floor in the given slot.
    // There are only 3 slots that an item can be spawned in.
    public void SpawnItem(int itemLevel, BaseItemStats itemStats, int slot, Vector2 spawnLocation)
    {
        // Set sprite for the item.
        GameObject itemObject = itemObjects[slot];

        itemObject.GetComponent<SpriteRenderer>().sprite = itemStats.GetSprite;
        itemObject.SetActive(true);

        // Create the actualy item object and set it in the current room.
        Item newItem = new Item(itemStats, itemLevel);
        CurrentRoom.SetItem(newItem, slot);

        // Calculate item trajectory for it's animation.
        Vector2 startPos = spawnLocation + new Vector2(0, 1);
        Vector2 endPos = new Vector2(Random.Range(-0.5f, 0.5f) + spawnLocation.x, itemObject.transform.position.y);
        Vector2[] curvePoints =
        {
            startPos,
            new Vector2(Mathf.Lerp(startPos.x, endPos.x, 0.5f), startPos.y + 3),
            endPos
        };

        newItem.position = endPos;

        // Start item drop animation.
        StartCoroutine(AnimateDrop(curvePoints, slot));
    }

    // Remove an item in the room.
    public void RemoveItem(Item item)
    {
        //Loops through all the items to check if given Item object exists
        for (int slot = 0; slot < 3; slot++)
        {
            if (CurrentRoom.GetItem(slot) == item)
            {
                NametagManager.instance.HideNametag(true, slot);
                itemObjects[slot].SetActive(false);

                CurrentRoom.SetItem(null, slot);

                return;
            }
        }
    }

    // Spawn an amount of coins in the given position.
    public IEnumerator SpawnCoin(int count, Vector2 pos)
    {
        for (int i = 0; i < count; i++)
        {
            coins.Add(Instantiate(coin, pos, Quaternion.identity));
            yield return new WaitForSeconds(0.1f);
        }
    }

    //Calculates and sets the position for item in "slot". Points list is for the beginning and end of the bezier curve
    private IEnumerator AnimateDrop(Vector2[] points, int slot)
    {
        //time variable for bezier curve
        float t = 0;

        Transform item = itemObjects[slot].transform;

        //Calculates the bezier curve when 0 < t < 1
        while (t < 1)
        {
            Vector2 currentPos = points[1] + Mathf.Pow((1 - t), 2) * (points[0] - points[1]) + Mathf.Pow(t, 2) * (points[2] - points[1]);
            item.position = currentPos;

            t += Time.deltaTime * 1.3f;
            yield return new WaitForEndOfFrame();
        }
        //Just to be sure, let's set the item's position to be the wanted end position
        item.position = points[2];

        SoundManager.instance.PlaySound("item_land", 0.9f, 1f, 0.2f);

        if (CurrentRoom.GetItem(slot) != null)
            NametagManager.instance.ShowNametag(true, slot, new Vector2(points[2].x, -1.5f), CurrentRoom.GetItem(slot).GetKeyword, -1);
    }

    //Searches the room for an item with the given name
    public Item GetItemByName(string name)
    {
        for (int i = 0; i < 3; i++)
        {
            if (CurrentRoom.GetItem(i) != null && CurrentRoom.GetItem(i).GetKeyword.GetWord == name)
            {
                Item temp = CurrentRoom.GetItem(i);
                return temp;
            }
        }

        return null;
    }

    public void RefreshRoom()
    {
        // Update minimap.
        onPlayerMove(playerPos);
        canMove = true;
    }

    // Hide all item & interactable object sprites and nametags in the room.
    private void CleanRoom()
    {
        for (int i = 0; i < encounterObjects.Length; i++)
            encounterObjects[i].SetActive(false);

        for (int i = 0; i < itemObjects.Length; i++)
            itemObjects[i].SetActive(false);

        NametagManager.instance.HideAll();

        StopCoroutine("RemoveCoins");
        //StartCoroutine(RemoveCoins(0f));
    }

    //Shows doors to adjacent rooms if there are any.
    private void ShowDoors()
    {
        DungeonRoom[,] rooms = DungeonGeneration.instance.GetRoomLayout;

        for (int i = 0; i < directions.Length; i++)
        {
            bool enableDoor = (!rooms.IndexOutOfRange(playerPos + directions[i]) && !rooms.CheckForValue(playerPos + directions[i], null));
            doorObjects[i].SetActive(enableDoor);
        }
    }

    // Show items & their nametags that are already in the room when entering.
    public void ShowItems()
    {
        for (int slot = 0; slot < 3; slot++)
        {
            if (CurrentRoom.GetItem(slot) != null)
            {
                Vector2 nameTagPos = new Vector2(CurrentRoom.GetItem(slot).position.x, -1.5f);

                int price = (CurrentRoom.encounterType == EncounterType.Shop ? CurrentRoom.GetItem(slot).GetBuyPrice : -1);
                NametagManager.instance.ShowNametag(true, slot, nameTagPos, CurrentRoom.GetItem(slot).GetKeyword, price);

                itemObjects[slot].GetComponent<SpriteRenderer>().sprite = CurrentRoom.GetItem(slot).GetSprite;
                itemObjects[slot].transform.position = CurrentRoom.GetItem(slot).position;
                itemObjects[slot].SetActive(true);
            }
            else
            {
                NametagManager.instance.HideNametag(true, slot);
                itemObjects[slot].SetActive(false);
            }
        }
    }

    public void SetChest(bool show)
    {
        encounterObjects[0].SetActive(show);
    }

    public void SetShop(bool show)
    {
        encounterObjects[1].SetActive(show);
    }

    public void SetStairs(bool show)
    {
        encounterObjects[2].SetActive(show);
    }

    public int GetFloor
    {
        get { return floor; }
    }

    public DungeonRoom CurrentRoom
    {
        get { return DungeonGeneration.instance.RoomAtPosition(playerPos); }
    }

    // List all the possible keywords for interactable objects in this room.
    public Keyword[] ListCurrentRoomTargets()
    {
        List<Keyword> currentKeywords = new List<Keyword>();

        for(int i = 0; i < doorObjects.Length; i++)
        {
            if (doorObjects[i].activeSelf)
                currentKeywords.Add(roomTargetKeywords[i]);
        }

        if(CurrentRoom.encounterType == EncounterType.Stairs)
            currentKeywords.Add(roomTargetKeywords[4]);

        if (CurrentRoom.encounterType == EncounterType.Chest)
            currentKeywords.Add(roomTargetKeywords[5]);

        return currentKeywords.ToArray();
    }

    public Keyword[] CurrentRoomTargetKeywords
    {
        get;
        private set;
    }
}
