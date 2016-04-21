using UnityEngine;
using System.Collections;

public class CaveViewPortManager : MonoBehaviour
{
    //sets up the viewports/screens of middleVR so the viewports from both eyes can be split

    public bool debug { get; set; }     //if set, we use testOutput
    private TestOutputScript testOutput;
    private FoveaScreenShifter screenShifter;

    void Start()
    {
        screenShifter = GameObject.Find("Shifter").GetComponent<FoveaScreenShifter>();
        testOutput = GetComponent<TestOutputScript>();
    }

    public void setScreenRotation(bool moveRightViewPort, Vector3 rotation)
    {
        if (debug)
            testOutput.testOutputRotation(moveRightViewPort, rotation);
        else
            shifterRotation(moveRightViewPort, rotation);
    }

    private void shifterRotation(bool moveRightViewPort, Vector3 rotation)
    {
        if (moveRightViewPort)
            //move the right veiwport
            screenShifter.applyRotation(Vector3.zero, rotation);
        else
            //move the left
            screenShifter.applyRotation(rotation, Vector3.zero);
    }

    public void printOut(string leftEye, string rightEye)
    {
        screenShifter.printOut(leftEye, rightEye);
    }
}
