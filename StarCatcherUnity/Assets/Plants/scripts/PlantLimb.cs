using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System.Linq;

/*
This is a plant limb. These are combined recursively to make a plant
*/


public class PlantLimb : MonoBehaviour
{

    private int depth;
    private int max_depth;

    private float base_angle;
    private float base_scale;

    float shrink_start;
    float shrink_end;

    [Tooltip("if true, 50% chance of flipping the sprite")]
    public bool can_flip_x;

    [Tooltip("every other depth will be flipped. overrides can_flip_x")]
    public bool alternate_flip_x;   
    private bool flip_x;

    public float extra_sway_dist_range;
    public float extra_sway_speed_range;
    private float extra_sway_dist, extra_sway_speed;


    public List<PlantConnection> connections;
    public List<PlantLimb> children = new List<PlantLimb>();

    public SpriteRenderer sprite_rend;

    private PlantRoot root;


    public void set_as_root(int z, float angle, int _max_depth, PlantRoot _root){
        float root_scale = 1 + Random.Range(0, _root.info.max_bonus_scale);
        setup(0, angle, root_scale, z, _max_depth, _root);
    }

    public void setup(int _depth, float _base_angle, float _base_scale, int z, int _max_depth, PlantRoot _root){
        root = _root;

        depth = _depth;
        max_depth = _max_depth;
        base_angle = _base_angle;

        //briefly setting it to the scale it will ultimatly be at so children get placed correctly
        base_scale = _base_scale;
        transform.localScale = new Vector3(base_scale, base_scale, base_scale);

        flip_x = false;
        if (can_flip_x){
            flip_x = Random.Range(0.0f, 1.0f) > 0.5f;

            //if this is root, flip the angle
            if (flip_x && depth == 0){
                base_angle *= -1;
            }
        }

        if (alternate_flip_x){
            if (depth == 0) flip_x = false;
            else            flip_x = true;
        }

        //sanity check to prevent infinite loops
        //this happens if you don't have terminal limbs
        if (depth > 50){
            Debug.Log("<color=red>UNBOUDNED PLANT DEPTH!!!</color>");
            return;
        }

        gameObject.name = root.info.plant_type +" depth  "+depth.ToString();

        set_health_values();

        sprite_rend.color = root.color;

        //demo coloring
        if (PlantManager.instance.use_debug_sprite_color){
            sprite_rend.color = Color.Lerp( Color.red, Color.blue, (float)depth / (float)max_depth);
        }

        //setting Z depth so later limbs are a little bit in front of their parents
        sprite_rend.sortingOrder = z - depth;

        //try to spawn children
        float depth_prc = (float)depth / (float)(max_depth-1);

        //spawn a child for each connection
        foreach(PlantConnection con in connections){
            //get the list of children
            PlantManager.ChildInfo[] possible_children = root.possible_children;
            //check if this connection overrides it
            if (con.possible_children_override.Length > 0){
                possible_children = con.possible_children_override;
            }

            string child_id = select_from_possible_children(depth_prc, possible_children);
            GameObject prefab = PlantPartPool.instance.get_limb(child_id);

            PlantLimb child = con.spawn_child(this, prefab);
            child.setup(depth + 1, con.get_angle(), con.get_scale(), z, max_depth, root);
            children.Add(child);
        }

        //set the sway
        extra_sway_dist = Random.Range(0,extra_sway_dist_range);
        extra_sway_speed = Random.Range(-extra_sway_speed_range,extra_sway_speed_range);

        //set the scale to 0 so we can animate in
        transform.localScale = Vector3.zero;

    }

