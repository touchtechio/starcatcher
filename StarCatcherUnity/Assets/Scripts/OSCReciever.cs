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
	public class OSCReciever :  UniOSCEventTarget {

        private StarSpawn starSpawner;  



        // Use this for initialization

        private const string easyDuration = "/1/rotary1";
        private const string easyDelay = "/1/rotary2";
        private const string easyPeriod = "/1/rotary3";

        private const string hardDuration = "/1/rotary4";
        private const string hardDelay = "/1/rotary5";
        private const string hardPeriod = "/1/rotary6";
        


        OscMessage msg;

     
      


        // handles messages
        public void LastMessageUpdate()
        {
            // if no message, escape
            if (msg == null) return;

            if (msg.Address.Contains(easyDuration))
            {
                starSpawner.easyFallDuration = (float)msg.Data[0] * 10.0f;
            }
            if (msg.Address.Contains(easyDelay))
            {
                starSpawner.easyLingerTime = (float)msg.Data[0] * 10.0f;

            }
            if (msg.Address.Contains(easyPeriod))
            {
                starSpawner.easyTimeToSpawn = (float)msg.Data[0] * 10.0f;

            }
            if (msg.Address.Contains(hardDuration))
            {
                starSpawner.hardFallDuration = (float)msg.Data[0] * 10.0f;

            }
            if (msg.Address.Contains(hardDelay))
            {
                starSpawner.hardLingerTime = (float)msg.Data[0] * 10.0f;

            }
            if (msg.Address.Contains(hardPeriod))
            {
                starSpawner.hardTimeToSpawn = (float)msg.Data[0] * 10.0f;

            }





        }


        // upon message received from OSC, call LastMessageUpdate()
        public override void OnOSCMessageReceived(UniOSCEventArgs args)
        {

            if (null == starSpawner)
            {
                starSpawner = StarSpawn.FindObjectOfType<StarSpawn>();
                if (null == starSpawner) Debug.Log("ERROR: no StarSpawn found");
            }


            msg = (OscMessage)args.Packet;


            Debug.Log(msg.Address);
            LastMessageUpdate();

        }
        
	}

}