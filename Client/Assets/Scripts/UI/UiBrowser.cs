using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiBrowser : UiMenu
{
    public Button Create;
    public Button Refresh;

    const float refreshInterval = 1f;
    float refreshTimeStamp = 0f;

    void Start()
    {
        Create.onClick.AddListener(() => Client.Instance.CreateRoom());
        Refresh.onClick.AddListener(() => OnRefresh());
    }

    void Update()
    {
        Refresh.interactable = Time.realtimeSinceStartup > (refreshTimeStamp + refreshInterval);
    }

    void OnRefresh()
    {
        refreshTimeStamp = Time.realtimeSinceStartup;
        Client.Instance.RequestRooms();
    }
}
