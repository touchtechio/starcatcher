using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This class represents the root of the plant and is used ro propoigate information down the whole plant
(things like health, color etc)
*/

public class PlantRoot 
{

   

    private PlantLimb root_limb;

    private float health_curve;

    [System.NonSerialized]
    public float sway_speed;

    [System.NonSerialized]
    public PlantManager.ChildInfo[] possible_children;

    [System.NonSerialized]
    public PlantManager.PlantRootInfo info;
    
    
    public PlantRoot(PlantManager.PlantRootInfo _info, Vector3 position, int z){

        info = _info;
        
        //get the possible children
        possible_children = null;
        if (info.plant_type == "cooksonia") possible_children = PlantManager.instance.possible_children_cooksonia;

        health_curve = Random.Range(0.5f,1.5f);

        sway_speed = Random.Range(PlantManager.instance.min_sway_speed, PlantManager.instance.max_sway_speed);

        GameObject obj = Object.Instantiate(PlantPartPool.instance.get_limb(info.limb_id), Vector3.zero, Quaternion.identity);
        obj.transform.parent = PlantManager.instance.transform;
        obj.transform.localPosition = position;
        root_limb = obj.GetComponent<PlantLimb>();
        root_limb.set_as_root(z, info.start_angle, info.max_depth, this);
        //root_limb.setup(0, 0, 1, z, color, root_limb.max_depth_if_trunk);
    }

    public void set_health(float val){
        float health_val = Mathf.Pow(val, health_curve);
        root_limb.set_health( health_val );
    }

    public void start_death_animation(){
        root_limb.start_death_animation();
    }

    public void kill(){
        Object.Destroy(root_limb.gameObject);
    }
}
