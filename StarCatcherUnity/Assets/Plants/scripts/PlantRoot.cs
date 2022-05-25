using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantRoot 
{

    private PlantLimb root_limb;

    private float health_curve;
    private Color color;

    public PlantRoot(GameObject limb_prefab, Vector3 position){
        health_curve = Random.Range(0.5f,1.5f);

        color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));

        int z = (int)Random.Range(0,99);

        GameObject obj = Object.Instantiate(limb_prefab, Vector3.zero, Quaternion.identity);
        obj.transform.parent = PlantManager.instance.transform;
        obj.transform.localPosition = position;
        root_limb = obj.GetComponent<PlantLimb>();
        root_limb.setup(0, 0, 1, z, color, PlantManager.instance.max_depth);
    }

    public void set_health(float val){
        float health_val = Mathf.Pow(val, health_curve);
        root_limb.set_health( health_val );
    }

    public void kill(){
        Object.Destroy(root_limb.gameObject);
    }
}
