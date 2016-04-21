using UnityEngine;
using System.Collections;

public class ModePicker : MonoBehaviour {
    //manages both forms of adjustment, only one type at a time moveCamera by default

    public bool shiftTexture = false;

    private RightCameraMover cameraMover;
    private RightTextureShifter textureShifter;
    
	// Use this for initialization
	void Awake ()
    {
        cameraMover = GetComponentInChildren<RightCameraMover>();
        textureShifter = GetComponentInChildren<RightTextureShifter>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        cameraMover.enabled = !shiftTexture;
        textureShifter.enabled = shiftTexture;
	}
}
