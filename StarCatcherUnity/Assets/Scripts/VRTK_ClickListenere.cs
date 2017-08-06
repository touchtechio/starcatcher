namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ClickListenere : MonoBehaviour
    {
        StripPosition stripPositionObject;

        private void Start()
        {
            stripPositionObject = FindObjectOfType<StripPosition>();
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
                return;
            }

            GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
       
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
            stripPositionObject.SetStripPosition(gameObject.transform.position);
        }

    }
}