using Protocol;

public static class StringFormatter
{
    public static string FormatPlayerAction(ServerMessage.NotifyPlayerAction message, PlayerData player)
    {
        // Determine if message is about self.
        string owner = message.Player.ID == player.ID ? "You" : message.Player.Name;
        bool isAboutSelf = message.Player.ID == player.ID;

        switch (message.Action)
        {
            case PlayerAction.None:
                break;
            case PlayerAction.Connected:
                return string.Format("{0} connected.", owner);
            case PlayerAction.Disconnected:
                return string.Format("{0} disconnected.", owner);
            case PlayerAction.Kicked:
                break;
            case PlayerAction.Joined:
                return string.Format("{0} joined the room.", owner);
            case PlayerAction.Left:
                return string.Format("{0} left.", owner);
            case PlayerAction.PromotedToOwner:
                if (isAboutSelf)
                {
                    return string.Format("{0} are now the room owner.", owner);
                }
                else
                {
                    return string.Format("{0} is now the room owner.", owner);
                }
            default:
                break;
        }

        return "";
    }
}
