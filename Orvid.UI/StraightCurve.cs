using System;
using Engine.MathEx;

namespace Orvid.UI
{
	public sealed class StraightCurve : Curve
	{
		public StraightCurve(params Vec3[] points) : base()
		{
			for(int i = 0; i < points.Length; i++)
			{
				this.AddValue(i, points[i]);
			}
		}

		public StraightCurve(params Vec2[] points) : base()
		{
			for (int i = 0; i < points.Length; i++)
			{
				this.AddValue(i, new Vec3(points[i], 0));
			}
		}
	}
}
