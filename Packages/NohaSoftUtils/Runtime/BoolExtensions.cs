using System.Collections.Generic;

namespace NohaSoftware.Utilities
{
	public static class BoolExtensions
	{
		public static bool And(this IEnumerable<bool> bools)
		{
			foreach (bool b in bools)
			{
				if (!b) return false;
			}
			return true;
		}
		public static bool Or(this IEnumerable<bool> bools)
		{
			foreach (bool b in bools)
			{
				if (b) return true;
			}
			return false;
		}
		public static bool Nor(this IEnumerable<bool> bools)
		{
			return !bools.Or();
		}
	}
}