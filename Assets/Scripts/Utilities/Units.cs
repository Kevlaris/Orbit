using System;
using UnityEngine;

[Serializable]
public class Quantity
{
	public float amount; //{ get; private set; }
	public Length.Unit Unit;
	/// <summary>
	/// Determines whether the measurement unit can be changed.
	/// </summary>
	public bool isStatic;

	public Quantity(float amount)
	{
		this.amount = amount;
		Unit = Length.defaultUnit;
	}
	public Quantity(float amount, Length.Unit unit, bool isStatic = false)
	{
		this.amount = amount;
		Unit = unit;
		this.isStatic = isStatic;
	}
	public Quantity(Quantity quantity)
	{
		this.amount = quantity.amount;
		this.Unit = quantity.Unit;
		this.isStatic = quantity.isStatic;
	}

	public string ConvertToString()
	{
		string output = amount.ToString();
		output += " " + Enum.GetName(typeof(Length.Unit), Unit);
		return output;
	}

	public void Convert(Length.Unit unit)
	{
		if (isStatic) return;
		Quantity newQuantity = Length.Convert(this, unit);
		this.amount = newQuantity.amount;
		Unit = newQuantity.Unit;
	}

	#region Operators
	#region Quantity operations
	public static Quantity operator *(Quantity a, Quantity b)
	{
		Quantity q = new Quantity(a.amount, a.Unit);
		if (a.Unit == b.Unit)
		{
			q.amount *= b.amount;
		}
		else
		{
			q.amount *= Length.Convert(b, a.Unit).amount;
		}
		return q;
	}
	public static Quantity operator /(Quantity a, Quantity b)
	{
		Quantity q = new Quantity(a.amount, a.Unit);
		if (a.Unit == b.Unit)
		{
			q.amount /= b.amount;
		}
		else
		{
			q.amount /= Length.Convert(b, a.Unit).amount;
		}
		return q;
	}

	public static Quantity operator +(Quantity a, Quantity b)
	{
		Quantity q = new Quantity(a.amount, a.Unit);
		if (a.Unit == b.Unit)
		{
			q.amount += b.amount;
		}
		else
		{
			q.amount += Length.Convert(b, a.Unit).amount;
		}
		return q;
	}
	public static Quantity operator -(Quantity a, Quantity b)
	{
		Quantity q = new Quantity(a.amount, a.Unit);
		if (a.Unit == b.Unit)
		{
			q.amount -= b.amount;
		}
		else
		{
			q.amount -= Length.Convert(b, a.Unit).amount;
		}
		return q;
	}
	#endregion

	#region float operations
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

	public static implicit operator float(Quantity quantity)
	{
		Quantity q = new Quantity(quantity);
		q.Convert(Length.defaultUnit);
		return q.amount;

	}
	#endregion

	#region booleans
	public static bool operator >(Quantity a, Quantity b)
	{
		if (a.Unit == b.Unit)
		{
			return a.amount > b.amount;
		}
		else
		{
			return a.amount > Length.Convert(b, a.Unit).amount;
		}
	}
	public static bool operator <(Quantity a, Quantity b)
	{
		if (a.Unit == b.Unit)
		{
			return a.amount < b.amount;
		}
		else
		{
			return a.amount < Length.Convert(b, a.Unit).amount;
		}
	}
	#endregion
	#endregion
}

public static class Length
{
	public const string unitName = "length";
	public static readonly float[] conversionRates = { 1, 1000, 149597871000, 9460730472580000, 30856775812799588 };
	public enum Unit
	{
		m,
		km,
		AU,
		ly,
		pc
	}
	public const Unit defaultUnit = Unit.km;

	public static Quantity Convert(Quantity quantity, Unit unit)
	{
		if (unit == quantity.Unit)
		{
			//Debug.Log("Attempted conversion to same unit");
			return quantity;
		}
		else
		{
			if (quantity.Unit == Unit.m)
			{
				float newAmount = quantity / conversionRates[(int)unit];
				return new Quantity(newAmount, unit);
			}
			else
			{
				float meter = quantity * conversionRates[(int)quantity.Unit];
				float newAmount = meter / conversionRates[(int)unit];
				return new Quantity(newAmount, unit);
			}
			/*
			int steps =  Enum.GetNames(typeof(Unit))
			if (unit < quantity.unit)   // convert to a higher unit (m->km)
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
			*/
		}
	}
	/// <summary>
	/// Converts quantity to its world value.
	/// </summary>
	public static float ConvertToWorld(Quantity quantity)
	{
		if (quantity.Unit == defaultUnit)
		{
			return quantity * Universe.lengthScale;
		}
		else
		{
			Quantity newQuantity = quantity;
			quantity.Convert(defaultUnit);
			return newQuantity * Universe.lengthScale;
		}
	}
	public static Quantity ConvertFromWorld(float value, Unit unit)
	{
		float newValue = value / Universe.lengthScale;
		Quantity quantity = new Quantity(newValue, Unit.km);
		quantity.Convert(unit);
		return quantity;
	}
}