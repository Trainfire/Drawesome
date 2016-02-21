using UnityEngine;
using System.Collections;

public class TestPreGame : MonoBehaviour
{
    UiGameStatePreGame view;

	// Use this for initialization
	void Start ()
    {
        view = GetComponent<UiGameStatePreGame>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyUp(KeyCode.Space))
        {
            view.SetCountdown(5f);
        }

        if (Input.GetKeyUp(KeyCode.E))
            view.CancelCountdown();
	}
}
