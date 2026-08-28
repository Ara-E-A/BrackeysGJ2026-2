using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The game's single, non-Ink NPC dialogue flow:
///
///   greeting  -> [Continue] [Leave]
///     Continue -> clue -> [Thanks] [I see] [How kind of you] -> end
///     Leave    -> end
///
/// </summary>
public class DialogManager : MonoBehaviour
{
    private static DialogManager instance;

    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<DialogManager>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(DialogManager)).AddComponent<DialogManager>();
                }
            }

            return instance;
        }
    }

    private enum Stage { Idle, Greeting, Clue }

    /// <summary>The non-Ink dialogue data: the fixed lines for the NPC currently talking.</summary>
    [System.Serializable]
    private struct Line
    {
        public string greeting;
        public string clue;
    }

    private Line current;
    private Stage stage = Stage.Idle;
    private ShowDialogue ui;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    /// <summary>Runs greeting -> Continue/Leave -> clue -> final response -> end for one NPC.</summary>
    public void StartNPCDialogue(string greeting, string clue)
    {
        if (stage != Stage.Idle || DBoxControl.speaking)
        {
            return;
        }

        ui = FindAnyObjectByType<ShowDialogue>();
        if (ui == null)
        {
            Debug.LogError("DialogManager: no ShowDialogue in the scene.");
            return;
        }

        current = new Line { greeting = greeting, clue = clue };
        stage = Stage.Greeting;

        ui.ShowNPCLine(current.greeting,
            Option(PlayerDialogueOption.Continue, ShowClue),
            Option(PlayerDialogueOption.Leave, EndDialogue));
    }

    private void ShowClue()
    {
        if (stage != Stage.Greeting)
        {
            return;
        }

        stage = Stage.Clue;

        ui.ShowNPCLine(current.clue,
            Option(PlayerDialogueOption.Thanks, EndDialogue),
            Option(PlayerDialogueOption.ISee, EndDialogue),
            Option(PlayerDialogueOption.HowKind, EndDialogue));
    }

    private void EndDialogue()
    {
        if (stage == Stage.Idle)
        {
            return;
        }

        stage = Stage.Idle;
        current = default;

        if (ui != null)
        {
            ui.EndNPCLine();
        }
    }

    private static InteractEventOption Option(PlayerDialogueOption option, UnityAction onSelected)
    {
        return new InteractEventOption(option.ToLabel(), onSelected);
    }
}
