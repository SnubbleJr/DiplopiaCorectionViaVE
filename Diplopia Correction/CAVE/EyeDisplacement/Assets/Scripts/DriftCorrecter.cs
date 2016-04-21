using UnityEngine;
using MiddleVR_Unity3D;
using System.Collections;

public enum DriftCorrectionType
{
    Binoculuar,
    BinocularDominant,
    Monocular,
}

public class DriftCorrecter : MonoBehaviour
{
    //show a cube when trigger is held down
    //find the rotation needed to look at that marker spawned infront of the palyer
    //and set the drift correciton for EyeGazeRayCastManger

    public DriftCorrectionType corrrectionType;

    public GameObject driftCorrectionPrefab;

    private GameObject dcMarker;
    private GameObject leftDCEye, rightDCEye;           //used to find the eye rotation needed to look at the DC marker
    public Vector2 spawnPoint = Vector2.zero;          //infront of the user, local space

    private bool correctingLeftEye = true;              //used for DC when monocular
    private bool dcCleared = false;

    private int leftEyeMask, rightEyeMask;

    private EyeGazeRayCasterManager casterManager;

    void Start()
    {
        casterManager = GetComponent<EyeGazeRayCasterManager>();
        setupDCEyes();
        setupCullingMasks();
    }

    //create new gaze casting children at the user's eyes
    private void setupDCEyes()
    {
        Transform leftCamera = GameObject.Find("FrontCameraStereo.Left").transform;
        Transform rightCamera = GameObject.Find("FrontCameraStereo.Right").transform;

        leftDCEye = new GameObject("leftDCEye");
        rightDCEye = new GameObject("rightDCEye");

        leftDCEye.transform.SetParent(this.transform, false);
        rightDCEye.transform.SetParent(this.transform, false);

        leftDCEye.transform.localPosition = leftCamera.localPosition;
        rightDCEye.transform.localPosition = rightCamera.localPosition;

        //and spawn the DC marker
        dcMarker = Instantiate(driftCorrectionPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        dcMarker.transform.SetParent(transform, false);
        //needs to be transform.right in cave (unless i set orientation)
        //or use middlevr converter
        dcMarker.transform.localPosition = (Vector3)spawnPoint + transform.right;
        dcMarker.SetActive(false);
    }

    //attempts to get the culling masks for each eye
    //and attemps to find the cameras and sets their culling mask
    private void setupCullingMasks()
    {
        leftEyeMask = LayerMask.NameToLayer("LeftEyeMask");
        rightEyeMask = LayerMask.NameToLayer("RightEyeMask");

        if (leftEyeMask == -1 || rightEyeMask == -1)
        {
            Debug.LogError("LeftEyeMask and/or RightEyeMask layers have not been set up!");
            return;
        }

        foreach (Camera camera in FindObjectsOfType(typeof(Camera)) as Camera[])
            if (camera.gameObject.name.Contains(".Left"))
            {
                camera.cullingMask |= 1 << leftEyeMask;
                camera.cullingMask &= ~(1 << rightEyeMask);
            }
            else if (camera.gameObject.name.Contains(".Right"))
            {
                camera.cullingMask |= 1 << rightEyeMask;
                camera.cullingMask &= ~(1 << leftEyeMask);
            }
    }

    void Update()
    {
        debugDCEyes();
        
        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(0))
            startDC();

        //when held, another button can also clear
        if (MiddleVR.VRDeviceMgr.IsWandButtonPressed(0))
        {
            if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(3))
                clearDC();

            //if monocular, button can toggle between left and right
            if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(4))
                switchMonocularEye();
        }

        //stop if trigger is released
        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(0, false))
            stopDC();
    }

    //sets the DC marker's colour based on which eye we are correcting
    //and what form of correction we are using
    private void setDCMarkerColor()
    {
        Color color;

        if (corrrectionType == DriftCorrectionType.Monocular)
            if (correctingLeftEye)
                color = Color.green;
            else
                color = Color.red;
        else
            color = Color.black;

        dcMarker.GetComponent<Renderer>().material.color = color;
    }

    //sets the DC marker's layer based on which eye which eye we are correcting
    //and what form of correciton we are using
    private void setDCMarkerLayer()
    {
        int layer;

        if (corrrectionType == DriftCorrectionType.Monocular)
            if (correctingLeftEye)
                layer = leftEyeMask;
            else
                layer = rightEyeMask;
        else
            layer = LayerMask.NameToLayer("Default");

        dcMarker.layer = layer;
        foreach (Transform child in dcMarker.transform)
            child.gameObject.layer = layer;
    }

    //make DC eyes look at the marker
    private void setDCEyeRotations()
    {
        leftDCEye.transform.LookAt(dcMarker.transform.position);
        rightDCEye.transform.LookAt(dcMarker.transform.position);
    }

    private void debugDCEyes()
    {
        Debug.DrawRay(leftDCEye.transform.position, leftDCEye.transform.forward * 50, Color.yellow);
        Debug.DrawRay(rightDCEye.transform.position, rightDCEye.transform.forward * 50, Color.yellow);
    }
    
    //toggles the current monocular eye for DC
    //and sets the DC value for the other eye
    public void switchMonocularEye()
    {
        if (corrrectionType != DriftCorrectionType.Monocular)
            return;
        
        setDC();
        correctingLeftEye = !correctingLeftEye;
        setDCMarkerColor();
        setDCMarkerLayer();
    }
    
    public void startDC()
    {
        //activate marker and set DC to zero for correction
        dcMarker.SetActive(true);
        setDCMarkerColor();
        setDCMarkerLayer();

        casterManager.unsetDriftCorrection();

        setDCEyeRotations();
        dcCleared = false;
    }

    //updates the values for drift correction
    public void setDC()
    {
        switch (corrrectionType)
        {
            case DriftCorrectionType.Binoculuar:
                casterManager.setBinocularDriftCorrection(leftDCEye.transform.localEulerAngles, rightDCEye.transform.localRotation.eulerAngles);
                break;
            case DriftCorrectionType.BinocularDominant:
                casterManager.setDominantDriftCorrection(leftDCEye.transform.localEulerAngles, rightDCEye.transform.localRotation.eulerAngles);
                break;
            case DriftCorrectionType.Monocular:
                if (correctingLeftEye)
                    casterManager.setLeftDriftCorrection(leftDCEye.transform.localEulerAngles);
                else
                    casterManager.setRightDriftCorrection(rightDCEye.transform.localEulerAngles);
                break;
        }
    }
    
    public void stopDC()
    {
        //update values if not cancled
        if(!dcCleared)
            setDC();

        //deactivate marker
        dcMarker.SetActive(false);
        casterManager.finishedDriftCorrection();
    }
    
    public void clearDC()
    {
        //stop DC and then clear drift
        dcCleared = true;
        stopDC();
    }
}
