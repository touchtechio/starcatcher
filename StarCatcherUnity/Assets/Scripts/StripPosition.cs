using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
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
    
    private UnityEngine.Random random = new UnityEngine.Random();

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
        home10by10();

        //oldStarStarts();

        // set a random starting strip position
        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);



    }

    void home10by10()
    {


        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(1.059726f, 1.33079f, -1.393115f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(0.520191f, 1.844978f, -1.396726f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(0.07259762f, 1.815557f, -1.615675f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(-0.08546925f, 1.842725f, -1.133391f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(-0.1843446f, 1.825109f, -0.6812489f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(1.175866f, 1.398848f, -0.08642793f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(1.134664f, 1.878924f, -0.4413282f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(0.7295327f, 1.854551f, -0.5884717f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(0.3300259f, 1.789383f, -0.4012734f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(-0.006430507f, 1.809757f, -0.2774734f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(0.2672575f, 1.454039f, 0.998863f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(0.5929818f, 1.895293f, 0.6950709f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(0.9057777f, 1.910389f, 0.3995727f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(0.5952582f, 1.869414f, 0.119608f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(0.05934513f, 1.829899f, -0.08878601f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(-0.6133128f, 1.439966f, 0.9932202f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(-0.1807976f, 1.965337f, 1.229396f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(-0.04258418f, 1.848567f, 0.7301938f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(-0.2516407f, 1.82855f, 0.3096327f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(-0.2351089f, 1.823797f, 0.08902574f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-0.6319648f, 1.850767f, -1.546893f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(-0.970807f, 1.851188f, -1.524421f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-0.8483121f, 1.758552f, -1.117436f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-0.6108332f, 1.831099f, -0.9388616f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(-0.4988703f, 1.788141f, -0.5427201f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(-1.794754f, 1.947356f, -1.472434f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(-1.460273f, 1.882933f, -1.069819f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(-1.314088f, 1.882119f, -0.6206622f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(-0.8923193f, 1.848495f, -0.6032078f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(-0.4478242f, 1.827267f, -0.3069352f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(-1.784257f, 1.886664f, -0.468276f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(-1.648832f, 1.969646f, 0.0731101f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-1.215944f, 1.883387f, -0.006329775f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-1.015607f, 1.850521f, -0.2135639f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-0.7152587f, 1.821804f, -0.1824857f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-1.458969f, 1.937838f, 0.4310877f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-1.12759f, 1.937596f, 0.8326677f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(-0.6527772f, 1.86044f, 0.5981877f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(-0.6507943f, 1.853665f, 0.1946073f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(-0.4770563f, 1.855852f, -0.08421588f), 0.5f, 39));


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

        Debug.Log("setting strip number " + setStripNumber + " at position of: "+ triggerPosition);
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


