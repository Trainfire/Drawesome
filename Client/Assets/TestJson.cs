using UnityEngine;
using Protocol;

public class TestJson : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var message = new Message();
        message.DataType = MessageDataType.Binary;
        var json = JsonHelper.ToJson(message);
        Debug.Log(json);

        var obj = JsonHelper.FromJson<Message>(json);
        Debug.Log(obj.DataType);
    }
}
