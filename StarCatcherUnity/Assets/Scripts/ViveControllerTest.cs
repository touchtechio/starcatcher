using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ViveControllerTest : MonoBehaviour
{
    // action set
    public SteamVR_ActionSet starcatcherControllerActionSet;
    //Hand to get (right hand or left hand or any hand)
    public SteamVR_Input_Sources handType;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
    public SteamVR_Action_Boolean SphereOnOff;
    public SteamVR_Action_Boolean SetStarPosition;
    public SteamVR_Action_Boolean ClearStarPositions;
    public SteamVR_Action_Boolean ChangeScene;
    public SteamVR_Action_Pose poseAction = null;
    Scene currentScene;
    public Camera projectorLoadCamera;

    //reference to the sphere
    public GameObject Sphere;
    public float speed;

    // reference strip position
    public StripPosition stripPosition;
    public GameObject butterflyNet;
    private Vector3 triggerPosition;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        starcatcherControllerActionSet.Activate();

        SetStarPosition.AddOnStateDownListener(TriggerDown, handType);
        ClearStarPositions.AddOnStateDownListener(GripDown, handType);
        ChangeScene.AddOnStateDownListener(MenuDown, handType);
    }

    void Update()
    {
        triggerPosition = butterflyNet.transform.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("KEY PRESS: space");
            SwitchScene();
        }    
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is down");
        Sphere.GetComponent<MeshRenderer>().enabled = false;
        stripPosition.SetStripPosition(triggerPosition);
        //Debug.Log("position"+ vPosition.x)
    }

    public void GripDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Clear Star Positions");
        stripPosition.clearStripArray = true;
        //stripPositionLogObject.clearStripArray = true;
    }

    public void MenuDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        SwitchScene();
    }

    public void SwitchScene() {
        Debug.Log("Change Scene");

        projectorLoadCamera.enabled = false;
        if (currentScene.name == "StarCatcher-audienceChoreo")
        {
            SceneManager.LoadScene("StarCatcher-load");
        }
        else if (currentScene.name == "StarCatcher-load")
        {
            SceneManager.LoadScene("StarCatcher-audienceChoreo");

        }
    }
}