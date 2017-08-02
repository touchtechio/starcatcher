/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;
 
[AddComponentMenu("MIDIUnified/Generators/MidiKeyboardInput")]
public class MidiKeyboardInput : MonoBehaviour, IMidiEvents {
	
		public static MidiKeyboardInput singleton;
	
		public event ShortMessageEventHandler ShortMessageEvent;
	
		public enum KeyboardInputType{
			ABCDEFG,
			QUERTY
		}
	
		public int keyboardOctave = 4;
		public bool updateKeyboardOctave = false;
		public bool muteTonesWhenChangingOctave = false;
		
		[Range (0, 1)]
		public float customVolume = 1f;		
		
		public bool midiOut = false;
		
		public ChannelEnum midiChannel = ChannelEnum.None;
		
		public KeyboardInputType keyboardInputType = KeyboardInputType.QUERTY;
				
		void Update(){
			ProceedKeyboardInput();	
		}
	
		void Awake(){
			if(singleton != null) {
				Debug.Log("GENERATOR MidiKeyboardInput already in scene.");
				Destroy(this);
				return;
			}
			singleton = this;
		}
	
		void OnDestroy(){
			singleton = null;
		}
		
		#region KeyboardInput
		
		
		enum AccidentalState{
			none = 0,
			sharp = 1,
			flat = -1
		}
		
		AccidentalState accidentalState = AccidentalState.none;
		AccidentalState[] accidentalStates = new AccidentalState[13];
		int[] keyOctaveIdx = new int[13];
				
		public void MuteTones(){
			int startIndex = keyboardOctave*12;
			MidiOut.fireMidiOutEvents = false;
			for(int i = startIndex; i<startIndex+13;i++){
				if(i>=0 && i < byte.MaxValue/2){
					SendShortMessage(CommandEnum.NoteOff, i, MidiConversion.GetMidiVolume(customVolume));						
				}
			}
			MidiOut.fireMidiOutEvents = true;
		}

		void KeyDown(int aMidiIdx){
			if(aMidiIdx >= 0 && aMidiIdx < byte.MaxValue/2){
				SendShortMessage(CommandEnum.NoteOn, aMidiIdx, MidiConversion.GetMidiVolume(customVolume));
			}
		}
		
		void KeyUp(int aMidiIdx){
			if(aMidiIdx >= 0 && aMidiIdx < byte.MaxValue/2){
				SendShortMessage(CommandEnum.NoteOff, aMidiIdx, MidiConversion.GetMidiVolume(customVolume));
			}
		}
	
		void SendShortMessage(CommandEnum aCommand, int aData1, int aData2){
		
			if(ShortMessageEvent != null){
				ShortMessageEvent(
					midiChannel == ChannelEnum.None ? (int)aCommand : (int)midiChannel + (int)aCommand,
					aData1,
					aData2
				);
			}
		
			if(midiOut){
				MidiOut.SendShortMessage(
					midiChannel == ChannelEnum.None ? (int)aCommand : (int)midiChannel + (int)aCommand,
					aData1,
					aData2
				);
			}
		}
		
