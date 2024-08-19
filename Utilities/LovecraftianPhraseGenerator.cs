namespace Boxy_Core.Utilities
{
    public static class LovecraftianPhraseGenerator
    {
        private static readonly Random Random = new();

        private static List<string> OngoingActions { get; } =
        [
            "Deciphering archaic poems",
            "Summoning unknowable horrors",
            "Transcribing grave misdeeds",
            "Performing diabolical prestidigitations",
            "Opening the primordial cradle",
            "Unveiling, by sinister alchemies",
            "Finalizing loathsome changes",
            "Investigating faint miasmal odour",
            "Lo, a detestably sticky noise",
            "Acting on studied malevolence",
            "Avoiding the undulating coils",
            "Malfeasant susurrations rising",
            "Plunging blindly into the abyss",
            "Resisting the madness of the infinite",
            "Devising oily temptations"
        ];

        public static string RandomPhrase()
        {
            int maxVal = OngoingActions.Count;
            var index = (int)(Random.NextDouble() * maxVal);
            return OngoingActions[index];
        }
    }
}
