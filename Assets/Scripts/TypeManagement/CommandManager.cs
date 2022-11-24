using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Keywords are highlighted with their color when the player types them.
[System.Serializable]
public struct Keyword
{
    [SerializeField] private string word;
    [SerializeField] private Color color;

    public Keyword(string str, Color clr)
    {
        word = str;
        color = clr;
    }

    public string GetWord
    {
        get { return word; }
    }

    public Color GetColor
    {
        get { return color; }
    }
}

public class CommandManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField cmdField = default;

    [SerializeField] private Keyword[] verbs = default;

    private EncounterManager encounterManager;
    private EnemyManager enemyManager;
    private InventoryManager invManager;
    private CommandExecution cmdExe;

    void Start()
    {
        cmdField.ActivateInputField();
        encounterManager = FindObjectOfType<EncounterManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        invManager = FindObjectOfType<InventoryManager>();

        cmdExe = FindObjectOfType<CommandExecution>();
    }

    private void Update()
    {
        // Set the text input box as active to prevent the player from deactivating it when not wanted.
        if (!GameOver.instance.GetGameOver)
            cmdField.ActivateInputField();
        else
            cmdField.DeactivateInputField();
    }

    //Is called when inputfield is updated
    public void ReadCmd(string text)
    {
        //int clipNumber = Random.Range(0, 2);
        SoundManager.instance.PlaySound("kbtest1", 1f, 1f, 0.2f);

        List<string> words = ParseCmd.SplitIntoWords(text);
        Keyword[] highlightableWords = new Keyword[0];

        // Include only those keywords that are compatible with whatever verb the player has typed.
        if (words[0] == "use" || words[0] == "equip" || words[0] == "inspect")
            highlightableWords = CurrentKeywords(true, false, true, true, false);
        else if (words[0] == "drop" || words[0] == "sell")
            highlightableWords = CurrentKeywords(true, false, true, false, false);
        else if (words[0] == "take" || words[0] == "buy")
            highlightableWords = CurrentKeywords(true, false, false, true, false);
        else if (words[0] == "attack")
            highlightableWords = CurrentKeywords(true, true, false, false, false);
        else if (words[0] == "walk" ||words[0] == "open")
            highlightableWords = CurrentKeywords(true, false, false, false, true);

        //Highlight words and then force the string to lowercase
        text = ParseCmd.ColorBasedOnSyntax(text, highlightableWords);
        cmdField.SetTextWithoutNotify(text.ToLower());

        // Set caret position to the end so it won't get stuck between the rich text tags.
        cmdField.caretPosition = text.Length;
    }

    //Is called when the player presses "Enter"
    public void ValidateCmd(string text)
    {
        if (text == "")
            return;

        //Converts inputfield text into a list of words
        List<string> words = ParseCmd.SplitIntoWords(text);

        EnterCommand(words);

        cmdField.text = "";
        cmdField.ActivateInputField();
    }

    //Checks if the first words is a valid verb.
    // If it is, then execute the typed command.
    public void EnterCommand(List<string> words)
    {
        if (words.Count <= 1)
            return;

        for (int i = 0; i < verbs.Length; i++)
        {
            if (words[0] == verbs[i].GetWord)
            {
                cmdExe.CallCommand(words[0], words[1]);
                return;
            }
        }
    }

    /*
    //Check if given word is a valid non-verb keyword
    private Keyword IsWordObject(string word)
    {
        Keyword[] objects = CurrentKeywords(false, true, true, true, false);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetWord == word)
                return objects[i];
        }
        return default;
    }
    */

    //Returns a list of specific highlightable keywords in the current session
    private Keyword[] CurrentKeywords(bool addVerbs, bool addEnemies, bool addInventory, bool addItemsOnFloor, bool addRoom)
    {
        Keyword[] sessionKeywords = new Keyword[0];

        if (addVerbs)
            sessionKeywords = sessionKeywords.Concat(verbs).ToArray();
        if (addEnemies)
            sessionKeywords = sessionKeywords.Concat(enemyManager.CurrentEnemyObjects).ToArray();
        if (addInventory)
            sessionKeywords = sessionKeywords.Concat(invManager.CurrentItemKeywords).ToArray();
        if (addItemsOnFloor)
            sessionKeywords = sessionKeywords.Concat(RoomManager.instance.CurrentRoom.CurrentItemObjects).ToArray();
        if (addRoom)
            sessionKeywords = sessionKeywords.Concat(RoomManager.instance.CurrentRoomTargetKeywords).ToArray();

        return sessionKeywords;
    }
}
