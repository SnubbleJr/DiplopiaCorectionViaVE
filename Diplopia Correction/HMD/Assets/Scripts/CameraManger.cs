using UnityEngine;
using System.Collections;

public class CameraManger : MonoBehaviour {
    //sets the correct fov for each rendering camera

    public Camera referenceCamera;
    public Camera leftRenderCamera, rightRenderCamera;

    private static CameraManger instance;

    public static CameraManger Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<CameraManger>();
            return instance;
        }
    }

    private float initialFOV;

    void Start()
    {
        if (leftRenderCamera != null)
            initialFOV = leftRenderCamera.fieldOfView;
    }

	void Update ()
    {
	    if (referenceCamera != null && leftRenderCamera != null && rightRenderCamera != null && referenceCamera.gameObject.activeSelf && referenceCamera.fieldOfView != initialFOV)
        {
            leftRenderCamera.fieldOfView = referenceCamera.fieldOfView;
            rightRenderCamera.fieldOfView = referenceCamera.fieldOfView;
            referenceCamera.gameObject.SetActive(false);
        }
	
	}
}
