/* Please Note:
 * 
 * This file is NOT in the public domain, it has been borrowed from the NeoAxis Wiki.
 * 
 * It was also modified so as to be more readable and usable to me personally.
 * (aka. mostly just naming/formatting changes)
 * 
 */
// Copyright (C) 2006-2012 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;

namespace Orvid.UI
{
	public static class LineRenderer
	{
		public static GuiRenderer.TriangleVertex[] GenerateStrips(IList<Vec2> points, Vec2 thickness, ColorValue color)
		{
			Vec2 halfThickness = thickness * .5f;

			GuiRenderer.TriangleVertex[] vertices = new GuiRenderer.TriangleVertex[points.Count * 2];
			int writeIndex = 0;

			for (int nPoint = 0; nPoint < points.Count; nPoint++)
			{
				Vec2 point = points[nPoint];

				if (nPoint == 0)
				{
					//first point
					Vec2 nextPoint = points[nPoint + 1];

					float angle = MathFunctions.ATan(nextPoint.Y - point.Y, nextPoint.X - point.X);
					Vec2 positionOffset = new Vec2(MathFunctions.Cos(angle), MathFunctions.Sin(angle)) * halfThickness;
					angle += MathFunctions.PI * .5f;
					Vec2 thicknessOffset = new Vec2(MathFunctions.Cos(angle), MathFunctions.Sin(angle)) * halfThickness;

					Vec2 p1 = point + positionOffset + thicknessOffset;
					Vec2 p2 = point + positionOffset - thicknessOffset;
					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(p1, color);
					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(p2, color);
				}
				else if (nPoint == points.Count - 1)
				{
					//last point

					Vec2 previousPoint = points[nPoint - 1];

					float angle = MathFunctions.ATan(point.Y - previousPoint.Y, point.X - previousPoint.X);
					Vec2 positionOffset = new Vec2(MathFunctions.Cos(angle), MathFunctions.Sin(angle)) * halfThickness;
					angle += MathFunctions.PI * .5f;
					Vec2 thicknessOffset = new Vec2(MathFunctions.Cos(angle), MathFunctions.Sin(angle)) * halfThickness;

					Vec2 p1 = point + positionOffset + thicknessOffset;
					Vec2 p2 = point + positionOffset - thicknessOffset;
					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(p1, color);
					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(p2, color);
				}
				else
				{
					Vec2 previousPoint = points[nPoint - 1];
					Vec2 nextPoint = points[nPoint + 1];

					float angle1 = MathFunctions.ATan(point.Y - previousPoint.Y, point.X - previousPoint.X);
					angle1 += MathFunctions.PI * .5f;

					float angle2 = MathFunctions.ATan(nextPoint.Y - point.Y, nextPoint.X - point.X);
					angle2 += MathFunctions.PI * .5f;

					Vec2 offset1 = new Vec2(MathFunctions.Cos(angle1), MathFunctions.Sin(angle1)) * halfThickness;
					Vec2 offset2 = new Vec2(MathFunctions.Cos(angle2), MathFunctions.Sin(angle2)) * halfThickness;

					Vec2 intersectPoint1;
					{
						Vec2 line1Start = previousPoint + offset1;
						Vec2 line1End = point + offset1;

						Vec2 line2Start = nextPoint + offset2;
						Vec2 line2End = point + offset2;

						if (!MathUtils.IntersectRayRay(line1Start, line1End, line2Start, line2End, out intersectPoint1))
							intersectPoint1 = line1End;
					}

					Vec2 intersectPoint2;
					{
						Vec2 line1Start = previousPoint - offset1;
						Vec2 line1End = point - offset1;

						Vec2 line2Start = nextPoint - offset2;
						Vec2 line2End = point - offset2;

						if (!MathUtils.IntersectRayRay(line1Start, line1End, line2Start, line2End, out intersectPoint2))
							intersectPoint2 = line1End;
					}

					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(intersectPoint1, color);
					vertices[writeIndex++] = new GuiRenderer.TriangleVertex(intersectPoint2, color);
				}
			}

			return vertices;
		}

