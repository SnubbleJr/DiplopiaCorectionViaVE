using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class RightTextureShifter : MonoBehaviour {
    //shifts the image on the right eye, as well as loging the offset needed at cerrtain positions
    
    public Transform rightRenderTexture;

    public Transform leftCross, rightCross;

    public Vector2 pos;

    public float incremant = 0.01f;

    private string dataPath = "output.csv";
    private List <string> output = new List<string>();

    private bool wantToReset = false;

    private Vector3 crossPos;
    
    // Use this for initialization
    void Start()
    {
        if (rightRenderTexture == null)
        {
            Debug.LogError("Material not connected");
            this.enabled = false;
        }
        
        //either make file, or continue from where we left off
        dataPath = Path.Combine(Application.dataPath, dataPath);

        if (File.Exists(dataPath))
            foreach (string line in File.ReadAllLines(dataPath))
                output.Add(line);
        else
        {
            output.Add("Depth,Target X,Target Y,Ofsset X, Offset Y");
            File.WriteAllLines(dataPath, output.ToArray());
        }

        crossPos = rightCross.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        handleInput();
        applyChanges();
    }

    private void handleInput()
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
        
        //increaseing incremeant
        if (Input.GetKey(KeyCode.LeftControl))
        {
            wantToReset = false;

            if (Input.GetKey(KeyCode.RightArrow))
                incremant += (incremant * 0.1f);
            if (Input.GetKey(KeyCode.LeftAlt))
                incremant -= (incremant * 0.1f);
            if (Input.GetKey(KeyCode.DownArrow))
                incremant *= 0.5f;
            if (Input.GetKey(KeyCode.UpArrow))
                incremant *= 2f;
        }

        //generating a new depth
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            generateNewZ();

            moveCrosses();
        }
        //reseting
        if (Input.GetKeyDown(KeyCode.R))
            if (wantToReset)
                reset();
            else
                wantToReset = true;

        //saving changes and moving on
        if (Input.GetKeyDown(KeyCode.Space))
        {
            logOutput();

            //move crosses
            generateNewX();
            generateNewY();

            moveCrosses();
        }

        //if cross outside of range, then reroll position
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //move crosses without logging
            generateNewX();
            generateNewY();

            moveCrosses();
        }
    }

    private void applyChanges()
    {
        //applying
        Vector3 newPos = rightRenderTexture.localPosition;
        newPos.x = pos.x;
        newPos.y = pos.y;

        rightRenderTexture.localPosition = newPos;
    }

    private void logOutput()
    {
        output.Add(crossPos.z.ToString() + "," + crossPos.x.ToString() + "," + crossPos.y.ToString() + "," + pos.x.ToString() + "," + pos.y.ToString());
        File.WriteAllLines(dataPath, output.ToArray());
    }

    private void moveCrosses()
    {
        leftCross.localPosition = crossPos;
        rightCross.localPosition = crossPos;
    }
    
    private void generateNewX()
    {
        //move the crosses to a random point on screen
        float newScale;

        //weight towards the centre of the screen
        newScale = Random.Range(-3f, 3f);

        if (Mathf.Abs(newScale) > crossPos.z)
            newScale = crossPos.z / newScale;

        crossPos.x = newScale;
    }

    private void generateNewY()
    {
        //move the crosses to a random point on screen
        float newScale;

        //weight towards the centre of the screen
        newScale = Random.Range(-3f, 3f);

        if (Mathf.Abs(newScale) > crossPos.z)
            newScale = crossPos.z / newScale;

        crossPos.y = newScale;
    }

    private void generateNewZ()
    {
        //move the crosses to a random depth on screen
        crossPos.z = Random.Range(0.1f, 8f);
    }

    private void reset()
    {
        pos = Vector2.zero;

        wantToReset = false;
    }
}
