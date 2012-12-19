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
			var oldSz = c.Size;
			c.Size = val;
			var ret = c.GetScreenSize();
			c.Size = oldSz;
			return ret;
		}
	}
}
