using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This is a joint on the plant limb that can be used to spawn future limbs
*/

public class PlantConnection : MonoBehaviour
{

    public float scale;
    public float scale_range_prc;
    public float angle;
    public float angle_range = 5;

    public int die_off_depth = 1;

    //public GameObject[] possible_prefabs;
    public string[] possible_children;

    public bool filled;
    
    
    public PlantLimb spawn_child(PlantLimb parent){
        int rand_id = (int)Random.Range(0,possible_children.Length);
        GameObject prefab = PlantPartPool.instance.get_limb(possible_children[rand_id]);
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = parent.gameObject.transform;

        //float new_scale = scale * Random.Range(1.0f-scale_range_prc, 1.0f+scale_range_prc);

        //obj.transform.localScale = new Vector3(new_scale,new_scale,new_scale);

        

        PlantLimb child = obj.GetComponent<PlantLimb>();
        return child;
    }

    public float get_angle(){
        return angle + Random.Range(-angle_range, angle_range);
    }

    public float get_scale(){
        return scale * Random.Range(1.0f-scale_range_prc, 1.0f+scale_range_prc);
    }

    public bool get_spawn_roll(int depth, int max_depth){
        float chance = 1.0f;

        if (depth >= die_off_depth){
            chance = 1.0f - (float)(depth-die_off_depth) / (float)(max_depth-die_off_depth);
        }

        Debug.Log("depth of "+depth+" has spawn chance "+chance);

        return Random.Range(0.0f,1.0f) < chance; 

    }
}
