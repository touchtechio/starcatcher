using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using OSCsharp.Data;

namespace UniOSC{

	[AddComponentMenu("UniOSC/OSCSpawnDataRecieve")]
	[RequireComponent(typeof(StarSpawn))]
	public class OSCSpawnDataRecieve :  UniOSCEventTarget {

        public string easyRate = "/1/easy/1";
        public string easyFall = "/1/easy/2";
        public string easyLinger = "/1/easy/3";

        public string mediumRate = "/1/medium/1";
        public string mediumFall = "/1/medium/2";
        public string mediumLinger = "/1/medium/3";

        public string hardRate = "/1/hard/1";
        public string hardFall = "/1/hard/2";
        public string hardLinger = "/1/hard/3";

        public string spawnStar = "/2/spawn/";

        public string levelSwitch = "/2/level/1/";



        private StarSpawn spawner;
	
		void Awake(){

		}

		private void _Init(){
		
			receiveAllAddresses = true;
	/*		_oscAddresses.Clear();
            _oscAddresses.Add(easyRate);
            _oscAddresses.Add(easyFall);
            _oscAddresses.Add(easyLinger);
            _oscAddresses.Add(mediumRate);
            _oscAddresses.Add(mediumFall);
            _oscAddresses.Add(mediumLinger);
            _oscAddresses.Add(hardRate);
            _oscAddresses.Add(hardFall);
            _oscAddresses.Add(hardLinger);
            */
            if (Application.isPlaying){
                    spawner = gameObject.GetComponent<StarSpawn>();
				}else{
                    spawner = null;
				}
		}


		public override void OnEnable(){
			_Init();
			base.OnEnable();
		}

	
		public override void OnOSCMessageReceived(UniOSCEventArgs args){
		
			if(((OscMessage)args.Packet).Data.Count <1)return;
			if(spawner == null)return;

			if(!( ((OscMessage)args.Packet).Data[0]  is  System.Single))return;
			float value = (float)((OscMessage)args.Packet).Data[0] ;



            if (args.Address.Contains(spawnStar))
            {
                string fullStarAddress = args.Address.Substring(spawnStar.Length);

                //Debug.Log(fullStarAddress.Substring(0, 1));
                int row = int.Parse(fullStarAddress.Substring(0, 1));

                //Debug.Log(fullStarAddress.Substring(2, 1));
                int column = int.Parse(fullStarAddress.Substring(2, 1));

                row = 5 - row;
                column = column - 1;

                int starNumber = (row * 8 + column);
                Debug.Log("star number spawning: " + starNumber);


                spawner.Spawn(spawner.stripPositions.GetStrip(starNumber));

            }
            if (args.Address.Contains(levelSwitch))
            {
                string fullStarAddress = args.Address.Substring(levelSwitch.Length);

                int level = int.Parse(fullStarAddress.Substring(0, 1))-1;
                Debug.Log("setting level: " + level);
                Score.SetLevel(level);

            }

            if (String.Equals(args.Address, easyRate))
            {
                spawner.easyTimeToSpawn = value;
            }
            if (String.Equals(args.Address, easyFall))
            {
                spawner.easyFallDuration = value;
            }
            if (String.Equals(args.Address, easyLinger))
            {
                spawner.easyLingerTime = value;
            }

            if (String.Equals(args.Address, mediumRate))
            {
                spawner.mediumTimeToSpawn = value;
            }
            if (String.Equals(args.Address, mediumFall))
            {
                spawner.mediumFallDuration = value;
            }
            if (String.Equals(args.Address, mediumLinger))
            {
                spawner.mediumLingerTime = value;
            }


            if (String.Equals(args.Address, hardRate))
            {
                spawner.hardTimeToSpawn = value;
            }
            if (String.Equals(args.Address, hardFall))
            {
                spawner.hardFallDuration = value;
            }
            if (String.Equals(args.Address, hardLinger))
            {
                spawner.hardLingerTime = value;
            }


        }



    }
}