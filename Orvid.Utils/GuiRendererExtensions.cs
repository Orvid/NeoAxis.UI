using System;
using System.Collections.Generic;
using Engine.MathEx;
using Engine.Renderer;

namespace Orvid.Utils
{
	public static class GuiRendererExtensions
	{
		private static GuiRenderer.TriangleVertex CircleVertex(float angle, Vec2 relPos, Vec2 radius, ColorValue color)
		{
			return new GuiRenderer.TriangleVertex((new Vec2(MathFunctions.Cos(angle), MathFunctions.Sin(angle)) * radius) + relPos, color);
		}

		public static void AddCircle(this GuiRenderer renderer, Vec2 center, ColorValue color, Vec2 radius, float startAngle = 0f, float endAngle = (float)Math.PI * 2f, float divisions = 16)
		{
			List<GuiRenderer.TriangleVertex> tris = new List<GuiRenderer.TriangleVertex>((int)(divisions * 3));
			float inc = (float)Math.PI / divisions;
			float cond = endAngle * (float)(1f - 1f / divisions);
			float curAngle;
			for (curAngle = startAngle; curAngle < cond; curAngle += inc)
			{
				tris.Add(new GuiRenderer.TriangleVertex(center, color));
				tris.Add(CircleVertex(curAngle, center, radius, color));
				tris.Add(CircleVertex(curAngle + inc, center, radius, color));
			}
			tris.Add(new GuiRenderer.TriangleVertex(center, color));
			tris.Add(CircleVertex(curAngle, center, radius, color));
			tris.Add(CircleVertex(endAngle, center, radius, color));
			renderer.AddTriangles(tris);
		}
	}
}