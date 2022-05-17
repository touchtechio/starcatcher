using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class ViveControllerTest : MonoBehaviour
{
    // action set
    public SteamVR_ActionSet starcatcherControllerActionSet;
    //Hand to get (right hand or left hand or any hand)
    public SteamVR_Input_Sources handType;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
    public SteamVR_Action_Vector2 AnalogStickLeft;
    public SteamVR_Action_Boolean SphereOnOff;
    public SteamVR_Action_Pose poseAction = null;
    public GameObject leftController;
    //SteamVR_Action_Pose poseActionL;
    //poseActionL = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose_left_tip");
    //vPosition = poseActionR[SteamVR_Input_Sources.LeftHand].localPosition;
    //qRotation = poseActionR[SteamVR_Input_Sources.LeftHand].localRotation;

    //reference to the sphere
    public GameObject Sphere;
    public float speed;

    // reference strip position
    public StripPosition stripPosition;
    public GameObject butterflyNet;
    private Vector3 triggerPosition;

    void Start()
    {
        starcatcherControllerActionSet.Activate();
        SphereOnOff.AddOnStateDownListener(TriggerDown, handType);
        SphereOnOff.AddOnStateUpListener(TriggerUp, handType);
    }

    void Update()
    {
        //Debug.Log("position"+leftController.transform.position.x);
        triggerPosition = butterflyNet.transform.position;
        //Vector2 stickVector = AnalogStickLeft.GetAxis(handType);
        //player.transform.position += speed * new Vector3(stickVector.x, 0, stickVector.y);
        //Debug.Log("stickVector: " + stickVector.x + ": " + stickVector.y);
        //Debug.Log("tringger: " + SphereOnOff.GetState(handType));
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
        Sphere.GetComponent<MeshRenderer>().enabled = true;
        
    }
    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is down");
        Sphere.GetComponent<MeshRenderer>().enabled = false;
        stripPosition.SetStripPosition(triggerPosition);
        //Debug.Log("position"+ vPosition.x)
    }

}