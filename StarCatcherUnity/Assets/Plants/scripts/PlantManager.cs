using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Manager class that runs the plants
*/

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance = null;

    public string[] root_ids;

    private List<PlantRoot> roots = new List<PlantRoot>();

    public int max_depth;   //this should be plant by plant

    //global health
    private float cur_health;
    public float health_lerp;

    //debug tools
    [Header("Debug Tools")]
    public bool use_debug_sprite_color;


    

    void Awake(){
        if (instance == null){
            instance = this;
        }else if (instance != this) {
            Destroy (gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        reset();
    }

    void reset(){
        for (int i=roots.Count-1; i>=0; i--){
            roots[i].kill();
        }
        roots.Clear();

        for (int i=0; i<3; i++){
            Vector3 pos = new Vector3(3.5f*(i-1),-3,0);
            string root_id = root_ids[ (int)Random.Range(0,root_ids.Length)];
            roots.Add( new PlantRoot(root_id, pos));
        }

        cur_health = 1.0f - Score.cumulativeEnvironmentDamageScore;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug spacebar to reset
        if (Input.GetKeyDown(KeyCode.Space)){
            reset();
        }

        //grab the health value from the game
        //This value is treated as damage, so it is inverted (1=dead, 0=alive)
        float raw_health_value = Mathf.Clamp(1.0f-Score.cumulativeEnvironmentDamageScore, 0.0f, 1.0f);
        //lerp it
        cur_health = Mathf.Lerp(cur_health, raw_health_value, health_lerp);

        for (int i=0; i<roots.Count; i++){
            roots[i].set_health( cur_health );
        }
    }
}
