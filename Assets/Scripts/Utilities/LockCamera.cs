using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCamera : MonoBehaviour
{
    public bool focus;
    public GravityBody focusObject;
    void Update()
    {
		if (focus)
		{
            transform.position = new Vector3(focusObject.transform.position.x, transform.position.y, focusObject.transform.position.z);
		}
    }
}
