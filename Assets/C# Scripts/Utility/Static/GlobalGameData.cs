


/// <summary>
/// static class that holds settings that are unchangable by players playing the game (Constants "const")
/// </summary>
public static class GlobalGameData
{
    public const int MAX_PLAYERS = 2;

    public const bool LOG_FILE_OPERATIONS = false;
    public const string DEBUG_LOGGER_SRC = "Enable_Debug_Systems";

    public const float TICK_TIME = 1f / 60;
    public const int MAX_TICK_CATCH_UP = 5;

    public const int INPUT_BUFFER_SIZE = 8;
    public const int DIRECTION_BUFFER_WINDOW = 2;
}
