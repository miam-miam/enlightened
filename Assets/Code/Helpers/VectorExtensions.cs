using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Helpers
{
	public static class VectorExtensions
	{

		public static bool IsOnLine(this Vector2 target, Vector2 p1, Vector2 p2)
		{
			if (p2.x - p1.x == 0)
			{
				if (p2.y - p1.y == 0)
					return true;
				return false;
			}
			return (target.x - p1.x) / (p2.x - p1.x) == (target.y - p1.y) / (p2.y - p1.y);
		}

		/// <summary>
		/// TODO: This was written by ChatGPT and needs to be rewritten
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lineStart"></param>
		/// <param name="lineEnd"></param>
		/// <returns></returns>
		public static Vector2 ClosestPointOnLine(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			// Calculate the direction vector of the line
			Vector2 lineDirection = lineEnd - lineStart;

			// Calculate the vector from the line start to the point
			Vector2 pointToLineStart = point - lineStart;

			// Calculate the dot product of the point-to-line-start vector and the line direction
			float dot = Vector2.Dot(pointToLineStart, lineDirection);

			// Calculate the squared length of the line
			float lineLengthSquared = lineDirection.sqrMagnitude;

			// Calculate the parameter t for the closest point on the line
			float t = Mathf.Clamp01(dot / lineLengthSquared);

			// Calculate the closest point on the line
			Vector2 closestPoint = lineStart + t * lineDirection;

			return closestPoint;
		}

		public static Vector2 CalculatePerpendicularLine(this Vector2 point, Vector2 towardsPoint)
		{
			// Each line has 2 normals, use the one that takes us towards the towards point
			// Normal vector: (+/-y, x) depending on which one goes towards the towards point
			Vector2 lineNormal1 = new Vector2(-point.y, point.x).normalized;
			Vector2 lineNormal2 = new Vector2(point.y, -point.x).normalized;
			float angle = Vector2.Angle(lineNormal1, towardsPoint - point);
			return (angle > 180) ? lineNormal1 : lineNormal2;
		}

	}
}
