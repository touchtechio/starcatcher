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

    [System.Serializable]
   public struct PlantRootInfo { 
       public string plant_type;
       public string limb_id; 
       public int max_depth;
       public float start_angle;
       public float start_angle_range;
       public float max_bonus_scale;
    }

    public static PlantManager instance = null;

    public PlantRootInfo[] possible_roots;

    
    [Header("Colors")]
    [Tooltip("This value overrides the alpha value in the colors aray")]
    public float color_alpha;
    public Color[] colors;

    private List<PlantRoot> roots = new List<PlantRoot>();

    [Header("Plant Positions")]
    [Tooltip("max X distance from center to spawn plants")]
    public float max_x_dist;
    [Tooltip("Number of plants to spawn")]
    public int num_plants_to_spawn;
    [Tooltip("How evenly spaced they'll be. 1=perfectly evenly spaced. The higher the vlaue the more random. Must be 1 or higher")]
    public float random_position_density;

    //global health
    [Header("Game Health")]
    private float cur_health;
    [Tooltip("How quickly the plant health will slide to match the environment damage value")]
    public float health_lerp;
    [Tooltip("lowest health plants can have in game mode. Must be 0-1")]
    public float min_health_during_gameplay;

    //movement - these may need to be specific to plants
    [Header("Movement Values")]
    public float min_sway_speed;
    public float max_sway_speed;
    [Tooltip("Distance in angles that an individual joint can sway")]
    public float sway_dist;

    [Header("Death State Control")]
    [Tooltip("min time to wait per plant before dying")]
    public float min_death_pause_time;
    [Tooltip("max time to wait per plant before dying. SHOULD BE LESS THAN Score.deadTimerValue")]
    public float max_death_pause_time;
    [Tooltip("min time for a dying plant to shrink to nothing")]
    public float min_death_shrink_time;
    [Tooltip("max time for a dying plant to shrink to nothing")]
    public float max_death_shrink_time;

    [Header("Rejuvenation State Control")]
    [Tooltip("pause time before an individual plant does growth animation")]
    public float rejuvenation_min_pause_time, rejuvenation_max_pause_time;
    [Tooltip("time it takes an individual plant part to grow to full size")]
    public float rejuvenation_min_grow_time, rejuvenation_max_grow_time;

    //debug tools
    [Header("Debug Tools")]
    public bool use_debug_sprite_color;
    public bool debug_even_spacing;
    public bool debug_fast_grow;
    [Tooltip("if true, debug_fake_damage_value & debug_cur_state are used")]
    public bool debug_use_fake_game_state;
    [Range(0.0f, 1.0f)]
    public float debug_fake_damage_value;
    [Tooltip("set this to -1 to turn it off")]
    public int debug_root_id;

    public Score.GameState debug_cur_state;

    private Score.GameState prev_state;

   


    //setting child values
    [Header("Cooksonia")]

    public ChildInfo[] possible_children_cooksonia;
    
    [Header("Leafy")]

    public ChildInfo[] possible_children_leafy;

    [Header("Tall")]

    public ChildInfo[] possible_children_tall;
    

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
        //change some timing values if fast_grow is on
        if (debug_fast_grow){
            rejuvenation_max_pause_time = 0.0f;
            rejuvenation_min_pause_time = 0.1f;
            rejuvenation_min_grow_time = 0.0f;
            rejuvenation_max_grow_time = 0.1f;

        }

        //make sure all colors have the correct alpha value
        for (int i=0; i<colors.Length; i++){
            colors[i].a = color_alpha;
        }

        prev_state = Score.GameState.Dead;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug spacebar to reset when testing
        if (Input.GetKeyDown(KeyCode.Space) && debug_use_fake_game_state){
            debug_cur_state = Score.GameState.Rejuvination;
        }

        //grab the global game state
        Score.GameState cur_state = Score.plasmaWorldState;

        //when testing, override the real game state with the testing value
        if (debug_use_fake_game_state){
            cur_state = debug_cur_state;
        }

        //during gameplay and rejuvination, grab the health value form the game
        if (cur_state == Score.GameState.Flourishing || cur_state == Score.GameState.Decline || cur_state == Score.GameState.Dying){
            //grab the health value from the game
            //This value is treated as damage, so it is inverted (1=dead, 0=alive)
            float raw_health_value = Mathf.Clamp(1.0f-Score.cumulativeEnvironmentDamageScore, 0.0f, 1.0f);

            //if testing, replace it with the dbeug value
            if (debug_use_fake_game_state){
                raw_health_value = 1.0f - debug_fake_damage_value;
            }

            //during gameplay, map this to a new minimum
            raw_health_value = min_health_during_gameplay + (1.0f-min_health_during_gameplay)*raw_health_value;
            
            //lerp it
            cur_health = Mathf.Lerp(cur_health, raw_health_value, health_lerp);

            //set the health value of all plants
            foreach(PlantRoot root in roots){
                root.set_health( cur_health );
            }
        }

        //if we just died, do that
        if(cur_state == Score.GameState.Dead && prev_state != Score.GameState.Dead){
            start_death();
        }

        //if we just entered regrwoth, do that
        if(cur_state == Score.GameState.Rejuvination && prev_state != Score.GameState.Rejuvination){
            start_rejuvination();
        }
        if (cur_state == Score.GameState.Rejuvination){
            cur_health = 1.0f;
        }

        //store the old state
        prev_state = cur_state;
    }

    void start_death(){
        foreach(PlantRoot root in roots){
            root.start_death_animation();
        }
    }

    void start_rejuvination(){
        //kill what's there
        for (int i=roots.Count-1; i>=0; i--){
            roots[i].kill();
        }
        roots.Clear();

        cur_health = 1;

        debug_fake_damage_value = 0.0f;
        
        StartCoroutine(do_rejuvination());
    }

    IEnumerator do_rejuvination(){
        Debug.Log("<color=purple>Rejuvnation start</color>");
        //give it a moment after destroying everything
        yield return new WaitForSeconds(0.1f);

        //get our total time
        float total_time_to_rejuvenate = Score.Instance.rejuvinationTimerValue - rejuvenation_max_grow_time - rejuvenation_max_pause_time;

        if (debug_fast_grow){
            total_time_to_rejuvenate = 0.2f;
        }

        Debug.Log("total rejuventation time: "+total_time_to_rejuvenate);

        //generate a list of possible spaces
        List<Vector3> spawn_positions = new List<Vector3>();

        //make sure we're in range
        if (random_position_density < 1.0f){
            random_position_density = 1.0f;
            Debug.Log("random_position_density MUS BE >= 1! Setting it to 1.");
        }
        int num_spawn_spots = (int)((float) num_plants_to_spawn * random_position_density);
        for (int i=0; i<num_spawn_spots; i++){
            float prc = (float)i / (float)(num_spawn_spots-1);
            float x = (1.0f-prc) * -max_x_dist + prc * max_x_dist;
            Vector3 pos = new Vector3(x, -3, 0);
            spawn_positions.Add(pos);
        }

        //list of possible colors
        int num_color_sets = (int)Mathf.Ceil( (float)num_plants_to_spawn / (float)colors.Length );
        List<Color> possible_colors = new List<Color>();
        for (int i=0; i<num_color_sets; i++){
            for (int k=0; k<colors.Length; k++){
                possible_colors.Add(colors[k]);
            }
        }

        //space it so we grow all the plants in the time allotted
        float time_spacing = total_time_to_rejuvenate / (float) num_plants_to_spawn;

        //spawn plants one by one
        for (int i=0; i<num_plants_to_spawn; i++){
            //grab one of the random positions
            int rand_pos_id = (int)Random.Range(0, spawn_positions.Count);
            Vector3 pos =spawn_positions [rand_pos_id];
            spawn_positions.RemoveAt(rand_pos_id);

            if (debug_even_spacing){
                pos = new Vector3(Random.Range(-max_x_dist, max_x_dist),-3,0);
                float prc = (float)i / (float)(num_plants_to_spawn-1);
                pos.x = (1.0f-prc)*-max_x_dist + prc * max_x_dist;
            }

            //grab a color
            int rand_color_id =  (int)Random.Range(0, possible_colors.Count);
            Color color = possible_colors[rand_color_id];
            possible_colors.RemoveAt(rand_color_id);

            PlantRootInfo info = possible_roots[ (int)Random.Range(0,possible_roots.Length)];
            if (debug_root_id >= 0){
                info = possible_roots[ debug_root_id ];
            }

            PlantRoot root = new PlantRoot(info, pos, i*20, color);

            root.start_growth_animation();
            roots.Add( root );

            yield return new WaitForSeconds(time_spacing);
        }

        //if we're testing automatically move to the game
        if (debug_use_fake_game_state){
            yield return new WaitForSeconds(rejuvenation_max_grow_time + rejuvenation_min_pause_time);
            debug_cur_state = Score.GameState.Flourishing;
        }
    }
}
