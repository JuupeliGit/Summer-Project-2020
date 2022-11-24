using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Constituent
{
    [Header("Base")]
    [Tooltip("Write a keyword for the object")] [SerializeField] protected string word = default;
    [Tooltip("Highlight color for the keyword")] [SerializeField] protected Color color = default;

    public Constituent(string word, Color color)
    {
        this.word = word;
        this.color = color;
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
