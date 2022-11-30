using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

This is a singleton class dedicated to grabbing plant limb prefabs

I cannot instantiate the limbs directly form inside their prefab because the recursive nature causes the original prefab to get the children
I'm not sure exactly why this is, but setting up a pooler like this seems to solve it

*/

public class PlantPartPool : MonoBehaviour
{
    [System.Serializable]
   public struct LimbObj { 
       public string id_name; 
       public GameObject prefab; 
    }

   public LimbObj[] limb_objs;


   public static PlantPartPool instance = null;

    void Awake(){
        if (instance == null){
            instance = this;
        }else if (instance != this) {
            Destroy (gameObject);
        }
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
