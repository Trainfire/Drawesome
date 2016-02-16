using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;

public class PlayerList
{
    UiPlayerList View { get; set; }
    Client Client { get; set; }
    Dictionary<PlayerData, UiPlayerListItem> Players { get; set; }

    public PlayerList(Client client, UiPlayerList view)
    {
        Client = client;
        View = view;
        Players = new Dictionary<PlayerData, UiPlayerListItem>();
    }

    public void SetTick(PlayerData player, bool enabled)
    {
        if (Players.ContainsKey(player))
        {
            Players[player].Tick.gameObject.SetActive(enabled);
        }
    }

    public void AddPlayer(PlayerData player)
    {
        if (!Players.ContainsKey(player))
        {
            Players.Add(player, View.AddPlayer(player));
        }
    }

    public void RemovePlayer(PlayerData player)
    {
        if (Players.ContainsKey(player))
        {
            View.RemovePlayer(Players[player]);
            Players.Remove(player);
        }
    }

    public void ClearTicks()
    {
        foreach (var player in Players)
        {
            player.Value.Tick.SetActive(false);
        }
    }

    public void Clear()
    {
        foreach (var player in Players)
        {
            View.RemovePlayer(player.Value);
        }

        Players.Clear();
    }
}
