using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : CelestialBody
{
	[SerializeField] Sprite icon;

	protected override void Reset()
	{
		base.Reset();
		icon = Icon;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		Name = gameObject.name;
		Icon = icon;
	}

	protected override void Awake()
	{
		base.Awake();
		Description = "bolygó";
	}
}