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

    [System.NonSerialized]
    public Color color;

    private SoundManager soundManager;
    
    public PlantRoot(PlantManager.PlantRootInfo _info, Vector3 position, int z, Color _color){

        info = _info;
        color = _color;

        //get the possible children
        possible_children = null;
        if (info.plant_type == "cooksonia") possible_children = PlantManager.instance.possible_children_cooksonia;
        if (info.plant_type == "leafy")     possible_children = PlantManager.instance.possible_children_leafy;
        if (info.plant_type == "tall")     possible_children = PlantManager.instance.possible_children_tall;

        //generate some values
        health_curve = Random.Range(0.5f,1.5f);

        sway_speed = Random.Range(PlantManager.instance.min_sway_speed, PlantManager.instance.max_sway_speed);

        //spawn the root limb
        GameObject obj = Object.Instantiate(PlantPartPool.instance.get_limb(info.limb_id), Vector3.zero, Quaternion.identity);
        obj.transform.parent = PlantManager.instance.transform;
        obj.transform.localPosition = position;
        
        root_limb = obj.GetComponent<PlantLimb>();
        float angle = info.start_angle + Random.Range(-info.start_angle_range, info.start_angle_range);
        root_limb.set_as_root(z, angle, info.max_depth, this);

        soundManager = (SoundManager)Object.FindObjectOfType<SoundManager>();
    }

    public void set_health(float val){
        float health_val = Mathf.Pow(val, health_curve);
        root_limb.set_health( health_val );
    }

    public void start_death_animation(){
        root_limb.start_death_animation();

        soundManager.PlayPlantDieSound(root_limb.transform.position);
    }

    public void start_growth_animation(){
        root_limb.start_growth_animation();

        soundManager.PlayGrowSound(root_limb.transform.position);
    }

    public void kill(){
        Object.Destroy(root_limb.gameObject);
    }
}
