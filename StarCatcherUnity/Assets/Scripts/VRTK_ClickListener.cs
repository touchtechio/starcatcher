namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class VRTK_ClickListener : MonoBehaviour
    {
        StripPosition stripPositionObject;
        Scene currentScene;
        public Camera projectorLoadCamera;
        stripPositionLog stripPositionLogObject;

        private void Start()
        {

            currentScene = SceneManager.GetActiveScene();
            stripPositionObject = FindObjectOfType<StripPosition>();
            stripPositionLogObject = FindObjectOfType<stripPositionLog>();
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
                return;
            }

            GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);

      //      GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            GetComponent<VRTK_ControllerEvents>().ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);
            GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        }

        private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
        {
            VRTK_Logger.Info("Controller on index '" + index + "' " + button + " has been " + action
                    + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
        }


        // happiness
        private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "clicked", e);
            stripPositionObject.SetStripPosition(gameObject.transform.position); // pass through the position of controller
            stripPositionLogObject.LogStrips();
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "pressed down", e);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            projectorLoadCamera.enabled = false;
            if (currentScene.name == "StarCatcher")
            {
                SceneManager.LoadScene("StarCatcher-load");
            }
            else if (currentScene.name == "StarCatcher-load")
            {
                SceneManager.LoadScene("StarCatcher");

            }
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "released", e);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "released", e);
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "pressed", e);
            stripPositionObject.clearStripArray = true;
            stripPositionLogObject.clearStripArray = true;

        }
    }
}