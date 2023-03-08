using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Strip
{
    public Vector3 starStartPoints;
    public float stripLength;
    public int stripNumber;

    public Strip(Vector3 stripBasePoint, float length, int strip)
    {
        starStartPoints = stripBasePoint;
        stripLength = length;
        stripNumber = strip;
    }
}

public class StripPosition : MonoBehaviour {

    private ArrayList starStrips;
    private static ArrayList stripLengths;
    private int setStripNumber = 0;
    
    // private UnityEngine.Random random = new UnityEngine.Random();
    public static System.Random random = new System.Random();

    // empty game object as parent for strips
    private GameObject parent;
    public int starStripCount = 0;
    [HideInInspector]
    public int stripNumber;
    public bool clearStripArray = false;
    public static StripPosition thisStripPosition;
    private GameObject parentStartPositions;

    // to test all LED strips spawning in sequence;
    [Header ("test strip conditions")]
    public int maxStripNumer = 10;
    public int testStripNumber = 0;

    Strip randomStrip; // instantiate random strip
    Strip lastRandomStrip;
    private Vector3 lastTriggerPosition = new Vector3(0, 0, 0);
    private Vector3 triggerPosition = new Vector3(1,0,0);

    //public Text stripCountText;


    void Awake ()
    {
        if (thisStripPosition)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            thisStripPosition = this;
        }

        parent = GameObject.FindGameObjectWithTag("STRIPS");

        // create and arraylist of Strip objects
        //starStrips = new ArrayList();
        onxStudio();


