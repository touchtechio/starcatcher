using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Manager class that runs the plants
*/

public class PlantManager : MonoBehaviour
{
    [System.Serializable]
   public struct ChildInfo { 
       public string id_name; 
       public float weight_start, weight_end;
       
    }

    public static PlantManager instance = null;

    public string[] root_ids;

    private List<PlantRoot> roots = new List<PlantRoot>();

    public float max_x_dist;
    public int num_plants_to_spawn;

    //global health
    private float cur_health;
    public float health_lerp;

    //movement - these may need to be specific to plants
    [Header("Movement Values")]
    public float min_sway_speed;
    public float max_sway_speed;
    public float sway_dist;

    //debug tools
    [Header("Debug Tools")]
    public bool use_debug_sprite_color;
    public bool debug_even_spacing;

    //testing out state stuff
    public enum GameState {Game, Dead, Rejuvination, Flourishing, Decline};
    public GameState cur_state;


    //setting child values
    [Header("Cooksonia")]

    public ChildInfo[] possible_children_cooksonia;
    

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

        for (int i=0; i<num_plants_to_spawn; i++){
            Vector3 pos = new Vector3(Random.Range(-max_x_dist, max_x_dist),-3,0);

            if (debug_even_spacing){
                float prc = (float)i / (float)(num_plants_to_spawn-1);
                pos.x = (1.0f-prc)*-max_x_dist + prc * max_x_dist;
            }

            string root_id = root_ids[ (int)Random.Range(0,root_ids.Length)];
            roots.Add( new PlantRoot(root_id, pos, i*20));
        }

        cur_health = 0;// 1.0f - Score.cumulativeEnvironmentDamageScore;
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
        
        //in some game states, intercept this value and hard set it
        if (cur_state == GameState.Dead)            raw_health_value = 0;
        if (cur_state == GameState.Rejuvination)    raw_health_value = 1;
        
        //lerp it
        cur_health = Mathf.Lerp(cur_health, raw_health_value, health_lerp);

        //set the health value of all plants
        foreach(PlantRoot root in roots){
            root.set_health( cur_health );
        }
    }
}
