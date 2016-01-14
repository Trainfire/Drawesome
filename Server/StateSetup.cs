using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Protocol;

namespace Server
{
    public class StateSetup : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<Message>(e.Data);

            Console.WriteLine("Recieved message of type: " + message.Type);
            Console.WriteLine("Data: " + e.Data);

            if (message.Type == MessageType.PlayerReady)
            {
                var data = message.Deserialise<PlayerReadyMessage>(e.Data);
                Console.Write("Is Ready? " + data.IsReady);
            }
        }
    }
}