using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : CelestialBody
{
	[SerializeField] Sprite icon;

	private void Reset()
	{
		icon = Icon;
	}

	private void OnValidate()
	{
		Name = gameObject.name;
		Icon = icon;
	}

	private void Awake()
	{
		Description = "bolygó";
	}
}