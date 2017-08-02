using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForieroEngine.MIDIUnified;

public class playMusic : MonoBehaviour {
	
	Rigidbody rb;
	public int midiIndex = 60;
	public float impulseForce = 5f;

	void Awake ()
	{
		MidiOut.ShortMessageEvent += ShortMessage;
		rb = GetComponent<Rigidbody> ();
	}

	void OnDestroy ()
	{
		MidiOut.ShortMessageEvent -= ShortMessage;
	}


	// Use this for initialization
	void Start () {
		
	}

	public void OnMouseDown ()
	{
		GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 1, 0));
		MidiOut.NoteOn (midiIndex, 127, 0);
	}
	// Update is called once per frame
	void Update () {


	}

	void ShortMessage (int Command, int Data1, int Data2)
	{
		if (Command.ToMidiCommand () == 144 && Data1 == midiIndex) {
			rb.AddForce (new Vector3 (0, impulseForce, 0), ForceMode.Impulse);
		}

		if (Command.ToMidiCommand () == 128 && Data1 == midiIndex) {

		}
	}
}
