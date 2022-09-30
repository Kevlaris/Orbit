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
	GameObject particlePrefab;

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

		particlePrefab = Universe.solarParticleStandard;
	}

	private void Start()
	{
		mat = meshHolder.GetComponent<MeshRenderer>().material;
		mat.EnableKeyword("_EMISSION");
		stellarClass = StellarClassification.Classify(temperature, luminosity * Universe.solarLuminosity);
		chromaticity = stellarClass.SpectralClass.chromaticity;
		CalibrateParticleSystems(new Quantity(solarRadius * Universe.solarRadius, Length.Unit.km));
	}

	private void Update()
	{
		Color.RGBToHSV(chromaticity, out float h, out float s, out float v);
		mat.SetColor("_EmissionColor", Color.HSVToRGB(h, s, v/2, true) * Mathf.LinearToGammaSpace(Mathf.Clamp(luminosity, 0, 15)));
	}

	void CalibrateParticleSystems(Quantity radius)
	{
		float multiplier = Length.Convert(radius, Length.Unit.km).amount / Length.Convert(Universe.solarRadius, Length.Unit.km).amount;

		// prefab particle systems
		ParticleSystem surfacePrefab = particlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem coronaPrefab = particlePrefab.transform.GetChild(0).GetComponent<ParticleSystem>();
		ParticleSystem flaresPrefab = particlePrefab.transform.GetChild(1).GetComponent<ParticleSystem>();
		// actual particle systems
		ParticleSystem surface = transform.GetChild(1).GetComponent<ParticleSystem>();
		ParticleSystem corona = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
		ParticleSystem flares = transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();

		ParticleSystem.MinMaxCurve startSpeed, startSize;

		ScaleParticleSystem(surfacePrefab, multiplier, out startSpeed, out startSize);
		var surfaceMain = surface.main;
		surfaceMain.startSpeed = startSpeed;
		surfaceMain.startSize = startSize;

		ScaleParticleSystem(coronaPrefab, multiplier, out startSpeed, out startSize);
		var coronaMain = corona.main;
		coronaMain.startSpeed = startSpeed;
		coronaMain.startSize = startSize;

		ScaleParticleSystem(flaresPrefab, multiplier, out startSpeed, out startSize);
		var flaresMain = flares.main;
		flaresMain.startSpeed = startSpeed;
		flaresMain.startSize = startSize;
	}

	/// <summary>
	/// Scale Particle System in accordance to the given multiplier.
	/// </summary>
	void ScaleParticleSystem(ParticleSystem particleSystem, float multiplier, out ParticleSystem.MinMaxCurve startSpeed, out ParticleSystem.MinMaxCurve startSize)
	{
		var main = particleSystem.main;

		startSpeed = main.startSpeed;
		startSize = main.startSize;

		if (multiplier == 1 || multiplier <= 0) return;

		switch (startSpeed.mode)
		{
			case ParticleSystemCurveMode.Constant:
				startSpeed.constant =  multiplier;
				break;
			case ParticleSystemCurveMode.TwoConstants:
				startSpeed.constantMin *= multiplier;
				startSpeed.constantMax *= multiplier;
				break;
			default:
				break;
		}
		
		switch (startSize.mode)
		{
			case ParticleSystemCurveMode.Constant:
				startSize.constant *= multiplier;
				break;
			case ParticleSystemCurveMode.TwoConstants:
				startSize.constantMin *= multiplier;
				startSize.constantMax *= multiplier;
				break;
			default:
				break;
		}
	}
}