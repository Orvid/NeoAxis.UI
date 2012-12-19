//#define DebugDraw
using System;
using System.ComponentModel;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;
using Orvid.Utils;

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
		[DefaultValue(typeof(ColorValue), "255 255 255")]
		public ColorValue Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		private ScaleValue mBorderWidth = new ScaleValue(ScaleType.Pixel, Vec2.Zero);
		/// <summary>
		/// The width of the border.
		/// </summary>
		[Serialize]
		[Category("General")]
		[DisplayName("Border Width")]
		[Description("The width of the border.")]
		[DefaultValue(typeof(ScaleValue), "Pixel 0 0")]
		public ScaleValue BorderWidth
		{
			get { return mBorderWidth; }
			set { mBorderWidth = value; }
		}

		private ScaleValue mBorderRadius = new ScaleValue(ScaleType.Pixel, Vec2.Zero);
		/// <summary>
		/// The radius of the corners of the rectangle.
		/// </summary>
		[Serialize]
		[Category("General")]
		[DisplayName("Border Radius")]
		[Description("The radius of the corners of the rectangle.")]
		[DefaultValue(typeof(ScaleValue), "Pixel 0 0")]
		public ScaleValue BorderRadius
		{
			get { return mBorderRadius; }
			set { mBorderRadius = value; }
		}

		private ColorValue mBorderColor = new ColorValue(1f, 1f, 1f);
		/// <summary>
		/// The color of the border around the rectangle.
		/// </summary>
		[Serialize]
		[Category("General")]
		[DisplayName("Border Color")]
		[Description("The color of the border around the rectangle.")]
		[DefaultValue(typeof(ColorValue), "255 255 255")]
		public ColorValue BorderColor
		{
			get { return mBorderColor; }
			set { mBorderColor = value; }
		}

		protected override void OnRenderUI(GuiRenderer renderer)
		{
			Rect r = new Rect();
			var pos = this.GetScreenPosition();
			var sz = this.GetScreenSize();
			r.Left = pos.X;
			r.Right = r.Left + sz.X;
			r.Top = pos.Y;
			r.Bottom = r.Top + sz.Y;
			if (mBorderRadius.Value != Vec2.Zero)
			{
#if DebugDraw
				ColorValue debugColor = new ColorValue(0f, 1f, 0f, .5f);
				ColorValue debugColor2 = new ColorValue(0f, 0f, 1f, .5f);
#endif

				Vec2 screenRelRad = this.ScreenFromValue(mBorderRadius);

				// By using clipping and a set of 3 rectangles, we ensure that each point
				// on the screen is only drawn once, thus allowing the alpha value to be
				// used in the color without it looking weird.

				// Top-Left
#if DebugDraw
				renderer.AddCircle(r.LeftTop + screenRelRad, debugColor, screenRelRad, (float)Math.PI, (float)(Math.PI * 1.5f));
#else
				renderer.AddCircle(r.LeftTop + screenRelRad, mColor, screenRelRad, (float)Math.PI, (float)(Math.PI * 1.5f));
#endif

				// Top-Right
#if DebugDraw
				renderer.AddCircle(new Vec2(r.Right - screenRelRad.X, r.Top + screenRelRad.Y), debugColor, screenRelRad, (float)(Math.PI * 1.5f));
#else
				renderer.AddCircle(new Vec2(r.Right - screenRelRad.X, r.Top + screenRelRad.Y), mColor, screenRelRad, (float)(Math.PI * 1.5f));
#endif

				// Bottom-Right
#if DebugDraw
				renderer.AddCircle(new Vec2(r.Right - screenRelRad.X, r.Bottom - screenRelRad.Y), debugColor, screenRelRad, 0f, (float)(Math.PI * 0.5f));
#else
				renderer.AddCircle(new Vec2(r.Right - screenRelRad.X, r.Bottom - screenRelRad.Y), mColor, screenRelRad, 0f, (float)(Math.PI * 0.5f));
#endif

				// Bottom-Left
#if DebugDraw
				renderer.AddCircle(new Vec2(r.Left + screenRelRad.X, r.Bottom - screenRelRad.Y), debugColor, screenRelRad, (float)(Math.PI * 0.5), (float)Math.PI);
#else
				renderer.AddCircle(new Vec2(r.Left + screenRelRad.X, r.Bottom - screenRelRad.Y), mColor, screenRelRad, (float)(Math.PI * 0.5), (float)Math.PI);
#endif

				// Middle-Middle
#if DebugDraw
				renderer.AddQuad(new Rect(r.Left, r.Top + screenRelRad.Y, r.Right, r.Bottom - screenRelRad.Y), debugColor2);
#else
				renderer.AddQuad(new Rect(r.Left, r.Top + screenRelRad.Y, r.Right, r.Bottom - screenRelRad.Y), mColor);
#endif
				// Middle-Top
				renderer.AddQuad(new Rect(r.Left + screenRelRad.X, r.Top, r.Right - screenRelRad.X, r.Top + screenRelRad.Y), mColor);
				// Middle-Bottom
				renderer.AddQuad(new Rect(r.Left + screenRelRad.X, r.Bottom - screenRelRad.Y, r.Right - screenRelRad.X, r.Bottom), mColor);


				if (mBorderWidth.Value != Vec2.Zero)
				{
					Vec2 screenRel = this.ScreenFromValue(mBorderWidth);

					float xHalfWidth = screenRel.X / 2;
					float yHalfWidth = screenRel.Y / 2;
					// Top
					renderer.AddQuad(new Rect(r.Left + screenRelRad.X, r.Top + yHalfWidth, r.Right - screenRelRad.X, r.Top - yHalfWidth), mBorderColor);
					// Right
					renderer.AddQuad(new Rect(r.Right - xHalfWidth, r.Top + screenRelRad.Y, r.Right + xHalfWidth, r.Bottom - screenRelRad.Y), mBorderColor);
					// Bottom
					renderer.AddQuad(new Rect(r.Left + screenRelRad.X, r.Bottom - yHalfWidth, r.Right - screenRelRad.X, r.Bottom + yHalfWidth), mBorderColor);
					// Left
					renderer.AddQuad(new Rect(r.Left - xHalfWidth, r.Top + screenRelRad.Y, r.Left + xHalfWidth, r.Bottom - screenRelRad.Y), mBorderColor);

					// Top-Left
					BezierCurve curve = new BezierCurve();
					curve.AddValue(0, new Vec3(r.Left, r.Top + screenRelRad.Y, 0f));
					curve.AddValue(1, new Vec3(r.LeftTop, 0f));
					curve.AddValue(2, new Vec3(r.Left + screenRelRad.X, r.Top, 0f));
					renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, curve, 0.1f, screenRel, mBorderColor));

					// Top-Right
					curve = new BezierCurve();
					curve.AddValue(0, new Vec3(r.Right - screenRelRad.X, r.Top, 0f));
					curve.AddValue(1, new Vec3(r.RightTop, 0f));
					curve.AddValue(2, new Vec3(r.Right, r.Top + screenRelRad.Y, 0f));
					renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, curve, 0.1f, screenRel, mBorderColor));

					// Bottom-Right
					curve = new BezierCurve();
					curve.AddValue(0, new Vec3(r.Right, r.Bottom - screenRelRad.Y, 0f));
					curve.AddValue(1, new Vec3(r.RightBottom, 0f));
					curve.AddValue(2, new Vec3(r.Right - screenRelRad.X, r.Bottom, 0f));
					renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, curve, 0.1f, screenRel, mBorderColor));

					// Bottom-Left
					curve = new BezierCurve();
					curve.AddValue(0, new Vec3(r.Left + screenRelRad.X, r.Bottom, 0f));
					curve.AddValue(1, new Vec3(r.LeftBottom, 0f));
					curve.AddValue(2, new Vec3(r.Left, r.Bottom - screenRelRad.Y, 0f));
					renderer.AddTriangles(LineRenderer.GenerateTrianglesFromCurve(renderer, curve, 0.1f, screenRel, mBorderColor));
				}
			}
			else
			{
				// Center
				renderer.AddQuad(r, Color);

				if (mBorderWidth.Value != Vec2.Zero)
				{
					Vec2 screenRel = this.ScreenFromValue(mBorderWidth);

					float xHalfWidth = screenRel.X / 2;
					float yHalfWidth = screenRel.Y / 2;
					// Top
					renderer.AddQuad(new Rect(r.Left, r.Top + yHalfWidth, r.Right + xHalfWidth, r.Top - yHalfWidth), mBorderColor);
					// Right
					renderer.AddQuad(new Rect(r.Right - xHalfWidth, r.Bottom + yHalfWidth, r.Right + xHalfWidth, r.Top), mBorderColor);
					// Bottom
					renderer.AddQuad(new Rect(r.Left - xHalfWidth, r.Bottom + yHalfWidth, r.Right, r.Bottom - yHalfWidth), mBorderColor);
					// Left
					renderer.AddQuad(new Rect(r.Left - xHalfWidth, r.Bottom, r.Left + xHalfWidth, r.Top - yHalfWidth), mBorderColor);
				}
			}
		}
	}
}
