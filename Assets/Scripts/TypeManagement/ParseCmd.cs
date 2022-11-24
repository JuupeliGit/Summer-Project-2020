using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParseCmd
{
    // Highlight each keyword with it's color in given string.
    public static string ColorBasedOnSyntax(string text, Keyword[] keywords)
    {
        List<string> words = SplitIntoWords(text);

        // Place color tags to color multiple words with different colors.
        for (int i = 0; i < words.Count; i++)
        {
            Keyword currentKeyword = FindKeyword(keywords, words[i]);

            if (currentKeyword.GetColor != new Color(0, 0, 0, 0))
                words[i] = "<color=#" + ColorUtility.ToHtmlStringRGB(currentKeyword.GetColor) + ">" + words[i] + "</color>";
        }

        // Combine the colored words back into a single string.
        return string.Join(" ", words);
    }

    //Return a keyword that matches the given word if found.
    private static Keyword FindKeyword(Keyword[] keywords, string word)
    {
        word = RemoveTags(word);

        for (int j = 0; j < keywords.Length; j++)
        {
            if (word == keywords[j].GetWord)
            {
                return keywords[j];
            }
        }

        return default;
    }

    //Split the string into a list of words.
    public static List<string> SplitIntoWords(string text)
    {
        text = RemoveTags(text);

        string[] words = text.Split(new char[] { ' ' }, 2);
        List<string> list = words.ToList();

        return list;
    }

    //Remove rich text tags from the string.
    private static string RemoveTags(string text)
    {
        while (text.Contains("<") && text.Contains(">"))
        {
            int start = text.IndexOf('<');
            int end = text.IndexOf('>');

            if (start >= 0)
                text = text.Remove(start, end - start + 1);
        }
        return text;
    }

    // THIS IS HOW WE COMBINED AFFIXES TOGETHER AT FIRST
    // WE FOUND A BETTER, SIMPLER SOLUTION THOUGH

    /*
    //Combine affixes with the object if all of the affixes are found.
    public static List<string> JoinAffixes(List<string> words, Word[] constituents)
    {
        List<string> finalList = new List<string>();

        //Loop through all of the words in the list.
        for (int i = 0; i < words.Count; i++)
        {
            bool addAsNew = true;
            bool alreadyIn = false;
            //Loop through all of consituents and find every object.
            for (int j = 0; j < constituents.Length; j++)
            {
                if (constituents[j].GetType() == typeof(Object))
                {
                    Object currentObject = (Object)constituents[j];
                    //Is current word one of the objects?
                    if (words[i] == currentObject.GetWord)
                    {
                        bool foundPrefix = false;
                        bool foundSuffix = false;
                        string suffixString = "";
                        int skipIndex = 0;

                        //Is the previous word a valid prefix?
                        if (i > 0 && words[i - 1] != "" && words[i - 1] == currentObject.prefix && !alreadyIn)
                        {
                            //Debug.Log("Prefix found!");
                            foundPrefix = true;
                        }
                        //Look at the next word to find a possible suffix.
                        if (i < words.Count - 1 && words[i + 1] != "")
                        {
                            //Is the next word "of"?
                            if (currentObject.useOf && words[i + 1] == "of")
                            {
                                suffixString += " " + words[i + 1];
                                skipIndex++;
                            }
                            //Is the next word a valid suffix?
                            if (i + skipIndex < words.Count - 1 && words[i + 1 + skipIndex] == currentObject.suffix)
                            {
                                suffixString += " " + words[i + 1 + skipIndex];
                                skipIndex++;

                                foundSuffix = true;
                            }
                        }

                        //Debug.Log(alreadyIn);

                        //Object has prefix and suffix.
                        if (currentObject.prefix != "" && currentObject.suffix != "" && (foundPrefix || alreadyIn) && foundSuffix)
                        {
                            //Debug.Log("pre and suffix found");
                            finalList[finalList.Count - 1] += (!alreadyIn ? " " + words[i] : "") + suffixString;
                            addAsNew = false;

                            i += skipIndex;
                        }

                        //Object has prefix, but no suffix.
                        else if (currentObject.prefix != "" && currentObject.suffix == "" && foundPrefix)
                        {
                            finalList[finalList.Count - 1] += " " + words[i];
                            alreadyIn = true;
                            addAsNew = false;
                        }

                        //Object has suffix, but no prefix.
                        else if (currentObject.prefix == "" && currentObject.suffix != "" && foundSuffix)
                        {
                            //Debug.Log("Suffix found");
                            finalList.Add(words[i]);
                            finalList[finalList.Count - 1] += suffixString;
                            addAsNew = false;

                            i += skipIndex;
                        }
                    }
                }
            }
            //If current word wasn't combined to another cell, add it as a new cell.
            if(addAsNew)
                finalList.Add(words[i]);
        }
        for (int i = 0; i < finalList.Count; i++)
        {
            Debug.Log(finalList[i] + " // ");
        }
        
        return finalList;
    }
    */
}
