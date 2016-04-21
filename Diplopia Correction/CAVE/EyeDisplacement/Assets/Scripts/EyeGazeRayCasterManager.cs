using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

[RequireComponent(typeof(EyeLinkDataConverter))]
[RequireComponent(typeof(CaveViewPortManager))]
//[RequireComponent(typeof(EyeGazeDataLogger))]
[RequireComponent(typeof(TCPSocketListener))]
[RequireComponent(typeof(DriftCorrecter))]

public class EyeGazeRayCasterManager : MonoBehaviour
{
    //THIS ASSUMES WE ARE LOOKING AT COLLIDERS
    //MIGHT IMPLIMENT A TRIANGLE FORM LATER
    //BUT WHAT ABOUT SPRITES?
    //Manages the calculators, checking to see what objects are infront of them, and moving them to corrected points 

    //the chain for this whole thing is:
    //eyelink sends the eye positions, and data neede to caluate their rotatoin, to eyelinkDataConverter
    //eyelinkdataConverter calculates the rotational data and sends it to eyeGazeRayCasterManager
    //eyeGazeRayCasterManger sets the rotation of the subserviant eyeGazeRayCaster to the correct position
    //eyeGazeRayCasterManger then calculates the rotation needed to move the gaze ray to equal the caster and sends it the to caveViewportManger
    //caveViewportManger then sets the rotation of the corresponding veiwport cube
    //eyeGazeRayCasterManger gives this info eyeGazeDataLogger to save

    public bool testInput;                   //set to activate test input and output
    public bool useTestObjects;
    public GameObject testObjects;      //the group of objects used for testing

    public Material lineMaterial;

    public bool leftEyeDominant = true;
    public bool logGazeData;                    //indicate IF we want to log
    private bool startLogging = false;          //indicat WHEN we want to log, assuming we do

    public GameObject markerPrefab;

    private GameObject marker;

    private EyeGazeRayCaster leftAlteredEye, rightAlteredEye;       //we change these to the corrected rotation
    private EyeGazeRayCaster leftUnalteredEye, rightUnalteredEye;   //used to compare and calculate the correction rotatoin
    
    public Vector2 leftEyeRotation { private get; set; }            //the rotational x,y coordinates of the eye
    public Vector2 rightEyeRotation { private get; set; }           //the rotational x,y coordinates of the eye

    private Vector2 leftDriftCorrection;                            //the drift correction of the eyes
    private Vector2 rightDriftCorrection;                           //the drift correction of the eyes

    private CaveViewPortManager caveViewPortManager;
    private EyeGazeDataLogger dataLogger;

    public bool moveViewPort = true;                               //indicator of whether we should actually move the viewport

