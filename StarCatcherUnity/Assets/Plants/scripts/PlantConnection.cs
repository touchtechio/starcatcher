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

    [Tooltip("overrides the normal children list for this plant")]
    public PlantManager.ChildInfo[] possible_children_override;

    
    public PlantLimb spawn_child(PlantLimb parent, GameObject prefab){

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = parent.gameObject.transform;

        PlantLimb child = obj.GetComponent<PlantLimb>();
        return child;
    }

    public float get_angle(){
        return angle + Random.Range(-angle_range, angle_range);
    }

    public float get_scale(){
        return scale * Random.Range(1.0f-scale_range_prc, 1.0f+scale_range_prc);
    }
}
