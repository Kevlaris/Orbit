using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Star : CelestialBody
{
	public Sprite icon;

	//[Header("Star Attributes")]
	// [Range(0,360)] public float hue;
	public Color chromaticity;
	public float solarRadius;
	public float solarLuminosity;
	public float absoluteMagnitude;
	public float temperature;

	GameObject particle;
	public float particleScale;	// scale of particle systems in solar radii

	Material mat;
	public StellarClassification.StellarClass stellarClass;

	private void Reset()
	{
		base.radius.Unit = Length.Unit.km;
		base.radius.isStatic = true;
		particleScale = solarRadius;
		Destroy(particle);
		particle = null;
		if (!particle)
		{
			particle = GetComponentInChildren<ParticleSystem>().gameObject;
		}
		icon = Icon;
	}

	private void OnValidate()
	{
		radius.Unit = Length.Unit.km;
		radius.isStatic = true;
		radius.amount = solarRadius * Universe.solarRadius;
		mass = Mathf.Pow(Universe.lengthScale, 3) * (surfaceGravity * Mathf.Pow(Length.Convert(radius, Length.Unit.m).amount, 2) / Universe.gravitationalConstant);
		meshHolder = transform.GetChild(0);
		meshHolder.localScale = 2 * Length.ConvertToWorld(radius) * Vector3.one;
		//chromaticity = Color.HSVToRGB(hue.Remap(0,360,0,1), 1, 1);

		Name = gameObject.name;
		Icon = icon;
	}

	private void Awake()
	{
		velocity = initialVelocity;
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
		rb.useGravity = false;

		particleScale = solarRadius;
		if (!particle)
		{
			ParticleSystem query = GetComponentInChildren<ParticleSystem>();
			if (query != null) particle = query.gameObject;
		}
	}

	protected override void Start()
	{
		base.Start();
		Debug.Log("asd");
		mat = meshHolder.GetComponent<MeshRenderer>().sharedMaterial;
		mat.EnableKeyword("_EMISSION");
		stellarClass = GetClass();
		//chromaticity = stellarClass.SpectralClass.chromaticity;
		//hue = stellarClass.SpectralClass.hue;
		CalibrateParticleSystems();
	}

	private void Update()
	{
		/*
		Color.RGBToHSV(chromaticity, out float h, out float s, out _);
		mat.SetColor("_EmissionColor", Color.HSVToRGB((hue-5).Remap(0,360,0,1), .5f, .35f, true) * Mathf.LinearToGammaSpace(Mathf.Clamp(luminosity, 0, 15)));
		*/
		mat.SetColor("_EmissionColor", chromaticity);
	}

	private void OnApplicationQuit()
	{
		Destroy(particle);
		particle = null;
	}

	public StellarClassification.StellarClass GetClass()
	{
		stellarClass = StellarClassification.Classify(temperature, solarLuminosity * Universe.solarLuminosity);
		Description = StellarClassification.ClassString(stellarClass) + " típusú csillag";
		return stellarClass;
	}

	#region Particles

	public void CalibrateParticleSystems()
	{
		LoadParticleSystem();

		ParticleSystem[] particleSystems = GetParticleSystems(particle);
		for (int i = 0; i < particleSystems.Length; i++)
		{
			var shape = particleSystems[i].shape;
			shape.meshRenderer = meshHolder.GetComponent<MeshRenderer>();
		}

		ScaleParticleSystems(solarRadius);
		//ColorParticleSystems(hue);
	}

	public void ScaleParticleSystems(float solarRadius)
	{
		float multiplier = solarRadius / particleScale;

		ParticleSystem[] particleSystems = GetParticleSystems();

		for (int i = 0; i < particleSystems.Length; i++)
		{
			ScaleParticleSystem(particleSystems[i], multiplier, out ParticleSystem.MinMaxCurve startSpeed, out ParticleSystem.MinMaxCurve startSize);
			var main = particleSystems[i].main;
			main.startSpeed = startSpeed;
			main.startSize = startSize;
		}
		particleScale = solarRadius;
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
				startSpeed.constant = multiplier;
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

	[Obsolete]
	void GetParticleSystems(out ParticleSystem surface, out ParticleSystem corona, out ParticleSystem flares)
	{
		surface = transform.GetChild(1).GetComponent<ParticleSystem>();
		corona = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
		flares = transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();
	}
	ParticleSystem[] GetParticleSystems()
	{
		return GetComponentsInChildren<ParticleSystem>(false);
	}
	ParticleSystem[] GetParticleSystems(GameObject gameObject)
	{
		return gameObject.GetComponentsInChildren<ParticleSystem>(false);
	}

	public void LoadParticleSystem()
	{
		GetClass();
		if (particle) UnloadParticleSystem();
		GameObject particleObject = Resources.Load<GameObject>("Stars/Particles/" + stellarClass.SpectralClass.letter);
		if (!particleObject)
		{
			particleObject = Universe.solarParticleStandard;
		}
		particle = Instantiate(particleObject, transform);
		particle.name = "Particle System " + particleObject.name;
	}
	public GameObject GetParticleObject()
	{
		if (!particle)
		{
			ParticleSystem query = GetComponentInChildren<ParticleSystem>();
			if (query != null) particle = query.gameObject;
		}
		return particle;
	}
	public void UnloadParticleSystem()
	{
		if (!particle) return;
#if UNITY_EDITOR
		DestroyImmediate(particle);
#else
		Destroy(particle);
#endif
		particle = null;
	}

	/*
	/// <param name="hue">Hue component of color [0-360]</param>
	void ColorParticleSystems(float hue)
	{
		GetParticleSystems(out ParticleSystem surface, out ParticleSystem corona, out ParticleSystem flares);
		Debug.Log(hue.Remap(0, 360, 0, 1));
		//surface
		ColorParticleSystem(surface, hue.Remap(0, 360, 0, 1), .85f, .9f, out ParticleSystem.MinMaxGradient surfaceGradient);
		var surfaceColor = surface.colorOverLifetime;
		surfaceColor.color = surfaceGradient;

		//corona
		ColorParticleSystem(corona, (hue - 5).Remap(0, 360, 0, 1), 1f, .8f, out ParticleSystem.MinMaxGradient coronaGradient);
		var coronaColor = surface.colorOverLifetime;
		coronaColor.color = coronaGradient;

		//flares
		ColorParticleSystem(flares, (hue - 20).Remap(0, 360, 0, 1), 1f, .75f, out ParticleSystem.MinMaxGradient flaresGradient);
		var flaresColor = flares.colorOverLifetime;
		flaresColor.color = flaresGradient;
	}

	void ColorParticleSystem(ParticleSystem particleSystem, float hue, float saturation, float value, out ParticleSystem.MinMaxGradient colorOverLifetime)
	{
		var colorModule = particleSystem.colorOverLifetime;
		colorOverLifetime = colorModule.color;
		var gradient = colorOverLifetime.gradient;
		for (int i = 0; i < gradient.colorKeys.Length; i++)
		{
			gradient.colorKeys[i].color = Color.HSVToRGB(hue, saturation, value);
		}
		colorOverLifetime.gradient = gradient;
	}
	*/
#endregion
}