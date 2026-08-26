using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea]
    public string[] startingLines;

    [Header("Message building blocks")]
    public string[] messagePartsA;
    public string[] messagePartsB;
    public string[] messagePartsC;

    private bool hasInteracted = false;

    public string GetStartingLine()
    {
        return startingLines[Random.Range(0, startingLines.Length)];
    }

    public bool HasInteracted => hasInteracted;
    public void MarkInteracted() => hasInteracted = true;

    public string BuildMessageFromClue(Clue clue)
    {
        string a = messagePartsA[Random.Range(0, messagePartsA.Length)];
        string b = messagePartsB[Random.Range(0, messagePartsB.Length)];
        string c = messagePartsC[Random.Range(0, messagePartsC.Length)];

        string clueText = NPCClueInterpreter.Interpret(clue);

        return $"{a} {b} {c}\n\n{clueText}";
    }
}