    public string select_from_possible_children(float depth_prc, PlantManager.ChildInfo[] possible_children){
        Dictionary<string, float> choices = new Dictionary<string, float>();
        float total_weight = 0;
        foreach( PlantManager.ChildInfo info in possible_children){
            float weight = (1.0f-depth_prc) * info.weight_start + depth_prc * info.weight_end;
            if (weight < 0) weight = 0;
            choices.Add(info.id_name, weight);
            total_weight += weight;
        }

        float roll = Random.Range(0,total_weight);
        foreach(KeyValuePair<string, float> entry in choices)
        {
            roll -= entry.Value;
            if (roll <= 0){
                return entry.Key;
            }
        }

        //we should not get here
        Debug.Log("BAD BAD BAD!!!!");
        return "NO ID";
    }

    // Update is called once per frame
    void Update()
    {
        //sway in the breeze
        float sway_angle = Mathf.Sin(Time.time * (root.sway_speed + extra_sway_speed)) * (PlantManager.instance.sway_dist + extra_sway_dist);
        transform.localEulerAngles = new Vector3(0,0, base_angle + sway_angle);
    }

    private void set_health_values(){
        float depth_prc_step = 1.0f / ((float)(max_depth)+1.5f);
        float base_window = depth_prc_step * 0.65f;
        float health_scale_wiggle = depth_prc_step * 0.2f;
        float shrink_center = depth_prc_step * (float)(depth+1);
        
        shrink_start = shrink_center + base_window + Random.Range(-health_scale_wiggle,health_scale_wiggle);
        shrink_end = shrink_center - base_window + Random.Range(-health_scale_wiggle,health_scale_wiggle);

        shrink_start = Mathf.Min(shrink_start, 1.0f);
        shrink_end = Mathf.Max(shrink_end, 0.0f);
    }

    //propigate health down the tree
    public void set_health(float health){
        float cur_prc = (health - shrink_end) / (shrink_start - shrink_end);
        cur_prc = Mathf.Max(0.0f, Mathf.Min(cur_prc, 1.0f));

        float cur_scale = base_scale * cur_prc;
        float cur_scale_x = cur_scale;
        if (flip_x) cur_scale_x *= -1;

        transform.localScale = new Vector3(cur_scale_x, cur_scale, cur_scale);
        
        foreach(PlantLimb child in children){
            child.set_health(health);
        }
    }


    public void start_death_animation(){
        float pause_time = Random.Range(PlantManager.instance.min_death_pause_time, PlantManager.instance.max_death_pause_time);
        float shrink_time = Random.Range(PlantManager.instance.min_death_shrink_time, PlantManager.instance.max_death_shrink_time);
        StartCoroutine(do_death(pause_time,shrink_time));
        foreach(PlantLimb child in children){
            child.start_death_animation();
        }
    }

    IEnumerator do_death(float pause_time, float die_time){
        yield return new WaitForSeconds(pause_time);

        Vector3 start_scale = transform.localScale;

        float timer = 0;
        while (timer < die_time){
            timer += Time.deltaTime;
            float prc = 1.0f - (timer / die_time);

            transform.localScale = start_scale * prc;

            yield return null;
        }

        transform.localScale = Vector3.zero;
    }


    public void start_growth_animation(){
        float pause_time = Random.Range(PlantManager.instance.rejuvenation_min_pause_time, PlantManager.instance.rejuvenation_max_pause_time);;
        float grow_time = Random.Range(PlantManager.instance.rejuvenation_min_grow_time, PlantManager.instance.rejuvenation_max_grow_time);
        StartCoroutine(do_growth(pause_time, grow_time));
        foreach(PlantLimb child in children){
            child.start_growth_animation();
        }
    }
    IEnumerator do_growth(float pause_time, float grow_time){
        yield return new WaitForSeconds(pause_time);

        float scale_x = base_scale;
        if (flip_x) scale_x *= -1;
        Vector3 end_scale = new Vector3(scale_x, base_scale,base_scale);

        float timer = 0;
        while (timer < grow_time){
            timer += Time.deltaTime;
            float prc = (timer / grow_time);

            transform.localScale = end_scale * prc;

            yield return null;
        }

        transform.localScale = end_scale;
    }


}
