using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter
{
    public delegate void OnEnd();
    public OnEnd onEnd;

    public virtual void StartEncounter()
    {

    }

    public virtual void EndEncounter()
    {
        onEnd();
    }
}
