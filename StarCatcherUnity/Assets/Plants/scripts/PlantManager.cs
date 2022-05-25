using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Manager class that runs the plants
*/

public class PlantManager : MonoBehaviour
{
    public string[] root_ids;

    private List<PlantRoot> roots = new List<PlantRoot>();

    public int max_depth;

    public bool use_debug_sprite_color;


    public static PlantManager instance = null;

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
        reset();
    }

    void reset(){
        for (int i=roots.Count-1; i>=0; i--){
            roots[i].kill();
        }
        roots.Clear();

        for (int i=0; i<3; i++){
            Vector3 pos = new Vector3(3.5f*(i-1),-3,0);
            string root_id = root_ids[ (int)Random.Range(0,root_ids.Length)];
            roots.Add( new PlantRoot(root_id, pos));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            reset();
        }

        float health_value = 0.9f;//health_slider.value;

        for (int i=0; i<roots.Count; i++){
            roots[i].set_health( health_value );
        }
    }
}
