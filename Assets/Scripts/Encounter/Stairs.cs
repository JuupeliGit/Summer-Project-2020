using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Encounter
{
    public override void StartEncounter()
    {
        Debug.Log("Started stairs");
        base.StartEncounter();

        EndEncounter();
    }

    public override void EndEncounter()
    {
        Debug.Log("Ended stairs");
        base.EndEncounter();
    }
}
