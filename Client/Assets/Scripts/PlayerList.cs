using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using System;

public class PlayerList : Game.IGameStateHandler, Game.IGameMessageHandler
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

    void SetTick(PlayerData player, bool enabled)
    {
        if (Players.ContainsKey(player))
        {
            Players[player].Tick.gameObject.SetActive(enabled);
        }
    }

    void AddPlayer(PlayerData player)
    {
        if (!Players.ContainsKey(player))
        {
            Players.Add(player, View.AddPlayer(player));
        }
    }

    void RemovePlayer(PlayerData player)
    {
        if (Players.ContainsKey(player))
        {
            View.RemovePlayer(Players[player]);
            Players.Remove(player);
        }
    }

    void ClearTicks()
    {
        foreach (var player in Players)
        {
            player.Value.Tick.SetActive(false);
        }
    }

    void Clear()
    {
        foreach (var player in Players)
        {
            View.RemovePlayer(player.Value);
        }

        Players.Clear();
    }

    void Game.IGameStateHandler.HandleState(GameState state)
    {
        ClearTicks();

        switch (state)
        {
            case GameState.PreGame:
                View.Show();
                break;
            case GameState.Answering:
                View.Show();
                break;
            case GameState.Results:
                View.Hide();
                break;
        }
    }

    void Game.IGameMessageHandler.HandleMessage(string json)
    {
        // Update player list
        Message.IsType<ServerMessage.RoomUpdate>(json, (data) =>
        {
            Clear();
            data.RoomData.Players.ForEach(x => AddPlayer(x));
        });

        // Set tick on player action such as submit drawing, answering and choosing
        Message.IsType<ServerMessage.Game.PlayerAction>(json, (data) =>
        {
            SetTick(data.Actor, true);
        });
    }
}
