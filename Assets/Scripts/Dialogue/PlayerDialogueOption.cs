public enum PlayerDialogueOption
{
    Continue,
    Leave,
    Thanks,
    ISee,
    HowKind,
    Submit
}

public static class PlayerDialogueOptionExtensions
{
    public static string ToLabel(this PlayerDialogueOption option)
    {
        switch (option)
        {
            case PlayerDialogueOption.ISee: return "I see";
            case PlayerDialogueOption.HowKind: return "How kind of you";
            case PlayerDialogueOption.Submit: return "Submit Paper";
            default: return option.ToString();
        }
    }
}
