using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Encounter
{
    public override void StartEncounter()
    {
        Debug.Log("Started shop");
        base.StartEncounter();

        EndEncounter();
    }

    public override void EndEncounter()
    {
        Debug.Log("Ended shop");
        base.EndEncounter();
    }
}
