using UnityEngine;
using System.Collections;

public class RightCameraMover : MonoBehaviour {

    public Transform rightRenderCamera;

    private Vector3 origPos;
    private Quaternion origRot;

    public Vector3 pos, rot;

    public float incremant = 0.01f;

    private bool wantToReset = false;

    // Use this for initialization
    void Start()
    {
        if (rightRenderCamera == null)
        {
            Debug.LogError("Camera not connected");
            this.enabled = false;
        }
        else
        {
            origPos = rightRenderCamera.localPosition;
            origRot = rightRenderCamera.localRotation;

            pos = origPos;
            rot = origRot.eulerAngles;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //moving pos
        if (Input.GetKey(KeyCode.W))
        {
            wantToReset = false;

            if (Input.GetKey(KeyCode.UpArrow))
                pos.y -= incremant;
            if (Input.GetKey(KeyCode.DownArrow))
                pos.y += incremant;
            if (Input.GetKey(KeyCode.LeftArrow))
                pos.x += incremant;
            if (Input.GetKey(KeyCode.RightArrow))
                pos.x -= incremant;
        }

        //moving rot
        if (Input.GetKey(KeyCode.E))
        {
            wantToReset = false;

            if (Input.GetKey(KeyCode.UpArrow))
                rot.x += (incremant * 1000);
            if (Input.GetKey(KeyCode.DownArrow))
                rot.x -= (incremant * 1000);
            if (Input.GetKey(KeyCode.LeftArrow))
                rot.y += (incremant * 1000);
            if (Input.GetKey(KeyCode.RightArrow))
                rot.y -= (incremant*1000);
        }

        //increaseing incremeant
        if (Input.GetKey(KeyCode.LeftControl))
        {
            wantToReset = false;

            if (Input.GetKey(KeyCode.UpArrow))
                incremant += (incremant * 0.1f);
            if (Input.GetKey(KeyCode.DownArrow))
                incremant -= (incremant * 0.1f);
            if (Input.GetKey(KeyCode.LeftArrow))
                incremant *= 0.5f;
            if (Input.GetKey(KeyCode.RightArrow))
                incremant *= 2f;
        }

        //reseting
        if (Input.GetKeyDown(KeyCode.R))
            if (wantToReset)
                reset();
            else
                wantToReset = true;

        //applying
        rightRenderCamera.localPosition = pos;
        rightRenderCamera.localRotation = Quaternion.Euler(rot);
    }

    private void reset()
    {
         pos = origPos;
         rot = origRot.eulerAngles;

        wantToReset = false;
    }
}