		void ProceedKeyboardInput(){
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				accidentalState = AccidentalState.sharp;
			}
			if(Input.GetKeyDown(KeyCode.DownArrow)){
				accidentalState = AccidentalState.flat;	
			}
			if(Input.GetKeyUp(KeyCode.UpArrow)){
				accidentalState = AccidentalState.none;
			}
			if(Input.GetKeyUp(KeyCode.DownArrow)){
				accidentalState = AccidentalState.none;
			}	
			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				if(updateKeyboardOctave) {
					if(muteTonesWhenChangingOctave)	MuteTones();
					keyboardOctave--;	
				}
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				if(updateKeyboardOctave) {
					if(muteTonesWhenChangingOctave)	MuteTones();
					keyboardOctave++;	
				}
			}
		
			if(Input.GetKeyDown(KeyCode.RightControl)){
				SendShortMessage(CommandEnum.Controller, (int)PedalEnum.Right, 127);
			}
		
			if(Input.GetKeyUp(KeyCode.RightControl)){
				SendShortMessage(CommandEnum.Controller, (int)PedalEnum.Right, 0);
			}
		
			switch(keyboardInputType){
			case KeyboardInputType.ABCDEFG:
			#region ABCDEFG
				if(Input.GetKeyDown(KeyCode.A)){
					KeyDown((keyboardOctave*12) + 9 + (int)accidentalState);
					accidentalStates[0] = accidentalState;
					keyOctaveIdx[0] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.B)){
					KeyDown((keyboardOctave*12) + 11+ (int)accidentalState);
					accidentalStates[1] = accidentalState;
					keyOctaveIdx[1] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.H)){
					KeyDown((keyboardOctave*12) + 11+ (int)accidentalState);
					accidentalStates[1] = accidentalState;
					keyOctaveIdx[1] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.C)){
					KeyDown((keyboardOctave*12) + 0 + (int)accidentalState);
					accidentalStates[2] = accidentalState;
					keyOctaveIdx[2] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.D)){
					KeyDown((keyboardOctave*12) + 2 + (int)accidentalState);
					accidentalStates[3] = accidentalState;
					keyOctaveIdx[3] = keyboardOctave;
				};
				if(Input.GetKeyDown(KeyCode.E)){
					KeyDown((keyboardOctave*12) + 4 + (int)accidentalState);
					accidentalStates[4] = accidentalState;
					keyOctaveIdx[4] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.F)){
					KeyDown((keyboardOctave*12) + 5 + (int)accidentalState);
					accidentalStates[5] = accidentalState;
					keyOctaveIdx[5] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.G)){
					KeyDown((keyboardOctave*12) + 7 + (int) accidentalState);
					accidentalStates[6] = accidentalState;
					keyOctaveIdx[6] = keyboardOctave;
				}
				
				if(Input.GetKeyUp(KeyCode.A)){
					KeyUp((keyOctaveIdx[0]*12) + 9+ (int)accidentalStates[0]);
					accidentalStates[0] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.B)){
					KeyUp((keyOctaveIdx[1]*12) + 11+ (int)accidentalStates[1]);
					accidentalStates[1] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.H)){
					KeyUp((keyOctaveIdx[1]*12) + 11+ (int)accidentalStates[1]);
					accidentalStates[1] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.C)){
					KeyUp((keyOctaveIdx[2]*12) + 0+ (int)accidentalStates[2]);	
					accidentalStates[2] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.D)){
					KeyUp((keyOctaveIdx[3]*12) + 2+ (int)accidentalStates[3]);
					accidentalStates[3] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.E)){
					KeyUp((keyOctaveIdx[4]*12) + 4+ (int)accidentalStates[4]);
					accidentalStates[4] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.F)){
					KeyUp((keyOctaveIdx[5]*12) + 5+ (int)accidentalStates[5]);
					accidentalStates[5] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.G)){
					KeyUp((keyOctaveIdx[6]*12) + 7+ (int)accidentalStates[6]);
					accidentalStates[6] = AccidentalState.none;
				}
			#endregion
			break;
			case KeyboardInputType.QUERTY:
			#region QUERTY
				if(Input.GetKeyDown(KeyCode.A)){
					KeyDown((keyboardOctave*12) + 0 + (int)accidentalState);
					accidentalStates[0] = accidentalState;
					keyOctaveIdx[0] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.W)){
					KeyDown((keyboardOctave*12) + 1 + (int)accidentalState);
					accidentalStates[1] = accidentalState;
					keyOctaveIdx[1] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.S)){
					KeyDown((keyboardOctave*12) + 2 + (int)accidentalState);
					accidentalStates[2] = accidentalState;
					keyOctaveIdx[2] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.E)){
					KeyDown((keyboardOctave*12) + 3 + (int)accidentalState);
					accidentalStates[3] = accidentalState;
					keyOctaveIdx[3] = keyboardOctave;
				};
				if(Input.GetKeyDown(KeyCode.D)){
					KeyDown((keyboardOctave*12) + 4 + (int)accidentalState);
					accidentalStates[4] = accidentalState;
					keyOctaveIdx[4] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.F)){
					KeyDown((keyboardOctave*12) + 5 + (int)accidentalState);
					accidentalStates[5] = accidentalState;
					keyOctaveIdx[5] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.T)){
					KeyDown((keyboardOctave*12) + 6 + (int) accidentalState);
					accidentalStates[6] = accidentalState;
					keyOctaveIdx[6] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.G)){
					KeyDown((keyboardOctave*12) + 7 + (int) accidentalState);
					accidentalStates[7] = accidentalState;
					keyOctaveIdx[7] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.Y)){
					KeyDown((keyboardOctave*12) + 8 + (int) accidentalState);
					accidentalStates[8] = accidentalState;
					keyOctaveIdx[8] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.Z)){
					KeyDown((keyboardOctave*12) + 8 + (int) accidentalState);
					accidentalStates[8] = accidentalState;
					keyOctaveIdx[8] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.H)){
					KeyDown((keyboardOctave*12) + 9 + (int) accidentalState);
					accidentalStates[9] = accidentalState;
					keyOctaveIdx[9] = keyboardOctave;
				}
			
				if(Input.GetKeyDown(KeyCode.U)){
					KeyDown((keyboardOctave*12) + 10 + (int) accidentalState);
					accidentalStates[10] = accidentalState;
					keyOctaveIdx[10] = keyboardOctave;
				}
			
				if(Input.GetKeyDown(KeyCode.J)){
					KeyDown((keyboardOctave*12) + 11 + (int) accidentalState);
					accidentalStates[11] = accidentalState;
					keyOctaveIdx[11] = keyboardOctave;
				}
				if(Input.GetKeyDown(KeyCode.K)){
					KeyDown((keyboardOctave*12) + 12 + (int) accidentalState);
					accidentalStates[12] = accidentalState;
					keyOctaveIdx[12] = keyboardOctave;
				}
				
			
				if(Input.GetKeyUp(KeyCode.A)){
					KeyUp((keyboardOctave*12) + 0 + (int)accidentalState);
					accidentalStates[0] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.W)){
					KeyUp((keyboardOctave*12) + 1 + (int)accidentalState);
					accidentalStates[1] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.S)){
					KeyUp((keyboardOctave*12) + 2 + (int)accidentalState);
					accidentalStates[2] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.E)){
					KeyUp((keyboardOctave*12) + 3 + (int)accidentalState);
					accidentalStates[3] = AccidentalState.none;
				};
				if(Input.GetKeyUp(KeyCode.D)){
					KeyUp((keyboardOctave*12) + 4 + (int)accidentalState);
					accidentalStates[4] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.F)){
					KeyUp((keyboardOctave*12) + 5 + (int)accidentalState);
					accidentalStates[5] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.T)){
					KeyUp((keyboardOctave*12) + 6 + (int) accidentalState);
					accidentalStates[6] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.G)){
					KeyUp((keyboardOctave*12) + 7 + (int) accidentalState);
					accidentalStates[7] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.Y)){
					KeyUp((keyboardOctave*12) + 8 + (int) accidentalState);
					accidentalStates[8] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.Z)){
					KeyUp((keyboardOctave*12) + 8 + (int) accidentalState);
					accidentalStates[8] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.H)){
					KeyUp((keyboardOctave*12) + 9 + (int) accidentalState);
					accidentalStates[9] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.U)){
					KeyUp((keyboardOctave*12) + 10 + (int) accidentalState);
					accidentalStates[10] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.J)){
					KeyUp((keyboardOctave*12) + 11 + (int) accidentalState);
					accidentalStates[11] = AccidentalState.none;
				}
				if(Input.GetKeyUp(KeyCode.K)){
					KeyUp((keyboardOctave*12) + 12 + (int) accidentalState);
					accidentalStates[12] = AccidentalState.none;
				}
			#endregion
			break;
			}
		}
		
	#endregion
	}

