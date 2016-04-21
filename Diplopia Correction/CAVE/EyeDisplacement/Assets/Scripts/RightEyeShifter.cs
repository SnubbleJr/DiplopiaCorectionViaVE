using UnityEngine;
using System.Collections;

public class RightEyeShifter : MonoBehaviour
{

    private float increment = 0.001f;
    private float incrementFidelity = 0.0001f;

    private float originalXDist;
    private float originalincrement;

    private float xDist;
    private float yDist;

    private float roll;
    private float yaw;

    private bool wantToReset = false;

    int frameCounter = 20;
    vrQuat quat;

    private bool begin = false;

    void Start()
    {
        originalincrement = increment;
        var displayMgr = MiddleVR.VRDisplayMgr;

        makeNewScreens();
        Debug.Log("got here !");

        begin = true;

        /*
        // For each vrCameraStereo, apply the new transform matrix
        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());
                originalXDist = stereoCam.GetCameraRight().GetPositionLocal().x();
            }
        }
        */
    }

    void OnApplicationQuit()
    {
        reset();
    }

    private void reset()
    {
        wantToReset = false;
        xDist = originalXDist;
        yDist = 0;
        yaw = 0;
        roll = 0;
        increment = originalincrement;
    }

    void Update()
    {
        if (!begin)
            return;

        vrKeyboard keyboard = MiddleVR.VRDeviceMgr.GetKeyboard();
        
        // Apply new transform
        if (keyboard != null)
        {
            // Hold down W to activate x, y move
            if (keyboard.IsKeyPressed(MiddleVR.VRK_W))
            {
                if (keyboard.IsKeyPressed(MiddleVR.VRK_RIGHT))
                    xDist += increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_LEFT))
                    xDist -= increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_UP))
                    yDist += increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_DOWN))
                    yDist -= increment;
            }
            // Hold down E to activate y, z rotation
            if (keyboard.IsKeyPressed(MiddleVR.VRK_E))
            {
                if (keyboard.IsKeyPressed(MiddleVR.VRK_RIGHT))
                    roll += increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_LEFT))
                    roll -= increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_UP))
                    yaw += increment;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_DOWN))
                    yaw -= increment;
            }
            //	Press R to reset
            if (keyboard.IsKeyToggled(MiddleVR.VRK_R))
            {
                if (wantToReset)
                    reset();
                else
                    wantToReset = true;
            }
            // Press + to increase movement amount
            if (keyboard.IsKeyToggled(MiddleVR.VRK_EQUALS))
            {
                int magnitude = 1;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_LSHIFT))
                    magnitude = 10;
                increment += (incrementFidelity * magnitude);
            }// Press - to increase movement amount
            if (keyboard.IsKeyToggled(MiddleVR.VRK_MINUS))
            {
                int magnitude = 1;
                if (keyboard.IsKeyPressed(MiddleVR.VRK_LSHIFT))
                    magnitude = 10;
                increment -= (incrementFidelity * magnitude);
                if (increment < 0)
                    increment = 0;
            }
        }

        // Display the values
        string text = "<size=20>Pos:(" + xDist + ", " + yDist + ", 0)</size>\nRot:(0, " + roll + ", " + yaw + ")\n<size=10>Adjustment: " + increment + "</size>";

        GetComponent<TextMesh>().text = text;

        print(text);

        applyOffset();
    }

    private void makeNewScreens()
    {
        var displayMgr = MiddleVR.VRDisplayMgr;
             
        // For each vrCameraStereo, make a new screen for the right eye
        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());

                vrScreen leftScreen = stereoCam.GetCameraLeft().GetScreen();
                vrScreen rightScreen = displayMgr.CreateScreen(leftScreen.GetName() + "right");
                leftScreen.SetName(leftScreen.GetName() + "left");

                rightScreen.SetParent(leftScreen.GetParent());
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

    private void applyOffset()
    {
        var displayMgr = MiddleVR.VRDisplayMgr;

        // For each vrCameraStereo, apply the new transform matrix to right cameras screen
        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());

                vrVec3 pos = stereoCam.GetCameraRight().GetScreen().GetPositionLocal();
                pos.SetX(pos.x() + xDist);
                pos.SetY(pos.y() + yDist);
                stereoCam.GetCameraRight().GetScreen().SetPositionLocal(pos);

                /*
                stereoCam.GetCameraRight().SetPositionLocal(new vrVec3(xDist, yDist, 0));
                stereoCam.GetCameraRight().SetRollLocal(roll);
                stereoCam.GetCameraRight().SetYawLocal(yaw);
                if ((frameCounter / 4) > 20)
                {
                    quat = stereoCam.GetCameraRight().GetOrientationLocal();
                    frameCounter = 0;
                }
                else
                    frameCounter++;
                GetComponent<TextMesh>().text = quat.x() + ", " + quat.y() + ", " + quat.z() + ", " + quat.w();
                quat.SetY(roll);
                quat.SetZ(yaw);
                stereoCam.GetCameraRight().SetOrientationLocal(quat);
                */
            }
        }
    }
}
