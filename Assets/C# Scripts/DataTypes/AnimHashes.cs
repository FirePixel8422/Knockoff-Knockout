


/// <summary>
/// Static class acting as a container, holding all fighter animation hashes (not attacks though)
/// </summary>
public static class AnimHashes
{
    public static class Hurt
    {
        public static readonly AnimData StandingLow = new AnimData("StandingLowHurt", 0, 5);
        public static readonly AnimData StandingHigh = new AnimData("StandingHighHurt", 0, 5);

        public static readonly AnimData CrouchingLow = new AnimData("CrouchingLowHurt", 0, 5);
        public static readonly AnimData CrouchingHigh = new AnimData("CrouchingHighHurt", 0, 5);
    }

    public static class Block
    {
        public static readonly AnimData Standing = new AnimData("StandingBlock", 0, 5);
        public static readonly AnimData Crouching = new AnimData("CrouchingBlock", 0, 5);
    }

    public static class Movement
    {
        public static readonly AnimData Crouching = new AnimData("Crouching", 3, 3);

        public static readonly AnimData Idle = new AnimData("Idle", 3, 3);

        public static readonly AnimData Retreat = new AnimData("Retreat", 3, 3);
        public static readonly AnimData Push = new AnimData("Push", 3, 3);

        public static readonly AnimData Dash = new AnimData("Dash", 3, 3);
        public static readonly AnimData SideStep = new AnimData("SideStep", 3, 3);
    }

    public static readonly AnimData Missing = new AnimData("Missing", 0, 0);
}

