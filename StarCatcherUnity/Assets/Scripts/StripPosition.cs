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

    public GameObject prefab;
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
    }

    // Use this for initialization
    void Start() {

        parent = GameObject.FindGameObjectWithTag("STRIPS");



        // create and arraylist of Strip objects
      //  playnyc();
        onxStudio();
        //starStrips = new ArrayList();
        //home10by10();
        //oldStarStarts();

        // set a random starting strip position
        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);



    }

    void onxStudio()
    {
        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(2.185858f, 1.930897f, 0.6399918f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(2.781356f, 1.979688f, 0.3737671f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(3.301333f, 1.983344f, 0.3678274f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(3.443224f, 1.970778f, 1.043836f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(3.461586f, 1.965021f, 1.599963f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(3.106287f, 1.983175f, 1.597613f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(2.582553f, 1.983774f, 1.657697f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(2.207542f, 1.998919f, 1.12885f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(2.041915f, 1.946571f, 1.64497f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(1.546475f, 1.972068f, 1.658114f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(0.9916849f, 1.977161f, 1.406731f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(1.03966f, 1.96594f, 0.9326388f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(0.9822216f, 1.981863f, 0.6437365f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(1.583429f, 1.982423f, 0.9795898f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(1.474602f, 1.978726f, 0.4397337f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(1.860981f, 1.96526f, 0.4822273f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(0.740859f, 1.956095f, 0.4799945f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(0.5280466f, 1.973841f, 0.7449601f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(-0.004975796f, 1.976484f, 0.5697751f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(-0.1615477f, 1.995574f, 0.982451f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-0.1482439f, 1.952695f, 1.514499f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(0.3512869f, 1.956134f, 1.628332f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-0.6772842f, 1.966351f, 1.723739f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-1.157107f, 1.964223f, 1.779242f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(2.064524f, 1.904704f, -0.02282143f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(1.526379f, 1.950947f, -0.07193518f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(0.9500103f, 1.97179f, -0.0449276f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(0.9715335f, 1.952155f, -0.446847f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(0.5509796f, 1.963398f, -0.555546f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(1.672501f, 1.954983f, -0.7670376f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(2.20991f, 1.964657f, -0.4645197f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(2.510136f, 1.964958f, -0.2444901f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-0.09800601f, 1.916277f, -1.878586f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-0.5622525f, 1.960996f, -1.476334f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-1.012771f, 1.965678f, -1.435258f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-0.9352531f, 2.012168f, -1.863196f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-1.401034f, 1.991823f, -1.84224f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(-1.638263f, 2.009162f, -1.453161f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(-1.961179f, 2.004122f, -1.80629f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(-2.433631f, 1.994329f, -1.813976f), 0.5f, 39));
        starStrips.Add(new Strip(new Vector3(1.294748f, 1.925376f, -1.941275f), 0.5f, 40));
        starStrips.Add(new Strip(new Vector3(1.697348f, 1.955131f, -2.002123f), 0.5f, 41));
        starStrips.Add(new Strip(new Vector3(2.118944f, 1.957969f, -1.734078f), 0.5f, 42));
        starStrips.Add(new Strip(new Vector3(2.09779f, 1.994103f, -2.286783f), 0.5f, 43));
        starStrips.Add(new Strip(new Vector3(2.517128f, 2.046899f, -2.047888f), 0.5f, 44));
        starStrips.Add(new Strip(new Vector3(3.120409f, 1.951195f, -2.097474f), 0.5f, 45));
        starStrips.Add(new Strip(new Vector3(3.348325f, 2.029034f, -1.55526f), 0.5f, 46));
        starStrips.Add(new Strip(new Vector3(3.3871f, 1.421309f, -1.803971f), 0.5f, 47));
        starStrips.Add(new Strip(new Vector3(-0.03043914f, 1.851128f, -0.7047112f), 0.5f, 48));
        starStrips.Add(new Strip(new Vector3(-0.5403283f, 1.919429f, -0.6399565f), 0.5f, 49));
        starStrips.Add(new Strip(new Vector3(-1.101983f, 1.922189f, -0.6483936f), 0.5f, 50));
        starStrips.Add(new Strip(new Vector3(-1.456722f, 1.955956f, -0.2364662f), 0.5f, 51));
        starStrips.Add(new Strip(new Vector3(-1.367714f, 1.985314f, 0.2823184f), 0.5f, 52));
        starStrips.Add(new Strip(new Vector3(-0.9447293f, 1.974146f, 0.5846162f), 0.5f, 53));
        starStrips.Add(new Strip(new Vector3(-0.5302753f, 1.977331f, 0.5376338f), 0.5f, 54));
        starStrips.Add(new Strip(new Vector3(-0.1144068f, 1.97729f, 0.2642674f), 0.5f, 55));
        starStrips.Add(new Strip(new Vector3(0.9005368f, 2.025945f, -1.43948f), 0.5f, 56));
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

    public void SetStripPosition(Vector3 triggerPosition)
    {
  
       // lastTriggerPosition = triggerPosition;
        starStrips.Add(new Strip(triggerPosition, 0.5f, starStripCount));

        Debug.Log("setting strip number " + starStripCount + " at position of: "+ triggerPosition);
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


