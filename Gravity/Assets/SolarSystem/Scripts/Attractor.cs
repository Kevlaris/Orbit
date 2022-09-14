using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class Attractor : GravityObject
{
	// private const float G = Universe.gravitationalConstant;

	[Header("Planet Attributes")]
	public float radius = 1;
	public float mass = 1;
	public float surfaceGravity = 1;
	public Vector3 initialVelocity;

	[HideInInspector]
	public Vector3 acceleration;
	[HideInInspector]
	public Vector3 currentVelocity;

	Transform meshHolder;
	Rigidbody rb;

	private void Awake()
	{
		currentVelocity = initialVelocity;

		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
	}

	public void UpdateVelocity(Attractor[] attractors, float timeStep)
	{
		foreach (Attractor attractor in attractors)
		{
			if (attractor != this)
			{
				Vector3 forceDirection = (attractor.rb.position - rb.position).normalized;
				float sqrDistance = (attractor.rb.position - rb.position).magnitude;

				Vector3 force = forceDirection * Universe.gravitationalConstant * mass * attractor.mass / sqrDistance;
				acceleration = force / mass;
				currentVelocity += acceleration * timeStep;
			}
		}
	}

	public void UpdatePosition(float timeStep)
	{
		rb.position += currentVelocity * timeStep;
	}

	void OnValidate()
	{
		mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
		transform.localScale = Vector3.one * (radius * 2);
	}

	public Rigidbody Rigidbody
	{
		get
		{
			return rb;
		}
	}
	public Vector3 Position
	{
		get
		{
			return rb.position;
		}
	}
}
