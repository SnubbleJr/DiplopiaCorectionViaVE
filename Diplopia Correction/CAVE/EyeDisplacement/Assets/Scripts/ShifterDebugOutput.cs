using UnityEngine;
using MiddleVR_Unity3D;
using System.Collections;

public class ShifterDebugOutput : MonoBehaviour {

    private vrNode3D leftScreenParent, rightScreenParent;

    private Transform leftScreenOutput, rightScreenOutput;


    // Use this for initialization
    void Start()
    {
        leftScreenOutput= GameObject.Find("LeftCameraScreens").transform;
        rightScreenOutput = GameObject.Find("RightCameraScreens").transform;
        
        var displayMgr = MiddleVR.VRDisplayMgr;

        //get the screen nodes
        leftScreenParent = displayMgr.GetNode("LeftCameraScreens");
        rightScreenParent = displayMgr.GetNode("RightCameraScreens");
    }

    // Update is called once per frame
    void Update()
    {
        leftScreenOutput.transform.localRotation = MVRTools.ToUnity(leftScreenParent.GetOrientationLocal());
        rightScreenOutput.transform.localRotation = MVRTools.ToUnity(rightScreenParent.GetOrientationLocal());
    }
}
