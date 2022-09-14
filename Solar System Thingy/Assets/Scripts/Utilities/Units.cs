using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
	public string unitName;
	public string[] units;              // names of units from biggest to smallest
	public float[] conversionRates;     // what number should we multiply the number with in order to get a smaller unit
	[Min(0)] public int defaultUnit;    // index of the default unit
}

[System.Serializable]
public class Quantity
{
	public float amount; //{ get; private set; }
	public Unit unitType; //{ get; private set; }
	public int unit { get; private set; }       // index of the unit

	public Quantity(float amount, Unit unitType)
	{
		this.amount = amount;
		this.unitType = unitType;
		unit = unitType.defaultUnit;
	}
	public Quantity(float amount, Unit unitType, int unit)
	{
		this.amount = amount;
		this.unitType = unitType;
		this.unit = unit;
	}

	public string ConvertToString()
	{
		string output = amount.ToString();
		output += " " + unitType.units[unit];
		return output;
	}

	public void Convert(int unit)
	{
		Quantity newQuantity = UnitConverter.Convert(this, unit);
		this.amount = newQuantity.amount;
		this.unit = newQuantity.unit;
		this.unitType = newQuantity.unitType;
	}


	#region Operators
	/// <summary>
	/// Multiply two quantities
	/// </summary>
	/// <returns>The multiple in the first quantity's unit</returns>
	public static float operator *(Quantity a, Quantity b)
	{
		if (a.unitType == b.unitType)
		{
			if (a.unit == b.unit)
			{
				return a.amount * b.amount;
			}
			else
			{
				return a.amount * UnitConverter.Convert(b, a.unit);
			}
		}
		else
		{
			Debug.LogWarning("Attempted the multiplication of two non-mathcing unit types");
			return -1;
		}
	}
	public static float operator /(Quantity a, Quantity b)
	{
		if (a.unitType == b.unitType)
		{
			if (a.unit == b.unit)
			{
				return a.amount / b.amount;
			}
			else
			{
				return a.amount / UnitConverter.Convert(b, a.unit);
			}
		}
		else
		{
			Debug.LogWarning("Attempted the division of two non-mathcing unit types");
			return -1;
		}
	}
	public static float operator *(Quantity a, float b)
	{
		return a.amount * b;
	}
	public static float operator *(float a, Quantity b)
	{
		return a * b.amount;
	}
	public static float operator /(Quantity a, float b)
	{
		return a.amount / b;
	}
	public static float operator /(float a, Quantity b)
	{
		return a / b.amount;
	}

	public static float operator +(Quantity a, Quantity b)
	{
		if (a.unitType == b.unitType)
		{
			if (a.unit == b.unit)
			{
				return a.amount + b.amount;
			}
			else
			{
				return a.amount + UnitConverter.Convert(b, a.unit).amount;
			}
		}
		else
		{
			Debug.LogWarning("Attempted the sum of two non-mathcing unit types");
			return -1;
		}
	}
	public static float operator -(Quantity a, Quantity b)
	{
		if (a.unitType == b.unitType)
		{
			if (a.unit == b.unit)
			{
				return a.amount - b.amount;
			}
			else
			{
				return a.amount - UnitConverter.Convert(b, a.unit).amount;
			}
		}
		else
		{
			Debug.LogWarning("Attempted the subtraction of two non-mathcing unit types");
			return -1;
		}
	}
	#endregion
}

public static class UnitConverter
{
    public static Quantity Convert(Quantity quantity, int unit)
	{
		Unit unitType = quantity.unitType;

		if (unit == quantity.unit)
		{
			Debug.Log("Attempted conversion to same unit");
			return quantity;
		}
		else if (unit > unitType.units.Length || unit < 0)
		{
			Debug.LogError("Failed Unit Conversion: unit out of bounds");
			return quantity;
		}
		else
		{
			if (unit < quantity.unit)	// convert to a higher unit (m->km)
			{
				float newAmount = quantity.amount;
				for (int i = quantity.unit - 1; i >= unit; i--)
				{
					newAmount /= unitType.conversionRates[i];
				}
				return new Quantity(newAmount, unitType, unit);
			}
			else
			{
				float newAmount = quantity.amount;
				for (int i = quantity.unit; i < unit; i++)
				{
					newAmount *= unitType.conversionRates[i];
				}
				return new Quantity(newAmount, unitType, unit);
			}
		}
	}
	public static float ConvertToWorld(Quantity quantity)
	{
		Unit unitType = quantity.unitType;
		if (unitType.unitName != "length")
		{
			Debug.LogError("Attempted converting a non-length quantity to world value");
			return quantity.amount;
		}

		if (quantity.unit == unitType.defaultUnit)
		{
			return quantity * Universe.lengthScale;
		}
		else
		{
			Quantity newQuantity = quantity;
			quantity.Convert(unitType.defaultUnit);
			return newQuantity * Universe.lengthScale;
		}
	}
}
