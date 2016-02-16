using UnityEngine;
using System.Collections.Generic;
using Protocol;
using System.Linq;

public class UiPlayerList : UiBase
{
    public UiPlayerListItem Prototype;
    public GameObject List;

    void Awake()
    {
        Prototype.gameObject.SetActive(false);
    }

    public UiPlayerListItem AddPlayer(PlayerData player)
    {
        var instance = UiUtility.AddChild(List, Prototype, true);
        instance.Tick.SetActive(false);
        instance.Name.text = player.Name;
        return instance;
    }

    public void RemovePlayer(UiPlayerListItem player)
    {
        Destroy(player.gameObject);
    }
}
