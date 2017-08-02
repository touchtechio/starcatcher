using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public static class Synth
{
	public enum SynthEnum
	{
		NONE = 0,
		NATIVE = 1,
		BASS24 = 2,
		CSHARP = 3
	}

	public static SynthSettings settings;

	public static int Start ()
	{
		if (settings == null) {
			if (MIDISettings.instance.debug) {
				Debug.LogError ("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
			}
			return -1;
		}
		int result = -1;
		switch (settings.synthesizer) {
		case SynthEnum.NONE:
			break;
		case SynthEnum.NATIVE:
			result = StartNATIVESynth ();
			break;
		case SynthEnum.BASS24:
			result = StartBASS24Synth ();
			PreinitBASS24 ();
			break;
		case SynthEnum.CSHARP:
			result = StartCSHARPSynth ();
			break;
		}
		return result;
	}

	public static int Stop ()
	{
		if (settings == null) {
			if (MIDISettings.instance.debug) {
				Debug.LogError ("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
			}
			return -1;
		}
		int result = -1;
		switch (settings.synthesizer) {
		case SynthEnum.NONE:
			break;
		case SynthEnum.NATIVE:
			result = NATIVESynth.Stop ();
			break;
		case SynthEnum.BASS24:
			result = BASS24Synth.Stop ();
			break;
		case SynthEnum.CSHARP:
			result = CSHARPSynth.Stop ();
			break;
		}
		return result;
	}

	public static int SendMidiMessage (int aChannelCommand, int aData1, int aData2)
	{
		if (settings == null) {
			if (MIDISettings.instance.debug) {
				Debug.LogError ("Settings are NULL. Likely unsupported synth platform or you set not to initialize mid!");
			}
			return -1;
		}
		int result = -1;
		switch (settings.synthesizer) {
		case SynthEnum.NONE:
			break;
		case SynthEnum.NATIVE:
			if (NATIVESynth.use && NATIVESynth.active) {
				result = NATIVESynth.SendMessage (aChannelCommand, aData1, aData2);
			}
			break;
		case SynthEnum.BASS24:
			if (BASS24Synth.use && BASS24Synth.active) {
				result = BASS24Synth.SendMidiMessage (aChannelCommand, aData1, aData2);
			}
			break;
		case SynthEnum.CSHARP:
			if (CSHARPSynth.use && CSHARPSynth.active) {
				result = CSHARPSynth.SendMessage (aChannelCommand, aData1, aData2);
			}
			break;
		}

		return result;
	}

	static int StartBASS24Synth ()
	{
		BASS24Synth.use = true;
		BASS24Synth.active = true;
		Debug.Log ("Starting BASS24Synth");
		return BASS24Synth.Start (settings.frequency, settings.channels);
	}

	static void PreinitBASS24 ()
	{
		Debug.Log ("Preinit BASS24h");
		if (settings.preinit) {
			for (int i = 0; i < settings.channels; i++) {
				for (int k = 0; k < 128; k++) {
					BASS24Synth.SendMidiMessage ((int)CommandEnum.NoteOn + i, k, 1);
					BASS24Synth.SendMidiMessage ((int)CommandEnum.NoteOff + i, k, 0);
				}
			}
		}
	}

	static int StartNATIVESynth ()
	{
		NATIVESynth.use = true;
		NATIVESynth.active = true;
		#if UNITY_IOS
		NATIVESynth.soundBank = (settings as SynthSettingsIOS).soundBank;
		#endif
		Debug.Log ("Starting NATIVESynth");
		return NATIVESynth.Start (settings.frequency, settings.channels);
	}

	static int StartCSHARPSynth ()
	{
		CSHARPSynth.use = true;
		CSHARPSynth.active = true;
		Debug.Log ("Starting C#Synth");
		return CSHARPSynth.Start (settings.frequency, settings.channels);
	}

	[System.Serializable]
	public class SynthSettings
	{
		public Synth.SynthEnum synthesizer = Synth.SynthEnum.NONE;
		public int frequency = 44100;
		public int channels = 16;
		public bool preinit = false;
	}

	[System.Serializable]
	public class SynthSettingsWSA : SynthSettings
	{
		
	}

	[System.Serializable]
	public class SynthSettingsWEBGL : SynthSettings
	{

	}

	[System.Serializable]
	public class SynthSettingsOSX : SynthSettings
	{

	}

	[System.Serializable]
	public class SynthSettingsLINUX : SynthSettings
	{

	}

	[System.Serializable]
	public class SynthSettingsWIN : SynthSettings
	{

	}

	[System.Serializable]
	public class SynthSettingsIOS : SynthSettings
	{
		public enum SoundBankEnum
		{
			sf2 = 0,
			dls = 1,
			aupreset = 2
		}

		public SoundBankEnum soundBank = SoundBankEnum.sf2;
	}

	[System.Serializable]
	public class SynthSettingsANDROID : SynthSettings
	{

	}
}
