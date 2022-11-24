using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Encounter
{
    public override void StartEncounter()
    {
        base.StartEncounter();

        EndEncounter();
    }

    public override void EndEncounter()
    {
        base.EndEncounter();
    }
}
