using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AudioSynthesis.Bank;
using AudioSynthesis.Sequencer;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Midi;

[RequireComponent (typeof(AudioSource))]
public class CSharpSynth: MonoBehaviour
{
	public static CSharpSynth singleton;

	public float gain = 5f;

	static int sampleRate = 22050;
	static int polyphony = 64;

	AudioSource audioSource = null;

	Synthesizer midiSynthesizer;
	//float[] sampleBuffer = new float[0];
	int channels = 0;
	int bufferLength = 0;
	int numBuffers = 0;
	readonly string soundBank = "softsynth.sf2";
	static volatile bool playEnabled = false;

	public static bool Init (int sampleRate = 22050, int polyphony = 64)
	{
		if (singleton) {
			Debug.LogError ("C# Synth already in scene!");
		}

		CSharpSynth.sampleRate = sampleRate;
		CSharpSynth.polyphony = polyphony;

		GameObject go = new GameObject ("CSharpSynth");
		singleton = go.AddComponent<CSharpSynth> ();

		playEnabled = singleton.InitSynth ();
		return playEnabled;
	}

	public static void ShortMessage (byte Command, byte Data1, byte Data2)
	{
		if (singleton) {
			singleton.midiSynthesizer.ProcessMidiMessage (Command.ToMidiChannel (), Command.ToMidiCommand (), Data1, Data2);	
		}
	}

	public static void AllSoundOff ()
	{
		if (singleton) {	
			singleton.midiSynthesizer.NoteOffAll (true);
			singleton.midiSynthesizer.ResetSynthControls ();
			singleton.midiSynthesizer.ResetPrograms ();
		}
	}

	void OnDestroy ()
	{
		playEnabled = false;
	}

	void OnDisable ()
	{
		playEnabled = false;
	}

	void OnEnable ()
	{
		if (midiSynthesizer != null) {
			playEnabled = true;
		}
	}

	byte[] LoadBank (string filename)
	{
		TextAsset asset = Resources.Load<TextAsset> (filename);
		if (asset != null) {
			return asset.bytes;
		}

		Debug.LogError (string.Format ("DaggerfallSongPlayer: Bank file '{0}' not found.", filename));

		return null;
	}

	public class MFile : AudioSynthesis.IResource
	{
		private byte[] file;
		private string fileName;

		public MFile (byte[] file, string fileName)
		{
			this.file = file;
			this.fileName = fileName;
		}

		public string GetName ()
		{
			return fileName;
		}

		public bool DeleteAllowed ()
		{
			return false;
		}

		public bool ReadAllowed ()
		{
			return true;
		}

		public bool WriteAllowed ()
		{
			return false;
		}

		public void DeleteResource ()
		{
			return;
		}

		public Stream OpenResourceForRead ()
		{
			return new MemoryStream (file);
		}

		public Stream OpenResourceForWrite ()
		{
			return null;
		}
	}

	bool InitSynth ()
	{
		// Get peer AudioSource
		audioSource = GetComponent<AudioSource> ();
		if (audioSource == null) {
			Debug.LogError ("DaggerfallSongPlayer: Could not find AudioSource component.");
			return false;
		}

		// Create synthesizer and load bank
		if (midiSynthesizer == null) {
			// Get number of channels
			if (AudioSettings.driverCapabilities.ToString () == "Mono")
				channels = 1;
			else
				channels = 2;

			// Create synth
			AudioSettings.GetDSPBufferSize (out bufferLength, out numBuffers);
			midiSynthesizer = new Synthesizer (sampleRate, channels, bufferLength / numBuffers, numBuffers, polyphony);

			// Load bank data
			byte[] bankData = LoadBank (soundBank);
			if (bankData == null)
				return false;
			else {
				midiSynthesizer.LoadBank (new MFile (bankData, soundBank));
				midiSynthesizer.ResetSynthControls (); // Need to do this for bank to load properly, don't know why
			}
		}

		return true;
	}

	int bufferHead;
	float[] currentBuffer;

	// Called when audio filter needs more sound data
	void OnAudioFilterRead (float[] data, int channels)
	{
		// Do nothing if play not enabled
		// This flag is raised/lowered when user starts/stops play
		// Helps avoids thread finding synth in state of shutting down
		if (!playEnabled)
			return;
		
		int count = 0;
		while (count < data.Length) {
			if (currentBuffer == null || bufferHead >= currentBuffer.Length) {
				midiSynthesizer.GetNext ();
				currentBuffer = midiSynthesizer.WorkingBuffer;
				bufferHead = 0;
			}
			var length = Mathf.Min (currentBuffer.Length - bufferHead, data.Length - count);
			System.Array.Copy (currentBuffer, bufferHead, data, count, length);
			bufferHead += length;
			count += length;
		}
	}
}
