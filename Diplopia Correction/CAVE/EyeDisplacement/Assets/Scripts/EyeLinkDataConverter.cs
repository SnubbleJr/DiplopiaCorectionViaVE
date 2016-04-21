using UnityEngine;
using System.Collections;

[System.Serializable]
public class GazeData
{
    public float[] hx;
    public float[] hy;

    public GazeData(float hx1, float hx2, float hy1, float hy2)
    {
        hx = new float[2] { hx1, hx2 };
        hy = new float[2] { hy1, hy2 };
    }
}

public class EyeLinkDataConverter : MonoBehaviour
{
    //debug
    //public GazeData data;

    //Converts the data given by eye link into something useable
    private EyeGazeRayCasterManager casterManager;

    void Awake()
    {
        casterManager = GetComponent<EyeGazeRayCasterManager>();
    }

    /*
    //testdata
    void Update()
    {
        setData(data);
    }
    */

    public void setData(string[] subData)
    {
        GazeData gazeData = new GazeData(float.Parse(subData[0]), float.Parse(subData[1]), float.Parse(subData[2]), float.Parse(subData[3]));
        setData(gazeData);
    }

    public void setData(GazeData gazeData)
    {
        //calculate the degrees of rotation for each eye based on the position and angulaar resolution
        //pass the gaze data to the calculator

        //left eye is 0th index
        //right eye is 1st index

        //arbetry distance of user from the virtual plane that these coords are on
        float f = 15000f;

        //reference point we are comparing the rotations to (use 0,0 for the sake of testing)
        float x0 = 0, y0 = 0;

        float x1 = gazeData.hx[0];
        float y1 = gazeData.hy[0];
        float x2 = gazeData.hx[1];
        float y2 = gazeData.hy[1];
        /*
        //angle for both is Mathf.Rad2Deg * Mathf.Acos((f * f + x0 * x1 + y0 * y1) / Mathf.Sqrt((f * f + x0 + x0 + y0 * y0) * (f * f + x1 * x1 + y1 * y1)));
        //to get the x component, set y1 = y0
        //to get the y component, set x1 = x0;
        float leftX = Mathf.Rad2Deg * Mathf.Acos((f * f + x0 * x1 + y0 * y0) / Mathf.Sqrt((f * f + x0 * x0 + y0 * y0) * (f * f + x1 * x1 + y0 * y0)));
        float leftY = Mathf.Rad2Deg * Mathf.Acos((f * f + x0 * x0 + y0 * y1) / Mathf.Sqrt((f * f + x0 + x0 + y0 * y0) * (f * f + x0 * x0 + y1 * y1)));

        //do the same for the right eye
        float rightX = Mathf.Rad2Deg * Mathf.Acos((f * f + x0 * x2 + y0 * y0) / Mathf.Sqrt((f * f + x0 * x0 + y0 * y0) * (f * f + x2* x2 + y0 * y0)));
        float rightY = Mathf.Rad2Deg * Mathf.Acos((f * f + x0 * x0 + y0 * y2) / Mathf.Sqrt((f * f + x0 + x0 + y0 * y0) * (f * f + x0 * x0 + y2 * y2)));

        if (calculatorManager)
        {
            //need to flip the numbers round as we are sending angles around the these axis
            calculatorManager.leftGaze = new Vector2(leftY, leftX);
            calculatorManager.rightGaze = new Vector2(rightY, rightX);
            */

        float leftX = Mathf.Rad2Deg * Mathf.Atan(x1 / f);
        float leftY = Mathf.Rad2Deg * Mathf.Atan(y1 / f);

        //do the same for the right eye
        float rightX = Mathf.Rad2Deg * Mathf.Atan(x2 / f);
        float rightY = Mathf.Rad2Deg * Mathf.Atan(y2 / f);

        if (casterManager)
        {
            //need to flip the numbers round as we are sending angles around the these axis (rotating around the y axis points on the x)
            casterManager.leftEyeRotation = new Vector2(leftY, leftX);
            casterManager.rightEyeRotation = new Vector2(rightY, rightX);
        }
    }
}