        // set a random starting strip position
        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);

    }

    void onxStudio()
    {
        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(-0.677202f, 2.171473f, 2.589209f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(-0.0539999f, 2.150523f, 2.603169f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(1.528516f, 2.034108f, 2.562455f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(1.815902f, 2.080061f, 1.84314f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(2.219249f, 2.09286f, 1.581161f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(2.344098f, 2.1452f, 0.6147509f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(2.098734f, 2.14248f, 0.3543839f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(2.370188f, 2.132711f, -0.0018363f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(-0.6371717f, 2.199324f, 1.678417f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(-0.5261359f, 2.201992f, 1.328846f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(0.5062184f, 2.097746f, 0.4969845f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(0.1646428f, 2.13502f, 0.2882626f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(-0.1798005f, 2.139428f, 0.5094523f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(-0.6641436f, 2.137222f, 0.5089588f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(-0.7511215f, 2.161216f, 0.1331737f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(0.04840612f, 2.121885f, -0.05448151f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(0.103148f, 2.124258f, 1.15597f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(1.216573f, 2.099984f, 0.4034538f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(1.394352f, 2.102135f, -0.7049011f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(1.901748f, 2.102933f, -0.8648938f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(2.231683f, 2.076561f, -1.305659f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(2.120373f, 2.046701f, -1.913203f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(2.198202f, 2.142795f, -2.54776f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(1.010656f, 2.07803f, -2.089525f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(-2.356556f, 2.211672f, 1.398779f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(-2.400635f, 2.192272f, 0.9231727f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(-1.983957f, 2.166991f, 0.7124023f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(-1.447709f, 2.122627f, 0.6055863f), 0.5f, 27));
    }

    void home10by10()
    {
        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(1.946978f, 1.108126f, -1.604831f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(1.600185f, 1.526819f, -1.592852f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(0.9154291f, 1.498443f, -1.505807f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(0.2733965f, 1.501068f, -1.477687f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(-0.3425129f, 1.29795f, -1.562479f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(-0.9819003f, 1.472502f, -1.504689f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(-1.600727f, 1.520975f, -1.455309f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(-2.139134f, 1.489455f, -1.451267f), 0.5f, 7));

    }

     void playnyc()
    {
        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(1.94319f, 2.225813f, 0.5722012f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(2.235461f, 2.211134f, 0.2997237f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(1.650277f, 2.20584f, -0.004387498f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(1.417283f, 2.226503f, 0.4830524f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(1.133637f, 2.225457f, 0.8739191f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(1.117562f, 2.201226f, 0.007285178f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(0.7150236f, 2.20683f, 0.3999602f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(0.2533253f, 2.210614f, 0.5673981f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(-0.08685946f, 2.214303f, 0.4387395f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(-0.1703076f, 2.211323f, 0.8406778f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(2.128849f, 2.209076f, -0.08945733f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(0.7260299f, 2.28346f, -0.3587393f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(0.1366973f, 2.271952f, -0.4755517f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(0.5010517f, 2.300206f, -0.8641638f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(-0.4382293f, 2.24962f, -0.678112f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(2.183211f, 2.248235f, -0.5196445f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(1.855874f, 2.18584f, -0.812539f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(1.933718f, 2.149821f, -0.5429768f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(1.577556f, 2.118636f, -0.8695004f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(1.225267f, 2.168814f, -0.7784498f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-0.8665378f, 2.263717f, 0.6132419f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(-1.26797f, 1.940831f, 0.668852f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-1.26797f, 1.940831f, 0.668852f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-1.26797f, 1.940831f, 0.668852f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(-2.048902f, 2.237367f, -0.01205355f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(-1.050894f, 2.208761f, 0.1403474f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(-0.8470242f, 2.182376f, -0.2987552f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(-0.4476082f, 2.179341f, -0.2302197f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(-0.01507926f, 2.234091f, -0.03875905f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(0.5089498f, 2.256142f, 0.132024f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(-1.438788f, 2.279312f, -0.737472f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(-1.853889f, 2.150224f, -0.4542697f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-2.129498f, 2.153162f, -0.2879891f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-2.318543f, 2.290787f, -0.54421f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-2.024328f, 2.255566f, -0.9179402f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-1.60758f, 2.297957f, -0.0990805f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-1.272851f, 2.287949f, -0.5214155f), 0.5f, 36));

    }

    void Update()

    {
       // clear the array of strip positions to repopulate;
       if (clearStripArray == true)
        {
            //LogStripPositons();
        Debug.LogWarning("CLEARING NOW in Update()");

            starStrips.Clear();
        //    stripCountText.text = "Clear";
            clearStripArray = false;
        }

        starStripCount = starStrips.Count;
    }

    public int Count()
    {
        return starStrips.Count;
    }

    public ArrayList getStarPositions() {
        return starStrips;
    }

    public void SetStripPosition(Vector3 triggerPosition)
    {
  
       // lastTriggerPosition = triggerPosition;
        starStrips.Add(new Strip(triggerPosition, 0.5f, starStripCount));

        Debug.Log("SETTING strip number: " + starStripCount + " at position of: "+ triggerPosition);
      //  stripCountText.text = (starStripCount).ToString();
        starStripCount++;

        // GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        LogStripPositons();
    }


    // return a randrom stip object , not position
    public Strip getRandomStrip()
    {
        
        randomStrip = FindRandomStrip();

		// make sure stars are not spawning too close
        if (Vector3.Distance(randomStrip.starStartPoints, lastRandomStrip.starStartPoints) < 1f)
        {
            FindRandomStrip();
        }
        lastRandomStrip = randomStrip;

        // test first strip
       //randomStrip = (Strip)starStrips.ToArray()[0];
      
        return randomStrip; 
        
    }

    private Strip FindRandomStrip()
    {
        int nextPosition = UnityEngine.Random.Range(0, (starStrips.Count - 1));
        randomStrip = GetStrip(nextPosition);
        return randomStrip;
    }

    public Strip GetStrip(int position)
    {
        if (position >= starStrips.Count)
        {
            position = 0;
        }
        return (Strip)starStrips.ToArray()[position];
    }

    // having a running strip number that counts up for testing 
    public Strip getTestStrip()
    {
        Strip testStrip = (Strip)starStrips.ToArray()[testStripNumber];
        if (testStripNumber < (starStripCount-1))
        {
            testStripNumber++;
        }
        else
        {
            testStripNumber = 0;
        }
        return testStrip;
    }

    private void LogStripPositons()
    {
        string msg = "        starStrips = new ArrayList();";
        for (int i =0; i<starStrips.Count;i++) 
        {
            Strip strip = (Strip)starStrips.ToArray()[i];
            msg += "\n        starStrips.Add(new Strip(new Vector3("+ strip.starStartPoints.x+ "f, "+ strip.starStartPoints.y+ "f, "+ strip.starStartPoints.z+ "f), 0.5f, "+i+"));";

        }
        Debug.LogWarning(msg);
    }
    
}


