using UnityEngine;
using System.Collections;

public class navigate_camera : MonoBehaviour {
	
	public float rotationSpeed = 20.0F;
	public float speed = 1.7F;
	
	
	
	public static bool CAVE = true; 
	
	void Update () 
	{
		
		float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		
		if (CAVE)
		{
			vrTracker wand = null;
			vrAxis    axis = null;
			if (MiddleVR.VRDeviceMgr != null)
			{
				wand = MiddleVR.VRDeviceMgr.GetTracker("VRPNTracker0.Tracker1");
				axis    = MiddleVR.VRDeviceMgr.GetAxis("VRPNAxis0.Axis");
				if( axis != null) // && axis.GetValue(0) != 0 )
        		{
           		 rotation = axis.GetValue(0) * rotationSpeed;
        		}
				if( axis != null)// && axis.GetValue(1) != 0 )
        		{
           		 translation = axis.GetValue(1)* speed;
        		}
			}
		}			
		translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
		print("Time.deltaTime = " + Time.deltaTime);
		print("translation = " + translation);
		print("rotation = " + rotation);
	}
}
