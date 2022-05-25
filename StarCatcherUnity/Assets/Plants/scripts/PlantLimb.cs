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
    public int max_depth_if_trunk;

    private float base_angle;
    private float base_scale;

    float shrink_start;
    float shrink_end;

    public float sway_dist;
    public float sway_speed;

    public List<PlantConnection> connections;
    public List<PlantLimb> children = new List<PlantLimb>();

    public SpriteRenderer sprite_rend;

    private Color base_color;


    public void setup(int _depth, float _base_angle, float _base_scale, int z, Color _color, int _max_depth){
        depth = _depth;
        max_depth = _max_depth;
        base_angle = _base_angle;

        base_scale = _base_scale;
        transform.localScale = new Vector3(base_scale, base_scale, base_scale);

        base_color = _color;
        sprite_rend.color = base_color;

        //Debug.Log("set up with depth "+depth);
        gameObject.name = "Limb  "+depth.ToString();

        set_health_values();

        //demo coloring
        if (PlantManager.instance.use_debug_sprite_color){
            sprite_rend.color = Color.Lerp( Color.red, Color.blue, (float)depth / (float)max_depth);
        }

        //setting Z depth so later limbs are a little bit in front of their parents
        sprite_rend.sortingOrder = z + depth;

        //try to spawn children
        if (depth < max_depth){
            foreach(PlantConnection con in connections){
                if (con.get_spawn_roll(depth, max_depth)){
                    PlantLimb child = con.spawn_child(this);
                    child.setup(depth + 1, con.get_angle(), con.get_scale(), z, base_color, max_depth);
                    children.Add(child);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //sway in the breeze
        float sway_angle = Mathf.Sin(Time.time * sway_speed) * sway_dist;
        transform.localEulerAngles = new Vector3(0,0, base_angle + sway_angle);

    }

    private void set_health_values(){
        float depth_prc_step = 1.0f / ((float)(max_depth)+1.5f);
        float health_scale_wiggle = depth_prc_step * 0.2f;
        float shrink_center = depth_prc_step * (float)(depth+1);
        
        shrink_start = shrink_center + depth_prc_step/2 + Random.Range(-health_scale_wiggle,health_scale_wiggle);
        shrink_end = shrink_center - depth_prc_step/2 + Random.Range(-health_scale_wiggle,health_scale_wiggle);

        shrink_start = Mathf.Min(shrink_start, 1.0f);
        shrink_end = Mathf.Min(shrink_end, 0.0f);
    }

    //propigate health down the tree
    public void set_health(float health){

        

        float cur_prc = (health - shrink_end) / (shrink_start - shrink_end);
        cur_prc = Mathf.Max(0.0f, Mathf.Min(cur_prc, 1.0f));

        //Debug.Log("depth: "+depth+"  cur "+cur_prc);


        float cur_scale = base_scale * cur_prc;
        transform.localScale = new Vector3(cur_scale, cur_scale, cur_scale);

        foreach(PlantLimb child in children){
            child.set_health(health);
        }
    }


}
