/* Please Note:
 * 
 * This file is NOT in the public domain, it has been borrowed from the NeoAxis Wiki.
 * 
 * It was also modified so as to be more readable and usable to me personally.
 * (aka. mostly just naming/formatting changes)
 * 
 */
// Copyright (C) 2006-2012 NeoAxis Group Ltd.
//#define DebugDraw
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

					float xDif = nextPoint.X - point.X;
					float yDif = nextPoint.Y - point.Y;
					if (Math.Abs(yDif) > Math.Abs(xDif))
					{
						if (yDif > 0)
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X - halfThickness.X, point.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X + halfThickness.X, point.Y), color);
						}
						else
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X + halfThickness.X, point.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X - halfThickness.X, point.Y), color);
						}
					}
					else
					{
						if (xDif < 0)
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y - halfThickness.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y + halfThickness.Y), color);
						}
						else
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y + halfThickness.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y - halfThickness.Y), color);
						}
					}
				}
				else if (nPoint == points.Count - 1)
				{
					//last point

					Vec2 previousPoint = points[nPoint - 1];
					float xDif = point.X - previousPoint.X;
					float yDif = point.Y - previousPoint.Y;
					if (Math.Abs(yDif) > Math.Abs(xDif))
					{
						if (yDif > 0)
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X - halfThickness.X, point.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X + halfThickness.X, point.Y), color);
						}
						else
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X + halfThickness.X, point.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X - halfThickness.X, point.Y), color);
						}
					}
					else
					{
						if (xDif < 0)
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y - halfThickness.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y + halfThickness.Y), color);
						}
						else
						{
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y + halfThickness.Y), color);
							vertices[writeIndex++] = new GuiRenderer.TriangleVertex(new Vec2(point.X, point.Y - halfThickness.Y), color);
						}
					}
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

		public static GuiRenderer.TriangleVertex[] GenerateTrianglesFromCurve(GuiRenderer renderer, Curve curve, float step, Vec2 thickness, ColorValue color)
		{
			float maxTime = curve.Times[curve.Times.Count - 1];
			float end = maxTime + step;

			List<Vec2> points = new List<Vec2>();
			for (float time = 0; time < end; time += step)
			{
				points.Add(curve.CalculateValueByTime(time).ToVec2());
			}

#if DebugDraw
			Vec2 xHalf = new Vec2(thickness.X / 16, 0f);
			Vec2 yHalf = new Vec2(0f, thickness.Y / 16);
			float g = 1f;
			// The green progression here is so we can tell which
			// direction the curve was heading.
			foreach (Vec2 point in points)
			{
				renderer.AddQuad(new Rect(point - xHalf, point + yHalf), new ColorValue(1f, g, 0f));
				g -= 0.05f;
			}
#endif

			return GenerateTriangles(points, thickness, color);
		}
	}
}