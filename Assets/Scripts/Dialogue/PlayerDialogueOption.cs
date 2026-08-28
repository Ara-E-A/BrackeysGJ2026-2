public enum PlayerDialogueOption
{
    Continue,
    Leave,
    Thanks,
    ISee,
    HowKind
}

public static class PlayerDialogueOptionExtensions
{
    public static string ToLabel(this PlayerDialogueOption option)
    {
        switch (option)
        {
            case PlayerDialogueOption.ISee: return "I see";
            case PlayerDialogueOption.HowKind: return "How kind of you";
            default: return option.ToString();
        }
    }
}
