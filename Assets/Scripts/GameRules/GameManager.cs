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

        //Generate clues
        ClueGenerator generator = new ClueGenerator(req);
        clues = generator.GenerateClues();

        Debug.Log("Rules, PlayerPaper, and Clues generated.");
    }
}
