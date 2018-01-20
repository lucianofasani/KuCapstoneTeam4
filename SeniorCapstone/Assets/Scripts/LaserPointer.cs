using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab; //Reference to laser prefab
    private GameObject laser; //stores a reference to an instance of the laser
    private Transform laserTransform; //transform component is stored
    private Vector3 hitPoint; //position where the laser and teleport reticle hit

    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset; //Reticle offset from the floor
    public LayerMask teleportMask; //Layer mask to filter areas which teleports are allowed, currently not being used but here for future use
    private bool shouldTeleport; //True when a valid teleport location is selected. Again, all surfaces are fair play right now

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    private void ShowLaser(RaycastHit hit)
    {
        //Show laser
        laser.SetActive(true);

        //Lerp is used because you give it two positions and the percent it should travel, 50% gives you the middle point.
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);

        //Laser is pointed to where the raycast hits
        laserTransform.LookAt(hitPoint);

        //Scales the laser to fit between the two positions
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private void Teleport()
    {
        //Doesn't allow you to teleport while teleporting is happening in order to avoid possible errors
        shouldTeleport = false;

        //Hides reticle
        reticle.SetActive(false);

        //difference between the cameraRig/playarea origin position and the player's head, this is so the player moves exactly where they want to
        //instead of the center of the cameraRig moving to the center of the reticle
        Vector3 difference = cameraRigTransform.position - headTransform.position;

        //Change the difference of the y position back to zero because the above equation doesn't account for the vertical position of the player's head
        //This could cause issues when wanting to teleport to floors of different heights but we'll find out when we get there.
        difference.y = 0;

        //Moves the cameraRig to the correct position
        cameraRigTransform.position = hitPoint + difference;
    }

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
    }

    // Use this for initialization
    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {

        if(controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            //Execute this block if the raycast actually hits something.
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point; //Stores the point where it hit
                ShowLaser(hit);
                reticle.SetActive(true); //Show teleport reticle
                teleportReticleTransform.position = hitPoint + teleportReticleOffset; //Adds offset to position raycast hit so you can actually see the reticle
                shouldTeleport = true;
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if(controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }


        
	}
}
