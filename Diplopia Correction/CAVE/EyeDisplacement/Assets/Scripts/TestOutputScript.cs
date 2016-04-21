using UnityEngine;
using System.Collections;

public class TestOutputScript : MonoBehaviour {
    //Hijacks calls to fovea screen shifter to see the effects when not in the cave
    //configeures viewportmanger to send rotations here

    private CaveViewPortManager viewportManager;

    private Transform leftScreenParent, rightScreenParent;

    // Use this for initialization
    void Start()
    {
        viewportManager = GetComponent<CaveViewPortManager>();
        viewportManager.debug = true;

        leftScreenParent = GameObject.Find("LeftCameraScreens").transform;
        rightScreenParent = GameObject.Find("RightCameraScreens").transform;
    }

    public void testOutputRotation(bool moveRightViewPort, Vector3 rotation)
    {
        if (moveRightViewPort)
            //move the right veiwport
            applyRotation(Vector3.zero, rotation);
        else
            //move the left
            applyRotation(rotation, Vector3.zero);
    }

    private void applyRotation(Vector3 leftFoveaRot, Vector3 rightFoveaRot)
    {        
        rotateScreen(leftScreenParent, leftFoveaRot);
        rotateScreen(rightScreenParent, rightFoveaRot);
    }

    private void rotateScreen(Transform screenParent, Vector3 rotation)
    {
        screenParent.localEulerAngles = rotation;
    }
}
