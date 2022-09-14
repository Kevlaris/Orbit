using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCamera : MonoBehaviour
{
    public bool focus;
    public CelestialBody focusObject;
    void Update()
    {
		if (focus)
		{
            transform.position = new Vector3(focusObject.Position.x, transform.position.y, focusObject.Position.z);
		}
    }
}
