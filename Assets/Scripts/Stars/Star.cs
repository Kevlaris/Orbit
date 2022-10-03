using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Star : CelestialBody
{
	//[Header("Star Attributes")]
	// [Range(0,360)] public float hue;
	public Color chromaticity;
	public float solarRadius;
	public float solarLuminosity;
	public float absoluteMagnitude;
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
		meshHolder.localScale = 2 * Length.ConvertToWorld(radius) * Vector3.one;
		//chromaticity = Color.HSVToRGB(hue.Remap(0,360,0,1), 1, 1);
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
		stellarClass = StellarClassification.Classify(temperature, solarLuminosity * Universe.solarLuminosity);
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

	void CalibrateParticleSystems()
	{
		ScaleParticleSystems(new Quantity(solarRadius * Universe.solarRadius, Length.Unit.km));
		//ColorParticleSystems(hue);
	}

	public void ScaleParticleSystems(Quantity radius)
	{
		particlePrefab = Universe.solarParticleStandard;
		float multiplier = Length.Convert(radius, Length.Unit.km).amount / Length.Convert(Universe.solarRadius, Length.Unit.km).amount;

		// prefab particle systems
		ParticleSystem surfacePrefab = particlePrefab.GetComponent<ParticleSystem>();
		ParticleSystem coronaPrefab = particlePrefab.transform.GetChild(0).GetComponent<ParticleSystem>();
		ParticleSystem flaresPrefab = particlePrefab.transform.GetChild(1).GetComponent<ParticleSystem>();
		// actual particle systems
		GetParticleSystems(out ParticleSystem surface, out ParticleSystem corona, out ParticleSystem flares);

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

	void GetParticleSystems(out ParticleSystem surface, out ParticleSystem corona, out ParticleSystem flares)
	{
		surface = transform.GetChild(1).GetComponent<ParticleSystem>();
		corona = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
		flares = transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();
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
}