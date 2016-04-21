using UnityEngine;
using System.Collections;

public class TestInputScript : MonoBehaviour {
    //script that mimics eye input and middlevr input

    public float movementSpeed = 0.1f;
    public float increaseSpeed = 10f;

    private Transform headNode;

    private float min = -30000, max = 30000;
    private EyeLinkDataConverter dataConverter;
    private DriftCorrecter driftCorrecter;

    public float leftX, leftY, rightX, rightY;

    private bool triggerPressed = false;

    // Use this for initialization
    void Start ()
    {
        dataConverter = GetComponent<EyeLinkDataConverter>();
        driftCorrecter = GetComponent<DriftCorrecter>();
        headNode = GameObject.Find("HeadNode").transform;
	}

    // Update is called once per frame
    void Update()
    {
        handleInput();
        sendEyeData();
    }

    private void handleInput()
    {
        //if left trigger is held, then drift correct
        if (Input.GetAxis("Left Trigger") > 0)
        {
            if (!triggerPressed)
            {
                triggerPressed = true;
                driftCorrecter.startDC();
            }

            //pressing A toggles between DC eyes (if we are monocular)
            if (Input.GetButtonDown("Fire1"))
                driftCorrecter.switchMonocularEye();

            //pressing B clears DC
            if (Input.GetButtonDown("Fire2"))
                driftCorrecter.clearDC();
        }
        else if (triggerPressed)
        {
            triggerPressed = false;
            driftCorrecter.stopDC();
        }

        float newValue;

        //if right trigger is held down then controll eyes
        if (Input.GetAxis("Right Trigger") > 0)
        {
            newValue = Input.GetAxis("Horizontal") * increaseSpeed + leftX;
            if (newValue > min && newValue < max)
                leftX = newValue;
            newValue = -Input.GetAxis("Vertical") * increaseSpeed + leftY;
            if (newValue > min && newValue < max)
                leftY = newValue;

            newValue = Input.GetAxis("Right Stick Horizontal") * increaseSpeed + rightX;
            if (newValue > min && newValue < max)
                rightX = newValue;
            newValue = -Input.GetAxis("Right Stick Vertical") * increaseSpeed + rightY;
            if (newValue > min && newValue < max)
                rightY = newValue;
        }
        //else controll head movement and rotation
        else
        {
            Vector3 newPos = transform.forward * Input.GetAxis("Vertical") * movementSpeed + transform.right * Input.GetAxis("Horizontal") * movementSpeed;
            newPos.y = 0;

            headNode.transform.localPosition += newPos;

            //seperate out the horizontal and vertical rotatoin (so we get nice, fps still looking)
            transform.RotateAround(transform.position, transform.right, -Input.GetAxis("Right Stick Vertical"));
            headNode.transform.Rotate(headNode.transform.up, Input.GetAxis("Right Stick Horizontal"));
        }
    }

    private void sendEyeData()
    {
        string[] args = { leftX.ToString(), rightX.ToString(), leftY.ToString(), rightY.ToString() };
        dataConverter.setData(args);
    }
}
