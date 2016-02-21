using UnityEngine;
using Protocol;

public class TestJson : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var message = new ClientMessage.GiveName("SomeName");
        var json = JsonHelper.ToJson(message);
    }
}
