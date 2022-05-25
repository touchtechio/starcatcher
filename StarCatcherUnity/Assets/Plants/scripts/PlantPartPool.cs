using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPartPool : MonoBehaviour
{
    [System.Serializable]
   public struct LimbObj { public string id_name; public GameObject prefab; }
   public LimbObj[] limb_objs;


   public static PlantPartPool instance = null;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject get_limb(string limb_id){
        for (int i=0; i<limb_objs.Length; i++){
            if (limb_objs[i].id_name == limb_id){
                return limb_objs[i].prefab;
            }
        }
        Debug.Log("COULD NOT FIND:"+limb_id);
        return null;
    }
}
