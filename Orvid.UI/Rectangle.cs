using System;
using System.ComponentModel;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;

namespace Orvid.UI
{
	/// <summary>
	/// Represents a simple rectangle that will be drawn.
	/// </summary>
	public class Rectangle : Control
	{
		private ColorValue mColor = new ColorValue(1f, 1f, 1f);
		/// <summary>
		/// The color of the rectangle.
		/// </summary>
		[Serialize]
		[Category("General")]
		[Description("The color of the rectangle.")]
		[DefaultValue("255 255 255")]
		public ColorValue Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		private float mBorderWidth = 0f;
		[Serialize]
		[DefaultValue(0f)]
		public float BorderWidth
		{
			get { return mBorderWidth; }
			set { mBorderWidth = Math.Abs(value); }
		}

		private ColorValue mBorderColor = new ColorValue(1f, 0f, 0f);

		protected override void OnRenderUI(GuiRenderer renderer)
		{
			Rect r = new Rect();
			var pos = this.GetScreenPosition();
			var sz = this.GetScreenSize();
			r.Left = pos.X;
			r.Right = r.Left + sz.X;
			r.Top = pos.Y;
			r.Bottom = r.Top + sz.Y;
			renderer.AddQuad(r, Color);

			if (mBorderWidth > 0)
			{
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.RightTop, r.RightBottom), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.RightBottom, r.RightTop), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.LeftBottom, r.LeftTop), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.LeftTop, r.LeftBottom), 0.1f, mBorderWidth, mBorderColor));

				float halfWidth = mBorderWidth / 3;
				r.Left -= halfWidth;

				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.LeftTop, r.RightTop), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.RightTop, r.LeftTop), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.RightBottom, r.LeftBottom), 0.1f, mBorderWidth, mBorderColor));
				renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, new StraightCurve(r.LeftBottom, r.RightBottom), 0.1f, mBorderWidth, mBorderColor));
			}
		}
	}
}
