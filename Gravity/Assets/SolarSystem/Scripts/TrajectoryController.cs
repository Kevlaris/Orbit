using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrajectoryController : MonoBehaviour
{
    [Header("Line Renderer Variables")]
    public LineRenderer line;
    [Range(2,30)]
    public int resolution;

    [Header("Formula Variables")]
    public Vector3 velocity;
    public float yLimit;
    public Vector3 acceleration;
    public float timeMultiplier = 1f;

	private void OnValidate()
	{
        velocity = transform.GetComponent<Attractor>().initialVelocity;
	}

	private void Update()
	{
        velocity = transform.GetComponent<Attractor>().currentVelocity;
        acceleration = transform.GetComponent<Attractor>().acceleration;

        StartCoroutine(RenderArc());
	}

	private IEnumerator RenderArc()
	{
        line.positionCount = resolution + 1;
        line.SetPositions(CalculateLineArray());
        yield return null;
	}

    private Vector3[] CalculateLineArray()
	{
        Vector3[] lineArray = new Vector3[resolution + 1];
        
        for (int i = 0; i < lineArray.Length; i++)
		{
            var t = (i / (float)lineArray.Length) * timeMultiplier;
            lineArray[i] = CalculateLinePoint(t);
		}
        return lineArray;
	}

    private Vector3 CalculateLinePoint(float t)
	{
        float x = (velocity.x * t) - (acceleration.x * t * t / 2);
        float y = (velocity.y * t) - (acceleration.y * t * t / 2);
        float z = (velocity.z * t) - (acceleration.z * t * t / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
    }
}
