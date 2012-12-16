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
		}
	}
}
