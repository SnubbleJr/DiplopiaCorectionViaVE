using UnityEngine;
using MiddleVR_Unity3D;
using System.Collections;

public class FoveaScreenShifter : MonoBehaviour {
    
    private vrNode3D leftScreenParent, rightScreenParent;

    TextMesh leftText, rightText;
    
    void Awake()
    {
        GameObject ltm = new GameObject("Left Text Mesh");
        GameObject rtm = new GameObject("Right Text Mesh");
        ltm.transform.SetParent(transform, false);
        rtm.transform.SetParent(transform, false);
        rtm.transform.localPosition = Vector3.up*2;

        leftText = ltm.AddComponent<TextMesh>();
        rightText = rtm.AddComponent<TextMesh>();

        leftText.color = Color.green;
        rightText.color = Color.red;

        makeNewScreens();
    }

    private void makeNewScreens()
    {
        var displayMgr = MiddleVR.VRDisplayMgr;

        //make a new set of screens
        //as well as rename screens to LeftCameraScreens
        leftScreenParent = displayMgr.GetNode("Screens");
        leftScreenParent.SetName("LeftCameraScreens");

        rightScreenParent = displayMgr.CreateNode("RightCameraScreens");
        rightScreenParent.SetParent(leftScreenParent.GetParent());
        rightScreenParent.SetPositionLocal(leftScreenParent.GetPositionLocal());

        // For each vrCameraStereo, make a new screen for the right eye
        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());

                vrScreen leftScreen = stereoCam.GetCameraLeft().GetScreen();
                vrScreen rightScreen = displayMgr.CreateScreen(leftScreen.GetName());

                rightScreen.SetParent(rightScreenParent);
                rightScreen.SetHeight(leftScreen.GetHeight());
                rightScreen.SetWidth(leftScreen.GetWidth());
                rightScreen.SetFiltered(leftScreen.IsFiltered());
                rightScreen.SetTracker(leftScreen.GetTracker());
                rightScreen.SetPositionWorld(leftScreen.GetPositionWorld());
                rightScreen.SetOrientationWorld(leftScreen.GetOrientationWorld());

                stereoCam.GetCameraRight().SetScreen(rightScreen);
            }
        }
    }

    public void applyRotation(Vector3 leftFoveaRot, Vector3 rightFoveaRot)
    {
        // Display the values
        leftText.text = "<size=10>Rot: " + leftFoveaRot + "</size>";
        rightText.text = "<size=10>Rot: " + rightFoveaRot + "</size>";

        /*
        Old way of doing it
        if (MiddleVR.VRDeviceMgr.IsWandButtonPressed(0))
        {
            rightScreenParent.SetYawWorld(MiddleVR.VRDisplayMgr.GetNode("HandNode").GetYawWorld());
            rightScreenParent.SetPitchWorld(MiddleVR.VRDisplayMgr.GetNode("HandNode").GetPitchWorld());
        }

        //vrQuat rot = MVRTools.FromUnity(fovea.rotation);

        vrNode3D rot = MiddleVR.VRDisplayMgr.GetNode("HandNode");
        */

        rotateScreen(leftScreenParent, leftFoveaRot);
        rotateScreen(rightScreenParent, rightFoveaRot);
    }

    private void rotateScreen(vrNode3D screenParent, Vector3 angles)
    {
        //remove z componant (roll)
        angles.z = 0;
        screenParent.SetOrientationLocal(MVRTools.FromUnity(Quaternion.Euler(angles)));
        //screenParent.SetRollLocal(angles.x);
        //screenParent.SetYawLocal(angles.y);
        //screenParent.SetPitchLocal(angles.z);
    }

    public void printOut(string leftEye, string rightEye)
    {
        if (leftText)
        {
            leftText.text = "<size=10>Rot: " + leftEye + "</size>";
            rightText.text = "<size=10>Rot: " + rightEye + "</size>";
        }
    }
}
