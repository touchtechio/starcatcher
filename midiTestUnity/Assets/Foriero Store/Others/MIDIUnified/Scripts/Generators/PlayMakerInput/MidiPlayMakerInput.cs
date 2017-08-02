/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

[AddComponentMenu("MIDIUnified/Generators/MidiPlayMakerInput")]
public class MidiPlayMakerInput : MonoBehaviour, IMidiEvents {
			
	public event ShortMessageEventHandler ShortMessageEvent;
	ShortMessageEventHandler shortMessageEventHandler;
	
	public static  MidiPlayMakerInput singleton;
	
	public bool midiOut = false;
	public ChannelEnum midiChannel = ChannelEnum.None;
	public bool useCustomVolume = false;
	public float customVolume = 1f;
	
	MidiOutHelper midiOutHelper = new MidiOutHelper();
			
	void Awake(){
		if(singleton != null){
			Debug.LogError("GENERATOR : MidiPlayMakerInput already in scene.");
			Destroy(this);
			return;
		}
		shortMessageEventHandler = new ShortMessageEventHandler(ShortMessageHelper);
		midiOutHelper.ShortMessageEvent += shortMessageEventHandler;
		singleton = this;
	}
			
	void OnDestroy(){
		singleton = null;
	}
			
	public ChannelEnum GetMidiChannel(){
		return midiChannel == ChannelEnum.None ? ChannelEnum.C0 : midiChannel;
	}
	
	public  void SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel){
		if(midiOut) {
			midiOutHelper.SetInstrument(anInstrument, GetMidiChannel());
		}
	}
	
	public  void SetInstrument(int anInstrument){
		if(midiOut) {
			 midiOutHelper.SetInstrument(anInstrument, (int)GetMidiChannel());
		}
		
		
	}
	
	public  void NoteOn(int aNoteIndex, int aVolume){
		if(midiOut) {
			 midiOutHelper.NoteOn(aNoteIndex, aVolume,(int)GetMidiChannel());
		}
		
	}
	
	public  void NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int aVolume){
		if(midiOut) {
			 midiOutHelper.NoteOn(aNote, anAccidental, anOctave, aVolume, GetMidiChannel());
		}
	}
	
	public  void NoteOff(int aNoteIndex){
		if(midiOut) {
			 midiOutHelper.NoteOff(aNoteIndex);
		}
		
	}
	
	public  void NoteOff(NoteEnum aNote, AccidentalEnum anAccidental,OctaveEnum anOctave){
		if(midiOut) {
			 midiOutHelper.NoteOff(aNote, anAccidental, anOctave, GetMidiChannel());
		}
		
	}
	
	public  void Pedal(int aPedal, int aValue){
		if(midiOut) {
			 midiOutHelper.Pedal(aPedal, aValue, (int)GetMidiChannel());
		}
		
	}
	
	public  void Pedal(PedalEnum aPedal, int aValue){
		if(midiOut) {
			 midiOutHelper.Pedal(aPedal, aValue, GetMidiChannel());
		}
		
	}
	
	public  void SendControl(ControllerEnum aControl, int aValue){
		if(midiOut) {
			 midiOutHelper.SendControl(aControl, aValue, GetMidiChannel());
		}
		
	}
	
	public  void SendControl(int aControl, int aValue){
		if(midiOut) {
			 midiOutHelper.SendControl(aControl, aValue, (int)GetMidiChannel());
		}
		
	}
	
	public  void AllSoundOff(){
		if(midiOut) {
			 midiOutHelper.AllSoundOff();
		}
		 
	}
	
	public  void ResetAllControllers(){
		if(midiOut) {
			 midiOutHelper.ResetAllControllers();
		}
		
	}
	
	void ShortMessageHelper(int aCommand, int aData1, int aData2){
		SendShortMessage(aCommand, aData1, aData2);	
	}
	
	public void SendShortMessage(int aCommand, int aData1, int aData2){
		if(ShortMessageEvent != null) {
			ShortMessageEvent(
				midiChannel == ChannelEnum.None ? aCommand : (aCommand | (int)midiChannel),
				aData1,
				useCustomVolume ? MidiConversion.GetByteVolume(customVolume, aData2) : aData2
			);
		}
		
		if(midiOut){
			MidiOut.SendShortMessage(
				midiChannel == ChannelEnum.None ? aCommand : (aCommand | (int)midiChannel),
				aData1,
				useCustomVolume ? MidiConversion.GetByteVolume(customVolume, aData2) : aData2
			);
		}
	}
	
	 class MidiOutHelper{
		
		public event ShortMessageEventHandler ShortMessageEvent;
		
		public  void SetInstrument(ProgramEnum anInstrument, ChannelEnum aChannel = ChannelEnum.C0){
			SendShortMessage((int)aChannel + (int)CommandEnum.ProgramChange,(int)anInstrument,0);
		}
		
		public void SetInstrument(int anInstrument, int aChannel = 0){
			SendShortMessage(aChannel + (int)CommandEnum.ProgramChange,anInstrument,0);
		}
		
		public  void NoteOn(int aNoteIndex, int aVolume = 80,  int aChannel = 0){
			SendShortMessage(aChannel + (int)CommandEnum.NoteOn, aNoteIndex, aVolume);	
		}
		
		public  void NoteOn(NoteEnum aNote, AccidentalEnum anAccidental, OctaveEnum anOctave, int aVolume = 80, ChannelEnum aChannel = ChannelEnum.C0){
			int noteIndex	= ((int)aNote + (int)anAccidental + ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
			NoteOn(noteIndex, aVolume, (int)aChannel);
		}
		
		public  void NoteOff(int aNoteIndex,  int aChannel = 0){
			SendShortMessage(aChannel + (int)CommandEnum.NoteOff, aNoteIndex, 0);
		}
		
		public  void NoteOff(NoteEnum aNote, AccidentalEnum anAccidental,OctaveEnum anOctave, ChannelEnum aChannel = ChannelEnum.C0){
			int noteIndex	= ((int)aNote + (int)anAccidental+ ((int)(anOctave == OctaveEnum.None ? OctaveEnum.Octave4 : anOctave) * 12) + 24);
			NoteOff(noteIndex, (int)aChannel);
		}
		
		public  void Pedal(int aPedal, int aValue,  int aChannel = 0){
			SendShortMessage(aChannel + (int)CommandEnum.Controller, aPedal, aValue);
		}
		
		public  void Pedal(PedalEnum aPedal, int aValue, ChannelEnum aChannel = ChannelEnum.C0){
			SendShortMessage((int)aChannel + (int)CommandEnum.Controller, (int)aPedal, aValue);
		}
		
		public  void SendControl(ControllerEnum aControl, int aValue, ChannelEnum aChannel = ChannelEnum.C0){
			SendShortMessage((int)aChannel + (int)CommandEnum.Controller, (int)aControl, aValue);
		}
		
		public  void SendControl(int aControl, int aValue,  int aChannel = 0){
			SendShortMessage(aChannel + (int)CommandEnum.Controller, aControl, aValue);
		}
		
		public  void AllSoundOff(){
			for(int i = 0; i < 16; i++){
				 SendShortMessage(i + (int)CommandEnum.Controller, (int)ControllerEnum.AllSoundOff,0);
			}
		}
		
		public  void ResetAllControllers(){
			for(int i = 0; i < 16; i++){
				SendShortMessage(i +(int)CommandEnum.Controller, (int)ControllerEnum.ResetControllers,0);
			}
		}
		
		public void SendShortMessage(int aCommand, int aData1, int aData2){
			if(ShortMessageEvent != null) ShortMessageEvent(aCommand, aData1, aData2);
		}
	}
}
