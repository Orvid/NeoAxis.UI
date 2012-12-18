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

		public override Vec3 CalculateValueByTime(float time)
		{
			int baseI = 0;
			while (baseI < this.Times.Count && Times[baseI] < time) 
				baseI++;

			baseI--;

			if (baseI >= this.Times.Count - 2)
				return this.Values[baseI - 1];

			Vec3 valA = Values[baseI];
			Vec3 valB = Values[baseI + 1];
			float timeA = Times[baseI];

			return valA + ((valB - valA) * (time - timeA));
		}
	}
}
