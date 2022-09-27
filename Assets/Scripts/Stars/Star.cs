using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : CelestialBody
{
	[Header("Star Attributes")]
    public Color chromaticity;
	public float solarRadius;
	public float luminosity;
	public float temperature;

	Material mat;
	StellarClassification.StellarClass stellarClass;

	private void Reset()
	{
		base.radius.Unit = Length.Unit.km;
		base.radius.isStatic = true;
	}

	private void OnValidate()
	{
		base.radius.Unit = Length.Unit.km;
		base.radius.isStatic = true;
		base.radius.amount = solarRadius * Universe.solarRadius.amount;
		mass = Mathf.Pow(Universe.lengthScale, 3) * (surfaceGravity * Mathf.Pow(Length.Convert(radius, Length.Unit.m).amount, 2) / Universe.gravitationalConstant);
		meshHolder = transform.GetChild(0);
		meshHolder.localScale = Vector3.one * Length.ConvertToWorld(radius) * 2;
	}

	private void Awake()
	{
		velocity = initialVelocity;
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
		rb.useGravity = false;
	}

	private void Start()
	{
		mat = meshHolder.GetComponent<MeshRenderer>().material;
		mat.EnableKeyword("_EMISSION");
		stellarClass = StellarClassification.Classify(temperature, luminosity * Universe.solarLuminosity);
		chromaticity = stellarClass.SpectralClass.chromaticity;
	}

	private void Update()
	{
		mat.SetColor("_EmissionColor", chromaticity * Mathf.LinearToGammaSpace(luminosity * .1f));
	}
}