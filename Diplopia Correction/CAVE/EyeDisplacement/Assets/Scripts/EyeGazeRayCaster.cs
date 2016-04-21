using UnityEngine;
using System.Collections;
using System;

public class EyeGazeRayCaster : MonoBehaviour
{
    //Shoots rays when told from the given point, and returns the raycast of interesectoin for the target object
    private LineRenderer lineRenderer;
    
    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetWidth(0.05f, 0.05f);
    }

    //raycast hit and return point
    public bool rayCast(Vector3 direction, out RaycastHit raycastHit)
    {        
        Ray ray = new Ray(transform.position, direction);

        return Physics.Raycast(ray, out raycastHit);
    }

    public void debugDraw(Vector3 lineEndpoint, float length, Color color)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineEndpoint);
        lineRenderer.SetColors(color, Color.clear);

        Debug.DrawRay(transform.position, transform.forward * length, color);
    }

    public void setLineMaterial(Material material)
    {
        lineRenderer.material = material;
    }
}
