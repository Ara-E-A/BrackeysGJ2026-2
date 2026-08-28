using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    private Story story;
    private bool playing = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TextAsset inkJSON = Resources.Load<TextAsset>("NPCDialog");
        story = new Story(inkJSON.text);
    }

    public void StartNPCDialogue(string greeting, string clue)
    {
        if (playing) return;
        playing = true;

        // Set Ink variables
        story.variablesState["greeting"] = greeting;
        story.variablesState["clue"] = clue;

        // Start at knot
        story.ChoosePathString("NPC_Start");

        ContinueStory();
    }

    public void ContinueStory()
    {
        if (story.canContinue)
        {
            string line = story.Continue();
            DialogUI.Instance.ShowLine(line, story.currentChoices);
        }
        else
        {
            playing = false;
            DialogUI.Instance.Hide();
        }
    }

    public void ChooseChoice(int index)
    {
        story.ChooseChoiceIndex(index);
        ContinueStory();
    }
}
