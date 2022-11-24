using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventLog : MonoBehaviour
{
    [SerializeField] private TMP_Text textLog = null;

    public static EventLog instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    // Print given text in the log.
    public void Print(string text)
    {
        textLog.text += '\n' + text + '\n';
    }

    // Clear the log.
    public void Clear()
    {
        textLog.text = "";
    }
}
