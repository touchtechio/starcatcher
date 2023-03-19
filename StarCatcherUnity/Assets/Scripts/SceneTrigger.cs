using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("KEY PRESS: C");
            SwitchToLoadScene();
        }    
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("KEY PRESS: W");
            SwitchToWelcomeScene();
        }    

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("KEY PRESS: G");
            SwitchToGameScene();
        }    

        
    }

    public void SwitchToLoadScene() {
        Debug.Log("Change Scene");
        SceneManager.LoadScene("StarCatcher-load");
    }
    public void SwitchToWelcomeScene() {
        Debug.Log("Change Scene: StarCatcher-WelcomeScene");
        SceneManager.LoadScene("StarCatcher-WelcomeScene");
    }    public void SwitchToGameScene() {
        Debug.Log("Change Scene: StarCatcher-audienceChoreo ");
        SceneManager.LoadScene("StarCatcher-audienceChoreo");
    }
}
