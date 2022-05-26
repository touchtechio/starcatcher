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
        starStrips.Add(new Strip(new Vector3(1.948621f, 0.9513419f, -1.795936f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(1.966198f, 0.9796948f, -1.359668f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(1.958197f, 1.061528f, -0.5673079f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(2.017306f, 1.070851f, -0.300168f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(1.960369f, 1.075197f, 0.2804232f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(1.971987f, 1.072215f, 0.729311f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(1.913491f, 1.05209f, 1.376725f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(1.460982f, 1.039142f, 1.516362f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(1.230176f, 1.109243f, 1.038358f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(1.165664f, 1.093896f, 0.6473521f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(1.166367f, 1.116576f, 0.1142464f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(1.12132f, 1.122709f, -0.4970703f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(1.096595f, 1.139531f, -1.00182f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(1.020283f, 1.149761f, -1.606499f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(0.6576118f, 1.115015f, -1.771696f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(0.5719728f, 1.11474f, -1.379122f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(0.6118402f, 1.11742f, -0.9463782f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(0.6276858f, 1.097309f, -0.4773064f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(0.6210558f, 1.085066f, -0.04613686f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(0.6656785f, 1.099189f, 0.4374577f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(0.6893773f, 1.090928f, 0.9674706f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(0.6859906f, 1.066105f, 1.46231f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(0.1947694f, 1.080747f, 1.610932f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(0.06300759f, 1.109553f, 1.152393f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(-0.009894609f, 1.05619f, 0.8052344f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(-0.07542586f, 1.071206f, 0.2925441f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(-0.07032943f, 1.072594f, -0.287816f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(-0.1187224f, 1.093927f, -0.8089995f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(-0.09631348f, 1.097349f, -1.37684f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(-0.1579359f, 1.096262f, -1.708518f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(-0.3569062f, 1.097133f, -1.800539f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(-0.4483888f, 1.104337f, -1.471219f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-0.4679294f, 1.110675f, -1.19118f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-0.4270835f, 1.08679f, -0.7446032f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-0.4265683f, 1.100107f, -0.1849899f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-0.387547f, 1.089845f, 0.2808297f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-0.3956637f, 1.084295f, 0.7837536f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(-0.3629332f, 1.051394f, 1.349308f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(-0.8725982f, 1.093934f, 1.617398f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(-1.051092f, 1.083656f, 1.233446f), 0.5f, 39));
        starStrips.Add(new Strip(new Vector3(-1.065862f, 1.095136f, 0.7590947f), 0.5f, 40));
        starStrips.Add(new Strip(new Vector3(-1.089089f, 1.087416f, 0.2932966f), 0.5f, 41));
        starStrips.Add(new Strip(new Vector3(-1.095935f, 1.103708f, -0.2226608f), 0.5f, 42));
        starStrips.Add(new Strip(new Vector3(-1.146848f, 1.103051f, -0.7403178f), 0.5f, 43));
        starStrips.Add(new Strip(new Vector3(-1.181769f, 1.11382f, -1.237792f), 0.5f, 44));
        starStrips.Add(new Strip(new Vector3(-1.250508f, 1.093565f, -1.808267f), 0.5f, 45));
        starStrips.Add(new Strip(new Vector3(-1.475002f, 1.120074f, -1.872408f), 0.5f, 46));
        starStrips.Add(new Strip(new Vector3(-1.486157f, 1.128169f, -1.49009f), 0.5f, 47));
        starStrips.Add(new Strip(new Vector3(-1.477116f, 1.12094f, -1.006654f), 0.5f, 48));
        starStrips.Add(new Strip(new Vector3(-1.400717f, 1.104531f, -0.5183289f), 0.5f, 49));
        starStrips.Add(new Strip(new Vector3(-1.359653f, 1.123742f, 0.1160092f), 0.5f, 50));
        starStrips.Add(new Strip(new Vector3(-1.345268f, 1.105212f, 0.7050128f), 0.5f, 51));
        starStrips.Add(new Strip(new Vector3(-1.366018f, 1.079232f, 1.324966f), 0.5f, 52));
        starStrips.Add(new Strip(new Vector3(-1.773124f, 1.052188f, 1.636476f), 0.5f, 53));
        starStrips.Add(new Strip(new Vector3(-1.936846f, 1.070781f, 1.193513f), 0.5f, 54));
        starStrips.Add(new Strip(new Vector3(-2.010192f, 1.110468f, 0.6720531f), 0.5f, 55));
        starStrips.Add(new Strip(new Vector3(-2.034064f, 1.110119f, 0.02618027f), 0.5f, 56));
        starStrips.Add(new Strip(new Vector3(-2.076367f, 1.083105f, -0.5735509f), 0.5f, 57));
        starStrips.Add(new Strip(new Vector3(-2.135203f, 1.114164f, -1.1254f), 0.5f, 58));
        starStrips.Add(new Strip(new Vector3(-2.17759f, 1.083249f, -1.652271f), 0.5f, 59));
        starStrips.Add(new Strip(new Vector3(-2.096524f, 1.111088f, -2.020293f), 0.5f, 60));
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


