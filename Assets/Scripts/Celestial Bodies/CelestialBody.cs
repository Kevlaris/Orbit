using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CelestialBody : GravityBody
{
	public string Name {  get; protected set; }
	public string Description { get; protected set; }
	public Sprite Icon { get; protected set; }
	public int ID {  get; private set; }

	protected override void Start()
	{
		base.Start();
		FetchID();
	}

	private void FetchID()
	{
		ID = Universe.RequestID(this);
	}
}