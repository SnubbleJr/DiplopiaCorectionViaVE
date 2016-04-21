using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct EyeGazeData
{
    public float depth;
    public Vector3 leftEyeGaze;
    public Vector3 rightEyeGaze;
    public Vector3 leftEyeRotationalDifference;
    public Vector3 rightEyeRotationalDifference;
}

public class EyeGazeDataLogger : MonoBehaviour {
    //logs all the data for the run

    private List<EyeGazeData> eyeGazeDataList = new List<EyeGazeData>();

    void Awake()
    {
        //create log folder if it doesn't exist
        if (!System.IO.Directory.Exists(Application.dataPath + "\\Logs\\"))
            System.IO.Directory.CreateDirectory(Application.dataPath + "\\Logs\\");
    }

    void OnApplicationQuit()
    {
        saveData();
    }

    private void saveData()
    {
        // Write the string to a file.
        System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "\\Logs\\" + System.DateTime.Now.ToFileTimeUtc() + "_gaze_output.csv");

        file.WriteLine("Depth Of Objectoin,Left Eye Rotation, , , Right Eye Rotation, , , Left Eye Rotational Correction, , , Right Eye Rotational Correctoin, , ");

        foreach (EyeGazeData eyeGazeData in eyeGazeDataList)
            file.WriteLine(eyeGazeData.depth.ToString() + ", " + eyeGazeData.leftEyeGaze.ToString() + ", " + eyeGazeData.rightEyeGaze.ToString() + ", " + eyeGazeData.leftEyeRotationalDifference.ToString() + ", " + eyeGazeData.rightEyeRotationalDifference.ToString());

        file.Close();
    }

    public void logData(bool leftEyeDominant, float depth, Vector3 leftEyeGaze, Vector3 rightEyeGaze, Vector3 subserviantAlteredGaze)
    {
        //careated a gazeData entry and add it ot the list
        EyeGazeData eyeGazeData;
        Vector3 leftEyeRotDif, rightEyeRotDif;

        if (leftEyeDominant)
        {
            leftEyeRotDif = Vector3.zero;
            rightEyeRotDif = subserviantAlteredGaze - rightEyeGaze;
        }
        else
        {
            rightEyeRotDif = Vector3.zero;
            leftEyeRotDif = subserviantAlteredGaze - leftEyeGaze;
        }

        eyeGazeData.depth = depth;
        eyeGazeData.leftEyeGaze = leftEyeGaze;
        eyeGazeData.rightEyeGaze = rightEyeGaze;
        eyeGazeData.leftEyeRotationalDifference = leftEyeRotDif;
        eyeGazeData.rightEyeRotationalDifference = rightEyeRotDif;

        eyeGazeDataList.Add(eyeGazeData);
    }
}
