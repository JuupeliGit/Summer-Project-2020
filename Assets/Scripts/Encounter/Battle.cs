using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : Encounter
{
    public override void StartEncounter()
    {
        Debug.Log("Started battle");
        base.StartEncounter();

        EndEncounter();
    }

    public override void EndEncounter()
    {
        Debug.Log("Ended battle");
        base.EndEncounter();
    }
}
