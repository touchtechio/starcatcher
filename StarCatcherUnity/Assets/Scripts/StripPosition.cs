using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
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
    public GameObject starPositionPrefab;
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
      //  playnyc();
        onxStudio();
        //starStrips = new ArrayList();
        //home10by10();
        //oldStarStarts();

        // set a random starting strip position
        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);

        parentStartPositions = GameObject.FindGameObjectWithTag("STARTING POSITIONS");
        //InvokeRepeating("Spawn", 0.5f, spawnRate);

        int positionCount = starStrips.Count;

        for (int i = 0; i < positionCount; i++) {
            Strip strip = (Strip)starStrips[i];
            Vector3 starPosition = strip.starStartPoints;
            GameObject star = Instantiate(starPositionPrefab, starPosition, Quaternion.identity) as GameObject;
            star.transform.parent = parentStartPositions.transform;
            StripIndex index = star.GetComponent<StripIndex>();
            index.stripIndex = i; 
        }    

    }

    void onxStudio()
    {
        starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(2.300205f, 2.060259f, 0.573231f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(2.766671f, 2.460858f, 0.2912414f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(3.251558f, 2.288739f, 0.3847592f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(3.447392f, 2.439425f, 0.8627195f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(3.519104f, 2.346648f, 1.48648f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(3.186251f, 2.41687f, 1.502752f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(2.728299f, 2.399908f, 1.527494f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(2.370366f, 2.433606f, 1.103268f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(2.219579f, 2.011885f, 1.566505f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(1.653921f, 2.410522f, 1.598226f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(1.184302f, 2.372085f, 1.394898f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(1.254002f, 2.335945f, 1.004415f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(1.139058f, 2.372335f, 0.6810585f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(1.6079f, 2.441931f, 0.9585223f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(1.572554f, 2.412303f, 0.4467983f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(1.815642f, 2.43186f, 0.4504411f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(0.938457f, 2.004121f, 0.4544164f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(0.6608782f, 2.338581f, 0.6621623f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(0.1486759f, 2.397324f, 0.5155631f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(-0.08617043f, 2.306467f, 0.9367439f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-0.06861067f, 2.302094f, 1.368641f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(0.3571837f, 2.344577f, 1.61755f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-0.4282594f, 2.168453f, 1.74518f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-1.029709f, 2.172896f, 1.797349f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(2.059488f, 1.995922f, -0.02450514f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(1.631151f, 2.432503f, -0.03121662f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(1.101691f, 2.412817f, -0.09278321f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(1.009214f, 2.40335f, -0.4530783f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(0.6315939f, 2.346111f, -0.56581f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(1.620994f, 2.349796f, -0.7085395f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(2.097924f, 2.368173f, -0.5079184f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(2.488941f, 2.382821f, -0.3090689f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-0.09134579f, 2.034169f, -1.901528f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-0.4084463f, 2.234221f, -1.635357f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-0.9144711f, 2.239192f, -1.528451f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-0.8976507f, 2.43608f, -1.859632f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(-1.401237f, 2.238621f, -1.872154f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(-1.492061f, 1.973484f, -1.53251f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(-1.853438f, 2.197114f, -1.777394f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(-2.323375f, 2.246128f, -1.800018f), 0.5f, 39));
        starStrips.Add(new Strip(new Vector3(1.203096f, 2.012955f, -2.003977f), 0.5f, 40));
        starStrips.Add(new Strip(new Vector3(1.595214f, 2.205708f, -1.975808f), 0.5f, 41));
        starStrips.Add(new Strip(new Vector3(2.055918f, 2.432405f, -1.741762f), 0.5f, 42));
        starStrips.Add(new Strip(new Vector3(2.111385f, 2.303154f, -2.181113f), 0.5f, 43));
        starStrips.Add(new Strip(new Vector3(2.515571f, 2.425565f, -2.044561f), 0.5f, 44));
        starStrips.Add(new Strip(new Vector3(3.119042f, 2.443919f, -2.07722f), 0.5f, 45));
        starStrips.Add(new Strip(new Vector3(3.318682f, 2.440698f, -1.695084f), 0.5f, 46));
        starStrips.Add(new Strip(new Vector3(3.466397f, 2.25527f, -1.295297f), 0.5f, 47));
        starStrips.Add(new Strip(new Vector3(0.008400917f, 2.019715f, -0.6901605f), 0.5f, 48));
        starStrips.Add(new Strip(new Vector3(-0.5405803f, 2.264815f, -0.676708f), 0.5f, 49));
        starStrips.Add(new Strip(new Vector3(-1.060173f, 2.414889f, -0.6315112f), 0.5f, 50));
        starStrips.Add(new Strip(new Vector3(-0.6936045f, 2.24081f, -0.3612471f), 0.5f, 51));
        starStrips.Add(new Strip(new Vector3(-0.6480339f, 2.160148f, 0.1576729f), 0.5f, 52));
        starStrips.Add(new Strip(new Vector3(-1.029977f, 2.217956f, 0.5225786f), 0.5f, 53));
        starStrips.Add(new Strip(new Vector3(-0.5336039f, 2.234694f, 0.5272273f), 0.5f, 54));
        starStrips.Add(new Strip(new Vector3(-0.1553524f, 2.421841f, 0.253958f), 0.5f, 55));
        starStrips.Add(new Strip(new Vector3(0.9249101f, 2.101629f, -1.348144f), 0.5f, 56));
        starStrips.Add(new Strip(new Vector3(2.01356f, 2.459557f, -1.33021f), 0.5f, 57));
        starStrips.Add(new Strip(new Vector3(2.153542f, 2.460256f, -0.9809799f), 0.5f, 58));
        starStrips.Add(new Strip(new Vector3(2.668509f, 2.446909f, -0.851038f), 0.5f, 59));
        starStrips.Add(new Strip(new Vector3(2.569478f, 2.261569f, -1.274115f), 0.5f, 60));
        starStrips.Add(new Strip(new Vector3(2.98518f, 2.393653f, -1.237629f), 0.5f, 61));
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

    public ArrayList getStarPositions() {
        return starStrips;
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


