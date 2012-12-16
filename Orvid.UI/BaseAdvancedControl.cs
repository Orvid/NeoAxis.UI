using System;
using Engine;
using Engine.UISystem;

namespace Orvid.UI
{
	public class BaseAdvancedControl : Control
	{
		//private bool mIsAltKeyDown = false;
		///// <summary>
		///// True if the alt key is currently down.
		///// </summary>
		//public bool IsAltKeyDown
		//{
		//    get { return mIsAltKeyDown; }
		//}
		//private bool mIsShiftKeyDown = false;
		///// <summary>
		///// True if the shift key is currently down.
		///// </summary>
		//public bool IsShiftKeyDown
		//{
		//    get { return mIsShiftKeyDown; }
		//}
		//private bool mIsControlKeyDown = false;
		///// <summary>
		///// True if the control key is currently down.
		///// </summary>
		//public bool IsControlKeyDown
		//{
		//    get { return mIsControlKeyDown; }
		//}

		private bool mOldFocused = false;
		protected override void OnTick(float delta)
		{
			base.OnTick(delta);
			if (Focused != mOldFocused)
			{
				if (Focused)
					OnFocused();
				else
					OnUnfocused();
				mOldFocused = Focused;
			}
		}

		//protected override bool OnKeyDown(KeyEvent e)
		//{
		//    if (!this.Focused || !this.IsEnabledInHierarchy())
		//    {
		//        return base.OnKeyDown(e);
		//    }
		//    switch (e.Key)
		//    {
		//        case EKeys.LAlt:
		//        case EKeys.RAlt:
		//        case EKeys.Alt:
		//            mIsAltKeyDown = true;
		//            break;
		//        case EKeys.LShift:
		//        case EKeys.RShift:
		//        case EKeys.Shift:
		//            mIsShiftKeyDown = true;
		//            break;
		//        case EKeys.LControl:
		//        case EKeys.RControl:
		//        case EKeys.Control:
		//            mIsControlKeyDown = true;
		//            break;
		//        default:
		//            base.OnKeyDown(e);
		//            break;
		//    }
		//    return true;
		//}

		//protected override bool OnKeyUp(KeyEvent e)
		//{
		//    switch (e.Key)
		//    {
		//        case EKeys.LAlt:
		//        case EKeys.RAlt:
		//        case EKeys.Alt:
		//            mIsAltKeyDown = false;
		//            break;
		//        case EKeys.LShift:
		//        case EKeys.RShift:
		//        case EKeys.Shift:
		//            mIsShiftKeyDown = false;
		//            break;
		//        case EKeys.LControl:
		//        case EKeys.RControl:
		//        case EKeys.Control:
		//            mIsControlKeyDown = false;
		//            break;
		//    }
		//    return base.OnKeyUp(e);
		//}

		protected virtual void OnFocused()
		{

		}

		protected virtual void OnUnfocused()
		{

		}
	}
}
