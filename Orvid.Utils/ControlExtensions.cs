using System;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;

namespace Orvid.Utils
{
	public static class ControlExtensions
	{
		public static Vec2 ScreenFromValue(this Control c, Control.ScaleValue val)
		{
			var oldPos = c.Position;
			c.Position = val;
			var ret = c.GetScreenPosition();
			c.Position = oldPos;
			return ret;
		}
	}
}
