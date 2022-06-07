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
        starStrips.Add(new Strip(new Vector3(2.260372f, 2.063245f, 0.5389717f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(2.735526f, 2.442588f, 0.3030348f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(3.264977f, 2.322007f, 0.3616948f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(3.438553f, 2.492779f, 0.9140506f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(3.486018f, 2.501507f, 1.50519f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(3.192454f, 2.499016f, 1.452868f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(2.663882f, 2.465607f, 1.517134f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(2.296345f, 2.536967f, 1.056704f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(2.097129f, 2.050803f, 1.529763f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(1.604666f, 2.475169f, 1.537731f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(1.106135f, 2.484293f, 1.325463f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(1.167741f, 2.4956f, 0.9352055f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(1.068961f, 2.530226f, 0.59816f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(1.554827f, 2.527421f, 0.9365305f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(1.478192f, 2.503503f, 0.4001139f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(1.82613f, 2.463954f, 0.4175894f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(0.9063876f, 2.022369f, 0.4074293f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(0.5601203f, 2.47147f, 0.6260679f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(0.0933845f, 2.490576f, 0.4737176f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(-0.1248751f, 2.503464f, 0.88397f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-0.07171106f, 2.489606f, 1.343377f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(0.3274992f, 2.488712f, 1.525361f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-0.6163337f, 2.456609f, 1.6894f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-1.131397f, 2.442747f, 1.723222f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(2.108123f, 2.002248f, -0.05405712f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(1.583511f, 2.457034f, -0.05068612f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(1.045765f, 2.461698f, -0.119427f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(0.9863799f, 2.529055f, -0.5688138f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(0.5153952f, 2.439087f, -0.625433f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(1.751615f, 2.499918f, -0.8391366f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(2.204096f, 2.495919f, -0.5181284f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(2.538173f, 2.43181f, -0.3037183f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-0.09060001f, 1.944586f, -1.958533f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-0.5552008f, 2.401611f, -1.585907f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-1.051112f, 2.408345f, -1.526935f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-0.9116697f, 2.403934f, -1.927232f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-1.406345f, 2.188428f, -1.876413f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(-1.677238f, 2.198981f, -1.48987f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(-2.021172f, 2.165898f, -1.809554f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(-2.458068f, 2.254882f, -1.841998f), 0.5f, 39));
        starStrips.Add(new Strip(new Vector3(1.298663f, 2.006974f, -1.993624f), 0.5f, 40));
        starStrips.Add(new Strip(new Vector3(1.73601f, 2.422158f, -2.055418f), 0.5f, 41));
        starStrips.Add(new Strip(new Vector3(2.140795f, 2.45981f, -1.770476f), 0.5f, 42));
        starStrips.Add(new Strip(new Vector3(2.111825f, 2.474341f, -2.3268f), 0.5f, 43));
        starStrips.Add(new Strip(new Vector3(2.617566f, 2.427985f, -2.111736f), 0.5f, 44));
        starStrips.Add(new Strip(new Vector3(3.183252f, 2.438406f, -2.140643f), 0.5f, 45));
        starStrips.Add(new Strip(new Vector3(3.368612f, 2.481155f, -1.625596f), 0.5f, 46));
        starStrips.Add(new Strip(new Vector3(3.463612f, 2.426723f, -1.276432f), 0.5f, 47));
        starStrips.Add(new Strip(new Vector3(-0.03218031f, 2.006245f, -0.75492f), 0.5f, 48));
        starStrips.Add(new Strip(new Vector3(-0.5855761f, 2.42029f, -0.7606065f), 0.5f, 49));
        starStrips.Add(new Strip(new Vector3(-1.112388f, 2.431147f, -0.6420603f), 0.5f, 50));
        starStrips.Add(new Strip(new Vector3(-0.7087255f, 2.396025f, -0.3092589f), 0.5f, 51));
        starStrips.Add(new Strip(new Vector3(-0.7013373f, 2.383443f, 0.2059863f), 0.5f, 52));
        starStrips.Add(new Strip(new Vector3(-1.118092f, 2.430272f, 0.4946183f), 0.5f, 53));
        starStrips.Add(new Strip(new Vector3(-0.6190836f, 2.413213f, 0.6071589f), 0.5f, 54));
        starStrips.Add(new Strip(new Vector3(-0.1879532f, 2.497161f, 0.2121208f), 0.5f, 55));
        starStrips.Add(new Strip(new Vector3(0.9722769f, 2.037162f, -1.446361f), 0.5f, 56));
        starStrips.Add(new Strip(new Vector3(2.094617f, 2.445735f, -1.415792f), 0.5f, 57));
        starStrips.Add(new Strip(new Vector3(2.189342f, 2.513483f, -0.9793148f), 0.5f, 58));
        starStrips.Add(new Strip(new Vector3(2.743191f, 2.417879f, -0.877943f), 0.5f, 59));
        starStrips.Add(new Strip(new Vector3(2.777131f, 2.446548f, -1.338606f), 0.5f, 60));
        starStrips.Add(new Strip(new Vector3(3.049854f, 2.436421f, -1.331088f), 0.5f, 61));
        starStrips.Add(new Strip(new Vector3(3.440267f, 2.459702f, -0.9333291f), 0.5f, 62));
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


