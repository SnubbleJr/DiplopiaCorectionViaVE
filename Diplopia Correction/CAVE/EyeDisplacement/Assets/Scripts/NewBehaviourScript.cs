using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Animation>().Stop();
	}
	
	// Update is called once per frame
	void Update () {
        vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

        if (!GetComponent<Animation>().isPlaying && keyb.IsKeyPressed(MiddleVR.VRK_S)) { GetComponent<Animation>().Play(); }

		if (keyb.IsKeyPressed(MiddleVR.VRK_R)) {
			//Application.LoadLevel(0);
            GetComponent<Animation>().Stop();
			transform.localScale = new Vector3(transform.localScale.x,transform.localScale.y * 1.005f,transform.localScale.z);
            GetComponent<Animation>().Play();
		}

        if (keyb.IsKeyPressed(MiddleVR.VRK_F))
        {
            //Application.LoadLevel(0);
            GetComponent<Animation>().Stop();
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.995f, transform.localScale.z);
            GetComponent<Animation>().Play();
        }

		if (keyb.IsKeyPressed(MiddleVR.VRK_T)) {
			//Application.LoadLevel(0);
            GetComponent<Animation>().Stop();
            transform.localScale = new Vector3(transform.localScale.x * 1.005f, transform.localScale.y, transform.localScale.z * 1.005f);
            GetComponent<Animation>().Play();
		}

        if (keyb.IsKeyPressed(MiddleVR.VRK_G))
        {
            //Application.LoadLevel(0);
            GetComponent<Animation>().Stop();
            transform.localScale = new Vector3(transform.localScale.x * 0.995f, transform.localScale.y, transform.localScale.z * 0.995f);
            GetComponent<Animation>().Play();
        }
		
		if (Input.GetKey(KeyCode.Return)) {
			Application.LoadLevel(0);
		}
	}
}
