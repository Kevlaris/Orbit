using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
	public const float gravitationalConstant = 0.0001f;
	public const float physicsTimeStep = 0.01f;

	Attractor[] attractors;

	private void Awake()
	{
		attractors = FindObjectsOfType<Attractor>();
		Time.fixedDeltaTime = physicsTimeStep;
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < attractors.Length; i++)
		{
			attractors[i].UpdateVelocity(attractors, physicsTimeStep);
		}
		for (int i = 0; i < attractors.Length; i++)
		{
			attractors[i].UpdatePosition(physicsTimeStep);
		}
	}
}
