using UnityEngine;
using System.Collections;
using ForieroEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public partial class MIDISettings : ScriptableObject
{
#if UNITY_EDITOR
	[MenuItem ("Foriero/Settings/MIDI")]
	public static void MIDISettingsMenu ()
	{
		MIDISettings s = MIDISettings.instance;
		EditorGUIUtility.PingObject (s);
		Selection.objects = new Object [1] { s };
	}
#endif

	static MIDISettings _instance;

	public static MIDISettings instance {
		get {
			if (_instance == null) {
				_instance = FResources.Instance<MIDISettings> (typeof (MIDISettings).Name, "Settings");
				_instance.linux.synthesizer = Synth.SynthEnum.BASS24;
				_instance.win.synthesizer = Synth.SynthEnum.BASS24;
				_instance.osx.synthesizer = Synth.SynthEnum.BASS24;
			}

			return _instance;
		}
	}

	public bool debug = false;

	[Header ("Initialize MIDI?")]
	public bool initialize = true;
	[Header ("Platform Synthesizers")]
	public Synth.SynthSettingsANDROID android;
	public Synth.SynthSettingsIOS ios;
	public Synth.SynthSettingsOSX osx;
	public Synth.SynthSettingsLINUX linux;
	public Synth.SynthSettingsWIN win;
	public Synth.SynthSettingsWSA wsa;
	public Synth.SynthSettingsWEBGL webgl;

	[Header ("Midi IN")]
	public bool forceDefaultMidiIn = false;
	public int defaultMidiIn = 1;
	[Header ("Midi OUT")]
	public bool forceDefaultMidiOut = false;
	public int defaultMidiOut = 1;
	[HideInInspector]
	public int synthChannelMask = -1;
	[HideInInspector]
	public int channelMask = -1;

	[Tooltip ("BASS NET Username")]
	[HideInInspector]
	public string userName;
	[Tooltip ("BASS NET Password")]
	[HideInInspector]
	public string password;

	public Synth.SynthSettings GetPlatformSettings ()
	{
#if UNITY_EDITOR_OSX
		return osx;
#elif UNITY_EDITOR_WIN
		return win;
#elif UNITY_STANDALONE_OSX
		return osx;
#elif UNITY_STANDALONE_WIN
		return win;
#elif UNITY_STANDALONE_LINUX
		return linux;
#elif UNITY_IOS
		return ios;
#elif UNITY_WSA
		return wsa;
#elif UNITY_WEBGL
		return webgl;
#elif UNITY_ANDROID
		return android;
#else
		return null;
#endif
	}
}
