


/// <summary>
/// Static class acting as a container, holding all fighter animation hashes (not attacks though)
/// </summary>
public static class GlobalAnimHashes
{
    public static class Hurt
    {
        public static readonly AnimData StandingLow = new AnimData("StandingLowHurt", 0, 5);
        public static readonly AnimData StandingHigh = new AnimData("StandingHighHurt", 0, 5);

        public static readonly AnimData CrouchingLow = new AnimData("CrouchingLowHurt", 0, 5);
        public static readonly AnimData CrouchingHigh = new AnimData("CrouchingHighHurt", 0, 5);

        public static readonly AnimData KnockedDownLow = new AnimData("KnockedDownLowHurt", 0, 5);
        public static readonly AnimData KnockedDownHigh = new AnimData("KnockedDownHighHurt", 0, 5);
    }

    public static class Block
    {
        public static readonly AnimData Standing = new AnimData("StandingBlock", 0, 5);
        public static readonly AnimData Crouching = new AnimData("CrouchingBlock", 0, 5);
    }

    public static class Movement
    {
        public static readonly AnimData Crouching = new AnimData("Crouching", 6, 5);

        public static readonly AnimData Idle = new AnimData("Idle", 15, 5);

        public static readonly AnimData Retreat = new AnimData("Retreat", 3, 7);
        public static readonly AnimData Push = new AnimData("Push", 3, 10);

        public static readonly AnimData DashBack = new AnimData("DashBack", 3, 5);
        public static readonly AnimData DashForward = new AnimData("DashForward", 3, 5);

        public static readonly AnimData SideStepDown = new AnimData("SideStepDown", 3, 5);
        public static readonly AnimData SideStepUp = new AnimData("SideStepUp", 3, 5);
    }

    public static readonly AnimData Missing = new AnimData("Missing", 0, 0);
}