    void Awake()
    {
        if (testInput)
        {
            gameObject.AddComponent<TestInputScript>();
            gameObject.AddComponent<TestOutputScript>();
        }

        testObjects.SetActive(useTestObjects);

        linkToHeadNode();
        setupGazeCasters();

        //correct our rotation to work with mvr
        transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 180));

        caveViewPortManager = GetComponent<CaveViewPortManager>();
        
        if(logGazeData)
            dataLogger = gameObject.AddComponent<EyeGazeDataLogger>();

        marker = Instantiate(markerPrefab);
    }
    
    //create new gaze casting children at the user's eyes
    private void setupGazeCasters()
    {
        Transform leftCamera = GameObject.Find("FrontCameraStereo.Left").transform;
        Transform rightCamera = GameObject.Find("FrontCameraStereo.Right").transform;

        leftAlteredEye = new GameObject("leftAlteredEye").AddComponent<EyeGazeRayCaster>();
        rightAlteredEye = new GameObject("rightAlteredEye").AddComponent<EyeGazeRayCaster>();
        leftUnalteredEye = new GameObject("leftUnalteredEye").AddComponent<EyeGazeRayCaster>();
        rightUnalteredEye = new GameObject("rightUalteredEye").AddComponent<EyeGazeRayCaster>();

        leftAlteredEye.setLineMaterial(lineMaterial);
        rightAlteredEye.setLineMaterial(lineMaterial);
        leftUnalteredEye.setLineMaterial(lineMaterial);
        rightUnalteredEye.setLineMaterial(lineMaterial);

        leftAlteredEye.transform.SetParent(this.transform, false);
        rightAlteredEye.transform.SetParent(this.transform, false);
        leftUnalteredEye.transform.SetParent(this.transform, false);
        rightUnalteredEye.transform.SetParent(this.transform, false);

        leftAlteredEye.transform.localPosition = leftCamera.localPosition;
        leftUnalteredEye.transform.localPosition = leftCamera.localPosition;

        rightAlteredEye.transform.localPosition = rightCamera.localPosition;
        rightUnalteredEye.transform.localPosition = rightCamera.localPosition;
    }

    private void linkToHeadNode()
    {
        transform.SetParent(GameObject.Find("HeadNode").transform, false);
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        updateGazeRotation();
        
        EyeGazeRayCaster dominantEye, subserviantEye, subserviantEyeUnaltered;
        Vector2 dominantEyeGaze;

        if (leftEyeDominant)
        {
            dominantEye = leftAlteredEye;
            subserviantEye = rightAlteredEye;
            dominantEyeGaze = leftEyeRotation;
            subserviantEyeUnaltered = rightUnalteredEye;
        }
        else
        {
            dominantEye = rightAlteredEye;
            subserviantEye = leftAlteredEye;
            dominantEyeGaze = rightEyeRotation;
            subserviantEyeUnaltered = leftUnalteredEye;
        }

        RaycastHit rayCastHit = generatedAlteredGaze(dominantEye, subserviantEye);

        Vector3 rotationalDifference = calculateDisparagy(subserviantEye, subserviantEyeUnaltered);
        
        //move the subservient camera, so flip the dominance
        if (moveViewPort)
            moveCamera(leftEyeDominant, rotationalDifference);

        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(2, true))
            startLogging = true;

        if (logGazeData && startLogging)
            logData(rayCastHit.distance, subserviantEye.transform.localRotation);

        //debug
        marker.transform.position = rayCastHit.point;

        printOut(leftAlteredEye.transform.localEulerAngles.ToString(), rightAlteredEye.transform.localEulerAngles.ToString());

        /*
        leftAlteredEye.debugDraw(rayCastHit.point, rayCastHit.distance, Color.blue);
        rightAlteredEye.debugDraw(rayCastHit.point, rayCastHit.distance, Color.red);
        leftUnalteredEye.debugDraw(leftUnalteredEye.transform.position, rayCastHit.distance * 2, Color.cyan);
        rightUnalteredEye.debugDraw(rightUnalteredEye.transform.position, rayCastHit.distance * 2, Color.magenta);
        */
    }

    //set the gaze rotatoin for all of the ray casters
    private void updateGazeRotation()
    {
        leftUnalteredEye.transform.localRotation = Quaternion.Euler(leftEyeRotation - leftDriftCorrection);
        rightUnalteredEye.transform.localRotation = Quaternion.Euler(rightEyeRotation - rightDriftCorrection);
        leftAlteredEye.transform.localRotation = Quaternion.Euler(leftEyeRotation - leftDriftCorrection);
        rightAlteredEye.transform.localRotation = Quaternion.Euler(rightEyeRotation - rightDriftCorrection);
    }

    //moves the altered gaze ray casters to both look at the correct point
    //also returns the RaycastHit of the object we are looking at
    private RaycastHit generatedAlteredGaze(EyeGazeRayCaster dominantEye, EyeGazeRayCaster subserviantEye)
    {
        RaycastHit dominantRaycastHit;

        //if we hit something
        if (dominantEye.rayCast(dominantEye.transform.forward, out dominantRaycastHit))
        {
            Quaternion previousRotatoin = subserviantEye.transform.localRotation;

            //set the subserviant eye to look at that point
            subserviantEye.transform.LookAt(dominantRaycastHit.point);

            RaycastHit rayCastHit;

            //if we hit anything
            if (subserviantEye.rayCast(subserviantEye.transform.forward, out rayCastHit))
                //check to see if ray castr towards selected point gives the same point (round a bout)
                if (Vector3.SqrMagnitude(rayCastHit.point - dominantRaycastHit.point) > 0.0000001f)
                    //and reset the eye if that isn't the case
                    subserviantEye.transform.localRotation = previousRotatoin;
        }

        return dominantRaycastHit;
    }

    //calculate the disparagy between the altered and unaltered gaze, and return the rotation needed for that to work
    private Vector3 calculateDisparagy(EyeGazeRayCaster alteredRay, EyeGazeRayCaster unalteredRay)
    {
        return alteredRay.transform.localEulerAngles - unalteredRay.transform.localEulerAngles;
    }

    //move the subservient camera by the subservient eye rotatoin
    private void moveCamera(bool moveRightEye, Vector3 rotation)
    {
        caveViewPortManager.setScreenRotation(moveRightEye, rotation);
    }

    private void logData(float depth, Quaternion subserviantRotatoin)
    {
        if (dataLogger) 
            dataLogger.logData(leftEyeDominant, depth, leftUnalteredEye.transform.localRotation.eulerAngles, rightUnalteredEye.transform.localRotation.eulerAngles, subserviantRotatoin.eulerAngles);
    }

    public void printOut(string leftString, string rightString)
    {
        caveViewPortManager.printOut(leftString, rightString);
    }

    //set the rotational drift correction 
    //arguments are the expected eye rotation
    //sets the same DC based on the dominant eye
    //also reenables moving of the viewport
    public void setDominantDriftCorrection(Vector2 leftEye, Vector2 rightEye)
    {
        //this assumes the same ammount of drift correction is needed for both eyes
        //(head band slippage and not camera movement)

        //sets the drift correction of the dominant eye

        Vector2 newDC;

        if(leftEyeDominant)
            newDC = leftEyeRotation - leftEye;
        else
            newDC = rightEyeRotation - rightEye;

        leftDriftCorrection = newDC;
        rightDriftCorrection = newDC;

        finishedDriftCorrection();
    }

    //set the rotational drift correction 
    //arguments are the expected eye rotation
    //also reenables moving of the viewport
    public void setBinocularDriftCorrection(Vector2 leftEye, Vector2 rightEye)
    {
        //assumes eyes can focus on the same point

        leftDriftCorrection = leftEyeRotation - leftEye;
        rightDriftCorrection = rightEyeRotation - rightEye;

        finishedDriftCorrection();
    }

    //set the rotational drift correction of the left eye
    //argument is the expected eye rotation
    public void setLeftDriftCorrection(Vector2 leftEye)
    {
        leftDriftCorrection = leftEyeRotation - leftEye;
    }

    //set the rotational drift correction of the left eye
    //argument is the expected eye rotation
    public void setRightDriftCorrection(Vector2 rightEye)
    {
        rightDriftCorrection = rightEyeRotation - rightEye;
    }

    //reenables moving of the viewport
    public void finishedDriftCorrection()
    {
        moveViewPort = true;
    }

    //remove left DC (so we can get an accurate new DC)
    //also resets and disables moving of the viewport (for same reason)
    public void unsetLeftDriftCorrection()
    {
        leftDriftCorrection = Vector2.zero;

        moveViewPort = false;
        moveCamera(leftEyeDominant, Vector3.zero);
    }

    //remove right DC (so we can get an accurate new DC)
    //also resets and disables moving of the viewport (for same reason)
    public void unsetRightDriftCorrection()
    {
        rightDriftCorrection = Vector2.zero;

        moveViewPort = false;
        moveCamera(leftEyeDominant, Vector3.zero);
    }

    //remove current DC (so we can get an accurate new DC)
    //also resets and disables moving of the viewport (for same reason)
    public void unsetDriftCorrection()
    {
        leftDriftCorrection = Vector2.zero;
        rightDriftCorrection = Vector2.zero;

        moveViewPort = false;
        moveCamera(leftEyeDominant, Vector3.zero);
    }
}
