using UnityEngine;

namespace NohaSoftware.Utilities
{
	public static class Vector3Extensions
	{
		public static Vector3 RotateAroundVector(this Vector3 vector, Vector3 axis, float angle)
		{
			// First, calculate the rotation axis
			Vector3 rotationAxis = Vector3.Cross(axis, vector);

			// Then, calculate the angle of rotation using the dot product
			float rotationAngle = Mathf.Acos(Vector3.Dot(axis, vector) / (vector.magnitude * axis.magnitude)) * Mathf.Rad2Deg;

			// Finally, apply the rotation
			Quaternion rotation = Quaternion.AngleAxis(rotationAngle + angle, rotationAxis);
			return rotation * vector;
		}

		public static Vector3 SetX(this Vector3 vector, float x)
		{
			return new(x, vector.y, vector.z);
		}
		public static Vector3 SetY(this Vector3 vector, float y)
		{
			return new(vector.x, y, vector.z);
		}
		public static Vector3 SetZ(this Vector3 vector, float z)
		{
			return new(vector.x, vector.y, z);
		}
	}
}