		public static GuiRenderer.TriangleVertex[] GenerateTriangles(IList<Vec2> points, Vec2 thickness, ColorValue color)
		{
			GuiRenderer.TriangleVertex[] strips = GenerateStrips(points, thickness, color);

			int count = (strips.Length - 2) * 3;
			GuiRenderer.TriangleVertex[] triangles = new GuiRenderer.TriangleVertex[count];

			int readIndex;
			int writeIndex = 0;

			for (readIndex = 0; readIndex < 3; readIndex++)
				triangles[writeIndex++] = strips[readIndex];

			for (; readIndex < strips.Length; readIndex++)
			{
				GuiRenderer.TriangleVertex v1 = triangles[writeIndex - 2];
				GuiRenderer.TriangleVertex v2 = triangles[writeIndex - 1];

				triangles[writeIndex++] = v1;
				triangles[writeIndex++] = v2;
				triangles[writeIndex++] = strips[readIndex];
			}

			return triangles;
		}

		public static void DrawCurvePoints(GuiRenderer renderer, Curve curve, float thickness, ColorValue color)
		{
			Vec2 thicknessXY = new Vec2(thickness / renderer.AspectRatio, thickness);

			Vec2 halfThickness = thicknessXY * .5f;
			foreach (Vec3 point in curve.Values)
			{
				renderer.AddQuad(new Rect(point.ToVec2() - halfThickness, point.ToVec2() + halfThickness), color);
			}
		}

		public static GuiRenderer.TriangleVertex[] GenerateTrianglesFromCurve(GuiRenderer renderer, Curve curve, float step, float thickness, ColorValue color)
		{
			float maxTime = curve.Times[curve.Times.Count - 1];
			float end = maxTime + step;

			List<Vec2> points = new List<Vec2>();
			for (float time = 0; time < end; time += step)
			{
				float t = time;
				if (t > maxTime)
					t = maxTime;
				points.Add(curve.CalculateValueByTime(t).ToVec2());
			}

			Vec2 thicknessXY = new Vec2(thickness / renderer.AspectRatio, thickness);

			return GenerateTriangles(points, thicknessXY, color);
		}

		//public static void Test(GuiRenderer renderer)
		//{
		//    //draw green uniform cubic curve
		//    {
		//        UniformCubicSpline curve = new UniformCubicSpline();
		//        curve.AddValue(0, new Vec3(.1f, .1f, 0));
		//        curve.AddValue(1, new Vec3(.5f, .07f, 0));
		//        curve.AddValue(2, new Vec3(.9f, .2f, 0));
		//        curve.AddValue(3, new Vec3(.6f, .9f, 0));
		//        curve.AddValue(4, new Vec3(.3f, .5f, 0));
		//        curve.AddValue(5, new Vec3(.9f, .6f, 0));
		//        curve.AddValue(6, new Vec3(.9f, .4f, 0));
		//        curve.AddValue(7, new Vec3(.2f, .5f, 0));
		//        curve.AddValue(8, new Vec3(.1f, .9f, 0));

		//        DrawCurvePoints(renderer, curve, .01f, new ColorValue(0, 1, 0));

		//        GuiRenderer.TriangleVertex[] vertices = GenerateTrianglesFromCurve(renderer, curve, .05f, .006f, new ColorValue(0, 1, 0, .7f));
		//        renderer.AddTriangles(vertices);
		//    }

		//    //draw red bezier curve
		//    {
		//        BezierCurve curve = new BezierCurve();
		//        curve.AddValue(0, new Vec3(.1f, .2f, 0));
		//        curve.AddValue(1, new Vec3(.2f, .7f, 0));
		//        curve.AddValue(2, new Vec3(.9f, .1f, 0));
		//        curve.AddValue(3, new Vec3(.9f, .9f, 0));

		//        DrawCurvePoints(renderer, curve, .015f, new ColorValue(1, 0, 0));

		//        GuiRenderer.TriangleVertex[] vertices = GenerateTrianglesFromCurve(
		//            renderer, curve, .1f, .01f, new ColorValue(1, 0, 0, .7f));
		//        renderer.AddTriangles(vertices);
		//    }
		//}
	}
}