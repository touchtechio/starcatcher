using System;
using System.IO;
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
    public string stripPositionfilename = "Strip_Positions.csv";


    // to test all LED strips spawning in sequence;
    [Header ("test strip conditions")]
    public int maxStripNumer = 10;
    public int testStripNumber = 0;

    Strip randomStrip; // instantiate random strip
    Strip lastRandomStrip;
    private Vector3 lastTriggerPosition = new Vector3(0, 0, 0);
    private Vector3 triggerPosition = new Vector3(1,0,0);

    public bool IsLoadFromFile = true;

    void Awake ()
    {
        // create shared object
        // 
        // if (thisStripPosition)
        // {
        //     DestroyImmediate(gameObject);
        // }
        // else
        // {
        //     DontDestroyOnLoad(gameObject);
        //     thisStripPosition = this;
        // }

        lastRandomStrip = new Strip(new Vector3(0, 0, 0), 0.5f, 0);

        parent = GameObject.FindGameObjectWithTag("STRIPS");

        // create and arraylist of Strip objects
        //starStrips = new ArrayList();
        onxStudio();

        if(IsLoadFromFile) {
        //    ExportStripPositons();
            ImportStripPositons();
            Debug.Log("IMPORTED Strips");
            LogStripPositons();
        }

        // set a random starting strip position

    }

    void onxStudio()
    {
        //2023-03-30
       starStrips = new ArrayList();
        starStrips.Add(new Strip(new Vector3(-1.867736f, 2.328755f, 1.078507f), 0.5f, 0));
        starStrips.Add(new Strip(new Vector3(-1.93014f, 2.283871f, 2.628036f), 0.5f, 1));
        starStrips.Add(new Strip(new Vector3(-0.3258767f, 2.240891f, 2.807004f), 0.5f, 2));
        starStrips.Add(new Strip(new Vector3(0.8032435f, 2.379582f, 2.696234f), 0.5f, 3));
        starStrips.Add(new Strip(new Vector3(1.690907f, 2.387886f, 2.22843f), 0.5f, 4));
        starStrips.Add(new Strip(new Vector3(1.935439f, 2.493163f, 2.484901f), 0.5f, 5));
        starStrips.Add(new Strip(new Vector3(2.405288f, 2.47435f, 2.598274f), 0.5f, 6));
        starStrips.Add(new Strip(new Vector3(2.806461f, 2.192104f, 1.706008f), 0.5f, 7));
        starStrips.Add(new Strip(new Vector3(-2.635942f, 2.311164f, 0.476285f), 0.5f, 8));
        starStrips.Add(new Strip(new Vector3(-2.714431f, 2.377419f, 0.8361378f), 0.5f, 9));
        starStrips.Add(new Strip(new Vector3(-2.645413f, 2.335084f, 2.005329f), 0.5f, 10));
        starStrips.Add(new Strip(new Vector3(-0.818094f, 2.154868f, 2.459945f), 0.5f, 11));
        starStrips.Add(new Strip(new Vector3(-0.670018f, 2.338582f, 1.98354f), 0.5f, 12));
        starStrips.Add(new Strip(new Vector3(-0.5737457f, 2.352514f, 1.528427f), 0.5f, 13));
        starStrips.Add(new Strip(new Vector3(-0.8637331f, 2.373242f, 1.608317f), 0.5f, 14));
        starStrips.Add(new Strip(new Vector3(-1.319554f, 2.142197f, 1.655512f), 0.5f, 15));
        starStrips.Add(new Strip(new Vector3(-1.9078f, 2.380058f, -0.5733008f), 0.5f, 16));
        starStrips.Add(new Strip(new Vector3(-1.853565f, 2.420654f, -0.2348764f), 0.5f, 17));
        starStrips.Add(new Strip(new Vector3(-1.566102f, 2.376633f, -0.6182952f), 0.5f, 18));
        starStrips.Add(new Strip(new Vector3(-1.459195f, 2.41585f, -0.090518f), 0.5f, 19));
        starStrips.Add(new Strip(new Vector3(-1.440666f, 2.469062f, 0.5526128f), 0.5f, 20));
        starStrips.Add(new Strip(new Vector3(-0.7576008f, 2.436736f, -0.690311f), 0.5f, 21));
        starStrips.Add(new Strip(new Vector3(-0.2671592f, 2.401877f, -1.359378f), 0.5f, 22));
        starStrips.Add(new Strip(new Vector3(-0.7606509f, 2.083847f, -2.016098f), 0.5f, 23));
        starStrips.Add(new Strip(new Vector3(-2.671151f, 2.333331f, -0.2293587f), 0.5f, 24));
        starStrips.Add(new Strip(new Vector3(-2.357237f, 2.355208f, -0.6215467f), 0.5f, 25));
        starStrips.Add(new Strip(new Vector3(-2.007567f, 2.292943f, -1.443592f), 0.5f, 26));
        starStrips.Add(new Strip(new Vector3(-1.475506f, 2.343328f, -1.854129f), 0.5f, 27));
        starStrips.Add(new Strip(new Vector3(-0.8625715f, 2.374358f, -2.617836f), 0.5f, 28));
        starStrips.Add(new Strip(new Vector3(-0.8373172f, 2.371892f, -3.21372f), 0.5f, 29));
        starStrips.Add(new Strip(new Vector3(-1.346368f, 2.105014f, -3.52193f), 0.5f, 30));
        starStrips.Add(new Strip(new Vector3(-1.362282f, 2.086133f, -3.542273f), 0.5f, 31));
        starStrips.Add(new Strip(new Vector3(-0.6261919f, 2.355288f, 0.4799266f), 0.5f, 32));
        starStrips.Add(new Strip(new Vector3(-0.6465721f, 2.380266f, 0.404017f), 0.5f, 33));
        starStrips.Add(new Strip(new Vector3(-0.7071686f, 2.323859f, -0.1405783f), 0.5f, 34));
        starStrips.Add(new Strip(new Vector3(-0.389396f, 2.35565f, -0.2622998f), 0.5f, 35));
        starStrips.Add(new Strip(new Vector3(0.34203f, 2.345353f, 0.03559685f), 0.5f, 36));
        starStrips.Add(new Strip(new Vector3(0.5724967f, 2.351135f, 0.5274343f), 0.5f, 37));
        starStrips.Add(new Strip(new Vector3(0.6277269f, 2.412604f, 1.752457f), 0.5f, 38));
        starStrips.Add(new Strip(new Vector3(0.8557994f, 2.168623f, 2.31639f), 0.5f, 39));
        starStrips.Add(new Strip(new Vector3(-0.07215095f, 2.473849f, -1.914645f), 0.5f, 40));
        starStrips.Add(new Strip(new Vector3(0.3011734f, 2.440038f, -2.199729f), 0.5f, 41));
        starStrips.Add(new Strip(new Vector3(0.6068707f, 2.382549f, -1.836057f), 0.5f, 42));
        starStrips.Add(new Strip(new Vector3(0.7946558f, 2.421237f, -2.228859f), 0.5f, 43));
        starStrips.Add(new Strip(new Vector3(1.199793f, 2.395712f, -2.080924f), 0.5f, 44));
        starStrips.Add(new Strip(new Vector3(1.411108f, 2.355591f, -2.534004f), 0.5f, 45));
        starStrips.Add(new Strip(new Vector3(1.838405f, 2.389603f, -2.126783f), 0.5f, 46));
        starStrips.Add(new Strip(new Vector3(2.522164f, 2.161908f, -2.378632f), 0.5f, 47));
        starStrips.Add(new Strip(new Vector3(-0.6170509f, 2.382033f, -1.368146f), 0.5f, 48));
        starStrips.Add(new Strip(new Vector3(0.4938687f, 2.418231f, -0.7497101f), 0.5f, 49));
        starStrips.Add(new Strip(new Vector3(1.374991f, 2.312321f, 0.1814222f), 0.5f, 50));
        starStrips.Add(new Strip(new Vector3(1.799752f, 2.39976f, -0.1719465f), 0.5f, 51));
        starStrips.Add(new Strip(new Vector3(1.841269f, 2.336597f, 0.2928739f), 0.5f, 52));
        starStrips.Add(new Strip(new Vector3(1.723389f, 2.335807f, 1.175551f), 0.5f, 53));
        starStrips.Add(new Strip(new Vector3(1.243009f, 2.483847f, 1.402164f), 0.5f, 54));
        starStrips.Add(new Strip(new Vector3(0.6880028f, 2.144475f, 1.305843f), 0.5f, 55));
        starStrips.Add(new Strip(new Vector3(0.01148033f, 2.390404f, -3.009354f), 0.5f, 56));
        starStrips.Add(new Strip(new Vector3(1.100853f, 2.411083f, -3.278909f), 0.5f, 57));
        starStrips.Add(new Strip(new Vector3(1.705485f, 2.455712f, -3.367733f), 0.5f, 58));
        starStrips.Add(new Strip(new Vector3(1.464619f, 2.444083f, -3.917652f), 0.5f, 59));
        starStrips.Add(new Strip(new Vector3(0.7323896f, 2.424633f, -4.396347f), 0.5f, 60));
        starStrips.Add(new Strip(new Vector3(0.4294763f, 2.426511f, -3.988973f), 0.5f, 61));
        starStrips.Add(new Strip(new Vector3(0.3718171f, 2.401585f, -3.476649f), 0.5f, 62));
        starStrips.Add(new Strip(new Vector3(-0.4908783f, 2.173222f, -3.662201f), 0.5f, 63));
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

        

        if(Input.GetKeyDown(KeyCode.P)) {
            SetStripPosition(new Vector3(0, 0, 0));
        }

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

        ExportStripPositons();
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

    private void ExportStripPositons()
    {
        string filePath = getStripPositionFilePath();
        StreamWriter writer = new StreamWriter(filePath, false);

        for (int i = 0; i < starStrips.Count; i++) 
        {
            Strip strip = (Strip)starStrips.ToArray()[i];
            string msg = strip.starStartPoints.x + "," + strip.starStartPoints.y + "," + strip.starStartPoints.z;
            writer.WriteLine(msg);
        }
        writer.Close();
        Debug.LogWarning("finished writing " + starStrips.Count + " strip positions: " + filePath);
    }

    private void ImportStripPositons()
    {
        string filePath = getStripPositionFilePath();
        if (!File.Exists(filePath)) {
            Debug.LogError("file not found for importing strip positions: " + filePath);
            return;
        }
        
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        StreamReader reader = new StreamReader(filePath);

        Debug.LogWarning("clearing previous star positions");
        starStrips = new ArrayList();

        for (int starIndex = 0; !reader.EndOfStream; starIndex++) {
            string line = reader.ReadLine();
            string[] corr = line.Split(',');
            if (corr.Length == 3) {
                starStrips.Add(new Strip(new Vector3(float.Parse(corr[0]), float.Parse(corr[1]), float.Parse(corr[2])), 0.5f, starIndex));
            }
            else {
                Debug.LogError("line at index " + starIndex + " invalid. `" + line + "'");
            }
        }

        reader.Close();

        Debug.LogWarning("finished reading " + starStrips.Count + " strip positions from " + filePath);
    }

    private string getStripPositionFilePath()
    {
    #if UNITY_EDITOR
            return Application.dataPath + "/Data/"  + stripPositionfilename;
    #else
            return Application.dataPath + "/" + stripPositionfilename;
    #endif
    }
    
}


