using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class InfoCursor : MonoBehaviour {

    // These public fields become settable properties in the Unity editor.
	public GameObject first;
    public GameObject second;
    public GameObject welcomeText;
    private bool showProfile = false;
    private bool flag = true;
    private int count = 0;
    private MeshRenderer meshRenderer;

	GestureRecognizer recognizer;

    // Use this for initialization
    void Start()
	{
        if (count == 0)
        {
            welcomeText.SetActive(true);
            count++;
        }

        second.SetActive(false);
        first.SetActive(false);

        // Grab the mesh renderer that's on the same object as this script.
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();

		// Set up a GestureRecognizer to detect Select gestures.
		recognizer = new GestureRecognizer();
		recognizer.TappedEvent += (source, tapCount, ray) =>
		{
            if (count > 0)
                welcomeText.SetActive(false);
			// Do a raycast into the world that will only hit the Spatial Mapping mesh.
			var headPosition = Camera.main.transform.position;
			var gazeDirection = Camera.main.transform.forward;

			RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                30.0f, SpatialMapping.PhysicsRaycastMask) && showProfile)
            {
                if (flag)
                {
                    // Move this object's parent object to
                    // where the raycast hit the Spatial Mapping mesh.
                    first.transform.position = hitInfo.point;

                    // Rotate this object's parent object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    first.transform.rotation = toQuat;

                    first.SetActive(true);
                    second.SetActive(false);
                }
                else
                {
                    // Move this object's parent object to
                    // where the raycast hit the Spatial Mapping mesh.
                    second.transform.position = hitInfo.point;

                    // Rotate this object's parent object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    second.transform.rotation = toQuat;
        
                    second.SetActive(true);
                    first.SetActive(false);
                }
                flag = !flag;
            }
            else
            {
                second.SetActive(false);
                first.SetActive(false);
            }
            showProfile = !showProfile;
        };
		recognizer.StartCapturingGestures();
	}

	// Update is called once per frame
	void Update()
	{
		// Do a raycast into the world based on the user's
		// head position and orientation.
		var headPosition = Camera.main.transform.position;
		var gazeDirection = Camera.main.transform.forward;

		RaycastHit hitInfo;

		if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
		{
			// If the raycast hit a hologram...
			// Display the cursor mesh.
			meshRenderer.enabled = true;

			// Move the cursor to the point where the raycast hit.
			this.transform.position = hitInfo.point;

			// Rotate the cursor to hug the surface of the hologram.
			this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
		}
		else
		{
			// If the raycast did not hit a hologram, hide the cursor mesh.
			meshRenderer.enabled = false;
		}
	}
}


