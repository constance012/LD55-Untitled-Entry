using System.Linq;
using System;

namespace CSTGames.Utility
{
	/// <summary>
	/// Provides some useful methods for manipulating Strings.
	/// </summary>
	public static class StringExtensions
	{
		public static string AddWhitespaceBeforeCapital(this string str)
		{
			return string.Concat(str.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
		}

		public static string AddHyphenBeforeNumber(this string str)
		{
			return string.Concat(str.Select(x => char.IsDigit(x) ? "-" + x : x.ToString())).TrimStart('-');
		}

		public static string ClearWhitespaces(this string str)
		{
			return new string(str.ToCharArray()
								 .Where(c => !char.IsWhiteSpace(c))
								 .ToArray());
		}
	}

	/// <summary>
	/// Provides some useful methods for manipulating Numbers.
	/// </summary>
	public static class NumberManipulator
	{
		/// <summary>
		/// Linearly converts a floating point number from one range to another, maintains ratio.
		/// <para />
		/// You can swap the min and max of a range to achieve inverse result, respectively.
		/// </summary>
		/// <param name="targetValue"></param>
		/// <param name="oldMin"></param>
		/// <param name="oldMax"></param>
		/// <param name="newMin"></param>
		/// <param name="newMax"></param>
		/// <returns> <c>newValue</c> A new converted value within the new range. </returns>
		public static float RangeConvert(float targetValue, float oldMin, float oldMax, float newMin, float newMax)
		{
			float oldRange = oldMax - oldMin;
			float newRange = newMax - newMin;

			// If the oldMax == oldMin, then just clamps the value directly within the new range.
			if (oldRange == 0f)
				return Math.Clamp(targetValue, newMin, newMax);
			else
				return ((targetValue - oldMin) * newRange / oldRange) + newMin;
		}

		/// <summary>
		/// Linearly converts an integer from one range to another, maintains ratio.
		/// <para />
		/// You can swap the min and max of a range to achieve inverse result, respectively.
		/// </summary>
		/// <param name="targetValue"></param>
		/// <param name="oldMin"></param>
		/// <param name="oldMax"></param>
		/// <param name="newMin"></param>
		/// <param name="newMax"></param>
		/// <returns> <c>newValue</c> A new converted value within the new range. </returns>
		public static int RangeConvert(int targetValue, int oldMin, int oldMax, int newMin, int newMax)
		{
			int oldRange = oldMax - oldMin;
			int newRange = newMax - newMin;

			if (oldRange == 0)
				return Math.Clamp(targetValue, newMin, newMax);
			else
				return ((targetValue - oldMin) * newRange / oldRange) + newMin;
		}
	}
}