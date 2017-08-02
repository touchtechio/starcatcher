/*
* UniOSC
* Copyright © 2014-2015 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using OSCsharp.Data;


namespace UniOSC{

	/// <summary>
	/// Rotates (localRotation) the hosting game object.
	/// For every axis you have a separate OSC address to specify
	/// </summary>
	[AddComponentMenu("UniOSC/RotateGameObject")]
	public class UniOSCRotateGameObject :  UniOSCEventTarget {


        // Use this for initialization

        private const string easyDuration = "/1/rotary1";
        private const string easyDelay = "/1/rotary2";
        private const string easyPeriod = "/1/rotary3";

        private const string hardDuration = "/1/rotary4";
        private const string haradDelay = "/1/rotary5";
        private const string hardPeriod = "/1/rotary6";
        


        OscMessage msg;


        // handles messages
        public void LastMessageUpdate()
        {
            // if no message, escape
            if (msg == null) return;






        }


        // upon message received from OSC, call LastMessageUpdate()
        public override void OnOSCMessageReceived(UniOSCEventArgs args)
        {

            msg = (OscMessage)args.Packet;


            Debug.Log(msg.Address);
            LastMessageUpdate();

        }
        
	}

}