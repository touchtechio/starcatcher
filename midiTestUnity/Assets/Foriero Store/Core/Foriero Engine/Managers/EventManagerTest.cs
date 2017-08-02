using UnityEngine;
using System.Collections;

public class EventManagerTest : MonoBehaviour
{


	// Use this for initialization
	void Start ()
	{
		EventManager.AddListener<EventManager.Test> (B);
	}

	void B (EventManager.Test t)
	{
		Debug.Log ("WOW : " + t.i);
	}

	void OnDestroy ()
	{
		EventManager.RemoveListener<EventManager.Test> (B);
	}
}
