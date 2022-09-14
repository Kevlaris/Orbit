using UnityEngine;

public class PlaceInOrbit : MonoBehaviour
{
    public CelestialBody centralBody;
    public Quantity altitude;
	[SerializeField] float amount;
	[SerializeField] [Min(0)] int unit;
	Unit length;

	/* private void Awake()
	{
		length = Resources.Load<Unit>("Units/Length");
		altitude = new Quantity(0, length);
		unit = altitude.unit;
	} */

	private void Reset()
	{
		length = Resources.Load<Unit>("Units/Length");
		altitude = new Quantity(0, length);
		unit = altitude.unit;
		amount = altitude.amount;
	}

	private void OnValidate()
	{
		altitude.amount = amount;
		altitude.Convert(unit);
		amount = altitude.amount;
	}

	
}
