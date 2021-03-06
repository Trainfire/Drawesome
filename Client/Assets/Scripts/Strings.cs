// TODO: Make JSON system maybe
public static class Strings
{
    public const string PlayersOwnDrawing = "This is your drawing. I hope you're proud of it.";
    public const string AnswerMatchesPrompt = "That's the right answer! Quick! Think of something else!";
    public const string AnswerMatchesExisting = "Somebody already entered that. Try again!";
    public const string AnswerEmpty = "You must enter something!";
    public const string AnswerSubmitted = "Answer submitted! Waiting for other players...";
    public const string ChoiceSubmitted = "Choice submitted! Waiting for other players...";
    public const string PromptEnterGuess = "Enter your guess here...";
    public const string WaitingForPlayers = "Waiting for {0} more players...";
    public const string WaitingForRoomOwner = "Waiting for room owner: {0}...";
    public const string StartGame = "You're the room owner. Click Start to begin";
    public const string DrawingSubmitted = "Drawing submitted! Waiting for other players...";
    public const string TimesUp = "time's up!";
    public const string AllPlayersDone = "everyone's finished";
    public const string StateEndDrawing = "let's see what you've drawn...";
    public const string StateEndAnswering = "the answers are in";
    public const string StateEndChoosing = "what did everybody pick?";
    public const string Cancel = "Cancel";
    public const string CountdownStart = "{0} has started the game";
    public const string CountdownCancel = "{0} has cancelled the countdown";
    public const string Winner = "winner!";

    public static class Result
    {
        public const string ActualAnswer = "the actual answer!";
        public const string Decoy = "decoy!";
        public const string PlayerAnswer = "{0}'s guess!";
    }

    public static class Login
    {
        public const string NameCharacterLimit = "Name invalid. Character limit is 3 - 24 characters";
        public const string NameMatchesExisting = "Someone already has that name";
        public const string ProtocolMismatch = "Client out-of-date.\nTry a hard refresh (CTRL + F5).";
    }

    public static class Popups
    {
        public const string ConnectionError = "There was a problem connecting to the server.";
        public const string InvalidPassword = "The password you entered was invalid";
        public const string RoomFull = "The room you joined is full";
        public const string RoomDoesNotExist = "The room you joined no longer exists";
        public const string GameAlreadyStarted = "The room's game has already started";
        public const string RoomLimitReached = "The maximum number of rooms has been reached";
        public const string ConfirmLeave = "Are you sure you want to leave?";
        public const string KickedForAfk = "You were removed from the room for being AFK";
    }
}
