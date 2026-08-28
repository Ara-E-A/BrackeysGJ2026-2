using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The border inspector. Clicking it opens the shared dialogue window
/// (<see cref="ShowDialogue"/> / <see cref="DBoxControl"/>, gated by
/// <see cref="Interactable.OnInteract"/>):
///
///   greeting -> [Continue] [Leave]
///     Continue -> "present papers" -> [Submit Paper] [Leave]
///       Submit -> PaperEvaluator checks PlayerPaper vs PaperReq
///         all met   -> "in order" line, [Leave], GameEvaluationManager.RegisterSuccess() (Victory)
///         some unmet -> lists them, [Leave] only, GameEvaluationManager.RegisterFailure()
///                       (a failed attempt; 0 remaining -> Game Over)
/// </summary>
public class Inspector : Interactable
{
    [TextArea]
    [SerializeField] private string greeting = "Papers, please.";
    [TextArea]
    [SerializeField] private string prompt = "Present your papers when you are ready.";
    [TextArea]
    [SerializeField] private string passLine = "Everything is in order. Welcome through.";

    private enum Stage { Idle, Greeting, Prompt, Result }
    private Stage stage = Stage.Idle;
    private ShowDialogue ui;

    protected override void Interact()
    {
        ui = FindAnyObjectByType<ShowDialogue>();
        if (ui == null)
        {
            Debug.LogError("Inspector: no ShowDialogue in the scene.");
            return;
        }

        stage = Stage.Greeting;
        ui.ShowNPCLine(greeting,
            Choice(PlayerDialogueOption.Continue, ShowPrompt),
            Choice(PlayerDialogueOption.Leave, End));
    }

    private void ShowPrompt()
    {
        if (stage != Stage.Greeting)
        {
            return;
        }

        stage = Stage.Prompt;
        ui.ShowNPCLine(prompt,
            Choice(PlayerDialogueOption.Submit, Submit),
            Choice(PlayerDialogueOption.Leave, End));
    }

    private void Submit()
    {
        if (stage != Stage.Prompt)
        {
            return;
        }

        stage = Stage.Result;

        if (GameEvaluationManager.Instance.Finished)
        {
            ui.ShowNPCLine("We are done here.", Choice(PlayerDialogueOption.Leave, End));
            return;
        }

        GameManager game = FindAnyObjectByType<GameManager>();
        PlayerPaper paper = game != null ? game.playerPaper : null;
        PaperReq req = game != null ? game.req : null;

        List<string> unmet = PaperEvaluator.FindUnmet(paper, req);

        if (unmet.Count == 0)
        {
            GameEvaluationManager.Instance.RegisterSuccess();
            ui.ShowNPCLine(passLine, Choice(PlayerDialogueOption.Leave, End));
            return;
        }

        GameEvaluationManager.Instance.RegisterFailure();
        int left = GameEvaluationManager.Instance.AttemptsRemaining;

        string tail = left > 0
            ? $"\n\nAttempts remaining: {left}."
            : "\n\nThat was your last attempt.";

        string body = "These do not check out:\n- " + string.Join("\n- ", unmet) + tail;
        ui.ShowNPCLine(body, Choice(PlayerDialogueOption.Leave, End));
    }

    private void End()
    {
        if (stage == Stage.Idle)
        {
            return;
        }

        stage = Stage.Idle;
        ui.EndNPCLine();
    }

    private InteractEventOption Choice(PlayerDialogueOption option, UnityAction onSelected)
    {
        return new InteractEventOption(option.ToLabel(), onSelected);
    }
}
