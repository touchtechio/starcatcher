/* 
* 	(c) Copyright Marek Ledvina, Foriero Studo
*/

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using ForieroEngine.MIDIUnified;

namespace ForieroEngine.MIDIUnified.Plugins
{
	#if UNITY_ANDROID && MIDI_PLUGIN && !UNITY_EDITOR
	public static class MidiInPlugin {

		public static List<string> deviceNames = new List<string>();
		public static Queue<MidiMessage> midiMessages = new Queue<MidiMessage>(100);

		[DllImport ("javabridge")]
		private static extern int MidiIn_PortCount();
		
		[DllImport ("javabridge")]
		private static extern string MidiIn_PortName(int i);
		
		[DllImport ("javabridge")]
		private static extern int PopMidiMessage (out MidiMessage packet);
				
		[DllImport ("javabridge")]
		private static extern int MidiIn_PortOpen(int i);
		
		[DllImport ("javabridge")]
		private static extern void MidiIn_PortClose();
							
		public static int ConnectDevice(int i){
			Debug.Log("MidiIn_PortOpen : " + i);
			return MidiIn_PortOpen(i);
		}
		
		public static int DisconnectDevice(){
			Debug.Log("MidiIn_PortClose");
			MidiIn_PortClose();
			return 1;
		}
				
		public static string GetDeviceName(int i){
			//Debug.Log("MidiIn_PortName : " + MidiIn_PortName(i));
			return MidiIn_PortName(i);
		}
		
		public static int GetDeviceCount(){
			//Debug.Log("MidiIn_PortCount : " + MidiIn_PortCount().ToString());
			return MidiIn_PortCount();
		}
		
		public static int PopMessage(out MidiMessage aMidiMessage){
		 	return PopMidiMessage(out aMidiMessage);
		}
	}
	
	public static class MidiOutPlugin{

		public static List<string> deviceNames = new List<string>();

		[DllImport ("javabridge")]
		private static extern int SendMessage(int Command, int Data1, int Data2);
						
		[DllImport ("javabridge")]
		private static extern int MidiOut_PortCount();
		
		[DllImport ("javabridge")]
		private static extern string MidiOut_PortName(int i);
		
		[DllImport ("javabridge")]
		private static extern int MidiOut_PortOpen(int i);
		
		[DllImport ("javabridge")]
		private static extern void MidiOut_PortClose();
		
		public static int SendShortMessage(byte Command,byte Data1, byte Data2)
	    {
			if(CSHARPSynth.useCSHARPSynth && CSHARPSynth.active){
				CSHARPSynth.SendMessage(Command, Data1, Data2);	
			}
			if(iOSSynth.useiOSSynth && iOSSynth.active){
				iOSSynth.SendSynthMessage(Command, Data1, Data2);	
			}
			return SendMessage(Command, Data1, Data2);
		}

		public static int SendData(byte[] Data){
			return 0;
		}
				
		public static int ConnectDevice(int i){
			Debug.Log("MidiOut_PortOpen : " + i.ToString());
			return MidiOut_PortOpen(i);
		}
		
		public static int DisconnectDevice(){
			Debug.Log("MidiOut_PortClose");
			MidiOut_PortClose();
			return 1;
		}
				
		
		public static string GetDeviceName(int i){
			//Debug.Log("MidiIn_PortName : " + MidiOut_PortName(i));
			return MidiOut_PortName(i);
		}
		
		public static int GetDeviceCount(){
			//Debug.Log("MidiOut_PortCount : " + MidiOut_PortCount().ToString());
			return MidiOut_PortCount();
		}
	}
	#endif
}
