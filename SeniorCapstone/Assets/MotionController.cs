using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour {

    
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private GameObject pickup;


    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {

        if(controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if(controller.GetPressDown(gripButton) && pickup != null)
        {
            pickup.transform.parent = this.transform;
            pickup.GetComponent<Rigidbody>().useGravity = false; //This allows the object to be freely picked up and moved around.
        }

        if(controller.GetPressUp(gripButton) && pickup != null)
        {
            pickup.transform.parent = null;
            pickup.GetComponent<Rigidbody>().useGravity = true; //Make the held object be affected by gravity again.
        }
    
    }

    /*
     * Tracks when the controller goes into a space that contains a collider
     */
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger entered");
        pickup = collider.gameObject;
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("Trigger exit");
        pickup = null;
    }


    

}
