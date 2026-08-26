using UnityEngine;
using System.Collections.Generic;

public class NPC_TestSetup : MonoBehaviour
{
    void Start()
    {
        var holder = GetComponent<ClueHolder>();

        holder.clue = new Clue(
            RuleType.Age,
            ClueTruth.True,
            new Dictionary<string, object>
            {
                { "min", 20 },
                { "max", 50 }
            }
        );
    }
}
