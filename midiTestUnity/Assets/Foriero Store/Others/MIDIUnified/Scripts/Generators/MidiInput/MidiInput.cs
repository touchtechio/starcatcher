/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System;
using System.Collections;
using ForieroEngine.MIDIUnified;

[AddComponentMenu ("MIDIUnified/Generators/MidiInput")]
public class MidiInput : MonoBehaviour, IMidiEvents
{

	public delegate void MidiMessageDelegate (MidiMessage aMidiMessage);

	public event MidiMessageDelegate SysExMessageEvent;
	public event ShortMessageEventHandler ShortMessageEvent;

	public static MidiInput singleton;

	public bool useCustomVolume = false;

	[Tooltip ("This value overrides volume data so you won't be able to hear pressed key dynamics.")]
	[Range (0, 1)]
	public float customVolume = 1f;
	[Tooltip ("This value multiplies volume data to make it softer or louder.")]
	[Range (0, 10)]
	public float multiplyVolume = 1f;
	public bool midiOut = false;
	[Tooltip ("This will set thread for polling midi messages from your Midi IN devices. If you encounter any instability please set 'through' to false.")]
	public bool midiThrough = false;
	public ChannelEnum midiChannel = ChannelEnum.None;

	public bool cleanBuffer = true;

	void Awake ()
	{
		if (singleton != null) {
			Debug.LogError ("GENERATOR : MidiInput already in scene");
			Destroy (this);
			return;
		} 
		singleton = this;
		MIDI.through = midiThrough;
	}

	void OnDestroy ()
	{
		singleton = null;
	}

	void Update ()
	{
		if (!midiThrough) {
			MIDI.Fetch ();
		}
		ProcessMidiInMessages ();
	}

	int volume = 0;
	int command = 0;

	public void ProcessMidiMessage (MidiMessage midiMessage)
	{
		if (midiMessage.dataSize == 3 && midiMessage.command != 0xF0) {
			if (midiMessage.command.ToMidiCommand () == 0x90 && midiMessage.data2 == 0) {
				midiMessage.command = (byte)(midiMessage.command.ToMidiChannel () + 0x80);
			}

			volume = 0;
			if (useCustomVolume) {
				volume = MidiConversion.GetMidiVolume (customVolume);
			} else {
				volume = (int)Mathf.Clamp (multiplyVolume * midiMessage.data2, 0, 127); 
			}

			command = 0;
			if (midiChannel == ChannelEnum.None) {
				command = midiMessage.command;
			} else {
				command = (int)midiChannel + midiMessage.command.ToMidiCommand ();
			}

			if (midiOut) {
				MidiOut.SendShortMessage (command, midiMessage.data1, volume, midiThrough);
			}

			if (ShortMessageEvent != null) {
				ShortMessageEvent (command, midiMessage.data1, volume);
			}
		} else {
			// SYS EX MESSAGE //
			if (midiMessage.command == 0xF0) {
				if (SysExMessageEvent != null)
					SysExMessageEvent (midiMessage);
			}
		}
	}

	void ProcessMidiInMessages ()
	{
		MidiMessage midiMessage = new MidiMessage ();

		while (MIDI.midiInMessages.Count > 0) {
			if (MIDI.midiInMessages.TryDequeue (out midiMessage)) {
				if (!cleanBuffer) {
					ProcessMidiMessage (midiMessage);
				}
			}
		}
		cleanBuffer = false;
	}
}
