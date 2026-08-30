using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PaperReq req;
    public PlayerPaper playerPaper;
    public List<Clue> clues;

    void Start()
    {
        //Generate rules
        req = new PaperReq();

        //Generate player identity
        playerPaper = new PlayerPaper();

        //Generate clues and hand them out so every required rule is covered by an NPC or Thing
        ClueGenerator generator = new ClueGenerator(req);
        clues = generator.GenerateAndDistribute();

        Debug.Log("Rules, PlayerPaper, and Clues generated.");

        //Fill NPC dialogue banks from the Souls-style library
        NPCDialoguePopulator dialoguePopulator = FindAnyObjectByType<NPCDialoguePopulator>();
        if (dialoguePopulator != null)
        {
            dialoguePopulator.Populate();
        }

        //Fallback: fill any remaining ClueHolders (non NPC/Thing) the generator did not assign
        ClueDistributor distributor = FindAnyObjectByType<ClueDistributor>();
        if (distributor != null)
        {
            distributor.Distribute(clues);
        }
    }
}
