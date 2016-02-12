using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour
{
    protected Client Client { get; private set; }

    public virtual void Initialise(Client client)
    {
        Client = client;
    }
}

public interface IClientProxy
{
    void Initialise(Client client);
}
