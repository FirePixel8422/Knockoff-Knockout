


/// <summary>
/// Static class acting as a container, holding all fighter animation hashes (not attacks though)
/// </summary>
public static class GlobalAnimHashes
{
    public static class Block
    {
        public static readonly AnimData Standing = new AnimData("StandingBlock", true, 0, 5);
        public static readonly AnimData Crouching = new AnimData("CrouchingBlock", true, 0, 5);
    }

    public static class Hurt
    {
        public static class Standing
        {
            public static readonly AnimData Low = new AnimData("StandingLowHurt", true, 0, 5);
            public static readonly AnimData MidHigh = new AnimData("StandingHighHurt", true, 0, 5);
        }
        public static class Crouching
        {
            public static readonly AnimData Low = new AnimData("CrouchingLowHurt", true, 0, 5);
            public static readonly AnimData MidHigh = new AnimData("CrouchingHighHurt", true, 0, 5);
        }
        public static class KnockedDown
        {
            public static readonly AnimData Low = new AnimData("KnockedDownLowHurt", true, 3, 5);
            public static readonly AnimData MidHigh = new AnimData("KnockedDownHighHurt", true, 3, 5);
        }
    }

    public static class KnockDown
    {
        public static readonly AnimData Low = new AnimData("LowKnockDown", true, 2, 5);
        public static readonly AnimData Mid = new AnimData("MidKnockDown", true, 2, 5);
        public static readonly AnimData High = new AnimData("HighKnockDown", true, 2, 5);
    }

    public static class Movement
    {
        public static readonly AnimData Crouching = new AnimData("Crouching", false, 6, 5);

        public static readonly AnimData Idle = new AnimData("Idle", false, 15, 5);

        public static readonly AnimData Retreat = new AnimData("Retreat", false, 3, 7);
        public static readonly AnimData Push = new AnimData("Push", false, 3, 10);

        public static class Dash
        {
            public static readonly AnimData Forward = new AnimData("DashForward", true, 3, 5);
            public static readonly AnimData Back = new AnimData("DashBack", true, 3, 5);
        }

        public static class SideStep
        {
            public static readonly AnimData Up = new AnimData("SideStepUp", true, 2, 5);
            public static readonly AnimData Down = new AnimData("SideStepDown", true, 2, 5);
        }
    }

    public static readonly AnimData Missing = new AnimData("Missing", false, 0, 0);
}