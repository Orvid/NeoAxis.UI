using System;
using System.Drawing.Design;
using System.ComponentModel;
using Engine;
using Engine.MathEx;
using Engine.Renderer;
using Engine.UISystem;

namespace Orvid.UI
{
	public class AdvancedEditBox : BaseAdvancedControl
	{
		private bool mEnableMasking = false;
		[Serialize]
		[Category("Masking")]
		[Description("If true, this causes all text entered to be masked before being displayed.")]
		[DefaultValue(false)]
		public bool EnableMasking
		{
			get { return mEnableMasking; }
			set { mEnableMasking = value; UpdateMask(realText); }
		}

		private char maskChar = '\u25CF'; // Black Circle
		[Serialize]
		[Category("Masking")]
		[Description("The character to mask with if masking is enabled.")]
		[DefaultValue('\u25CF')]
		public char MaskCharacter
		{
			get { return maskChar; }
			set { maskChar = value; UpdateMask(realText); }
		}

		private string realText = "";
		[Serialize]
		[Category("General")]
		[Description("The current text in this EditBox. This will be the unmasked text if masking is enabled.")]
		[DefaultValue("")]
		public override string Text
		{
			get { return realText; }
			set { UpdateMask(value); }
		}

		private int mMaxCharacters = ushort.MaxValue;
		[Serialize]
		[Category("General")]
		[Description("The maximum number of characters that can be entered into this edit box.")]
		[DefaultValue(ushort.MaxValue)]
		public int MaxCharacters
		{
			get { return mMaxCharacters; }
			set { mMaxCharacters = value; UpdateMask(realText); }
		}

		private bool mEnableMultilineInput = false;
		[Serialize]
		[Category("General")]
		[Description("True if multi-line input should be enabled.")]
		[DefaultValue(false)]
		public bool EnableMultilineInput
		{
			get { return mEnableMultilineInput; }
			set { mInnerText.WordWrap = mEnableMultilineInput = value; }
		}

		private readonly TextBox mEmptyText = new TextBox() 
		{
			TextColor = new ColorValue(1f, 1f, 1f, .75f), 
			TextHorizontalAlign = HorizontalAlign.Left, 
			TextVerticalAlign = VerticalAlign.Top,
			WordWrap = true,
		};
		[Serialize]
		[Category("General")]
		[Description("The text to display when this control is empty and is not focused.")]
		[Editor("Engine.UISystem.Editor.EControlTextEditor, UISystem.Editor", typeof(UITypeEditor))]
		[DefaultValue("")]
		public string EmptyText
		{
			get { return mEmptyText.Text; }
			set { mEmptyText.Text = value; }
		}

		[Serialize]
		[Category("General")]
		[Description("The color of the EmptyText.")]
		[DefaultValue(typeof(ColorValue), "255 255 255 192")]
		public ColorValue EmptyTextColor
		{
			get { return mEmptyText.TextColor; }
			set { mEmptyText.TextColor = value; }
		}

		[Serialize]
		[Category("General")]
		[Description("The font to use for the EmptyText.")]
		[Editor("Engine.UISystem.Editor.ETextBoxFontEditor, UISystem.Editor", typeof(UITypeEditor))]
		public Font EmptyTextFont
		{
			get { return mEmptyText.Font; }
			set { mEmptyText.Font = value; }
		}

		[Serialize]
		[Category("Layout")]
		[Description("The horizontal align to use for the EmptyText.")]
		[DefaultValue(typeof(HorizontalAlign), "Left")]
		public HorizontalAlign EmptyTextHorizontalAlign
		{
			get { return mEmptyText.TextHorizontalAlign; }
			set { mEmptyText.TextHorizontalAlign = value; UpdateLabels(); }
		}

		[Serialize]
		[Category("Layout")]
		[Description("The vertical align to use for the EmptyText.")]
		[DefaultValue(typeof(VerticalAlign), "Top")]
		public VerticalAlign EmptyTextVerticalAlign
		{
			get { return mEmptyText.TextVerticalAlign; }
			set { mEmptyText.TextVerticalAlign = value; UpdateLabels(); }
		}

		private Recti mPadding = new Recti(7, 5, 5, 5);
		[Serialize]
		[Category("Layout")]
		[DefaultValue(typeof(Recti), "7 5 5 5")]
		public Recti Padding
		{
			get { return mPadding; }
			set { mPadding = value; UpdateLabels(); }
		}

		private Rectangle mCursorRect = new Rectangle()
		{
			Size = new ScaleValue(ScaleType.Pixel, new Vec2(1f, 10f)),
			Color = new ColorValue(1f, 0f, 0f),
			Visible = false,
		};
		[Serialize]
		[Category("Animation")]
		[Description("The color of the cursor.")]
		[DefaultValue(typeof(ColorValue), "255 255 255 255")]
		public ColorValue CursorColor
		{
			get { return mCursorRect.Color; }
			set { mCursorRect.Color = value; }
		}

		private bool mEnableCursorBlink = true;
		[Serialize]
		[Category("Animation")]
		[Description("If this is true, a blinking cursor will be placed after the text while the control is focused.")]
		[DefaultValue(true)]
		public bool EnableCursorBlink
		{
			get { return mEnableCursorBlink; }
			set { mEnableCursorBlink = value; if (!value) CursorVisible = false; }
		}

		private float mCursorBlinkRate = 500f;
		[Serialize]
		[Category("Animation")]
		[Description("The number of milliseconds between the toggling of the cursor.")]
		[DefaultValue(500f)]
		public float CursorBlinkRate
		{
			get { return mCursorBlinkRate; }
			set { mCursorBlinkRate = value; }
		}

		private readonly TextBox mInnerText = new TextBox()
		{
			TextColor = new ColorValue(1f, 1f, 1f, 1f),
			TextHorizontalAlign = HorizontalAlign.Left,
			TextVerticalAlign = VerticalAlign.Top,
			SupportLocalization = false,
		};
		[Serialize]
		[Category("General")]
		[Description("The color to use when displaying the user's text.")]
		[DefaultValue(typeof(ColorValue), "255 255 255 255")]
		public ColorValue TextColor
		{
			get { return mInnerText.TextColor; }
			set { mInnerText.TextColor = value; }
		}

		[Serialize]
		[Category("Layout")]
		[Description("The horizontal align to use for the text that the user enters.")]
		[DefaultValue(typeof(HorizontalAlign), "Left")]
		public HorizontalAlign TextHorizontalAlign
		{
			get { return mInnerText.TextHorizontalAlign; }
			set { mInnerText.TextHorizontalAlign = value; UpdateLabels(); }
		}

		[Serialize]
		[Category("Layout")]
		[Description("The vertical align to use for the text that the user enters.")]
		[DefaultValue(typeof(VerticalAlign), "Top")]
		public VerticalAlign TextVerticalAlign
		{
			get { return mInnerText.TextVerticalAlign; }
			set { mInnerText.TextVerticalAlign = value; UpdateLabels(); }
		}

		public override bool CanFocus { get { return IsEnabledInHierarchy(); } }

		protected override void OnAttach()
		{
			base.OnAttach();

			mEmptyText.Size = this.Size;
			mInnerText.Size = this.Size;
			this.Controls.Add(mInnerText);
			this.Controls.Add(mEmptyText);
			this.Controls.Add(mSelectionRect);
			this.Controls.Add(mCursorRect);
			mInnerText.TopMost = true;
			mEmptyText.TopMost = true;
			mSelectionRect.TopMost = true;
			mCursorRect.TopMost = true;
			if (EmptyText == "")
				mEmptyText.Visible = false;

			base.TextChange += sender => UpdateMask(sender.Text);
		}

		private bool CursorVisible
		{
			get { return mCursorRect.Visible; }
			set { mCursorRect.Visible = value; }
		}
		private float mLastCursorBlink = 0f;
		protected override void OnTick(float delta)
		{
			base.OnTick(delta);

			if (EnableCursorBlink && Focused)
			{
				mLastCursorBlink += delta * 1000;
				if (mLastCursorBlink >= CursorBlinkRate)
				{
					CursorVisible = !CursorVisible;
					mLastCursorBlink = 0f;
				}
			}
		}

		protected override void OnResize()
		{
			base.OnResize();
			UpdateLabels();
		}

		private int mInnerCursorIndex = 0;
		private int mCursorIndex
		{
			get { return mInnerCursorIndex; }
			set
			{
				mSelectionEndIndex = mInnerCursorIndex = value;
			}
		}
		private int mSelectionStartIndex = 0;
		private int mSelectionEndIndex = 0;
		private bool mSelecting
		{
			get { return mSelectionRect.Visible; }
			set { mSelectionRect.Visible = value; }
		}
		private bool mShiftDown = false;

		private Rectangle mSelectionRect = new Rectangle()
		{
			Position = new ScaleValue(ScaleType.Parent, new Vec2(0f, 0.2f)),
			Size = new ScaleValue(ScaleType.Parent, new Vec2(0.1f, 0.6f)),
			Color = new ColorValue(0f, 0f, 0.5f, 0.75f),
			Visible = false,
		};

		protected override void OnRenderUI(GuiRenderer renderer)
		{
			base.OnRenderUI(renderer);
			string txt = mInnerText.Text;
			mInnerText.Text = txt;
			Font f = renderer.DefaultFont;
			if (CursorVisible)
			{
				if (mEnableMultilineInput)
				{
					//f.GetWordWrapLines(renderer, txt, 
				}
				else
				{
					int w;
					int ti;
					Vec2i drawOffset;
					Vec2i drawSize;
					Vec2i textCoords;
					f.GetTrueTypeCharacterInfo(renderer, 'W', out drawSize, out w, out drawOffset, out ti, out textCoords);
					drawSize += drawOffset;
					mCursorRect.Size = new ScaleValue(ScaleType.Pixel, new Vec2(1f, drawSize.Y + 2));


					if (txt.Length > 0 && mCursorIndex != txt.Length)
					{
						txt = txt.Substring(0, mCursorIndex);
					}
					float tLen = f.GetTextLength(renderer, txt);
					tLen = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Screen, new Vec2(tLen, 0))).X;
					var b = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Pixel, new Vec2(mPadding.Left, mPadding.Top)));
					b.X += tLen;
					mCursorRect.Position = new ScaleValue(ScaleType.Parent, b);
				}
			}
			txt = mInnerText.Text;
			if (mSelecting)
			{
				float ySizeRelativeToParentSize = this.GetLocalOffsetByValue(mCursorRect.Size).Y;

				if (mEnableMultilineInput)
				{

				}
				else
				{
					if (mSelectionEndIndex < mSelectionStartIndex)
					{
						if (CursorVisible)
						{
							mSelectionRect.Position = mCursorRect.Position;
						}
						else
						{
							string selEndTxt = txt;
							if (selEndTxt.Length > 0 && mSelectionEndIndex != selEndTxt.Length)
							{
								selEndTxt = selEndTxt.Substring(0, mSelectionEndIndex);
							}
							float tLen = f.GetTextLength(renderer, selEndTxt);
							tLen = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Screen, new Vec2(tLen, 0))).X;
							var b = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Pixel, new Vec2(mPadding.Left, mPadding.Top)));
							b.X += tLen;
							mSelectionRect.Position = new ScaleValue(ScaleType.Parent, b);
						}
						txt = txt.Substring(mSelectionEndIndex, mSelectionStartIndex - mSelectionEndIndex);
					}
					else
					{
						string selStartTxt = txt;
						if (selStartTxt.Length > 0 && mSelectionStartIndex != selStartTxt.Length)
						{
							selStartTxt = selStartTxt.Substring(0, mSelectionStartIndex);
						}
						float tLen = f.GetTextLength(renderer, selStartTxt);
						tLen = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Screen, new Vec2(tLen, 0))).X;
						var b = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Pixel, new Vec2(mPadding.Left, mPadding.Top)));
						b.X += tLen;
						mSelectionRect.Position = new ScaleValue(ScaleType.Parent, b);
						txt = txt.Substring(mSelectionStartIndex, mSelectionEndIndex - mSelectionStartIndex);
					}
					float tLen2 = f.GetTextLength(renderer, txt);
					tLen2 = this.GetLocalOffsetByValue(new ScaleValue(ScaleType.Screen, new Vec2(tLen2, 0))).X;
					mSelectionRect.Size = new ScaleValue(ScaleType.Parent, new Vec2(tLen2, ySizeRelativeToParentSize));
				}
			}
		}

		protected override bool OnKeyDown(KeyEvent e)
		{
			if (!this.Focused || !this.IsEnabledInHierarchy())
			{
				return base.OnKeyDown(e);
			}
			switch (e.Key)
			{
				case EKeys.LShift:
				case EKeys.RShift:
				case EKeys.Shift:
					mShiftDown = true;
					if (!mSelecting)
						mSelectionStartIndex = mCursorIndex;
					break;

				case EKeys.Left:
					if (!mShiftDown)
						mSelecting = false;
					if (mCursorIndex > 0)
					{
						if (mShiftDown)
							mSelecting = true;
						mCursorIndex--;
					}
					break;
				case EKeys.Right:
					if (!mShiftDown)
						mSelecting = false;
					if (mCursorIndex < mInnerText.Text.Length)
					{
						if (mShiftDown)
							mSelecting = true;
						mCursorIndex++;
					}
					break;
				case EKeys.Up:
					if (mEnableMultilineInput)
					{

					}
					if (!mShiftDown)
						mSelecting = false;
					break;
				case EKeys.Down:
					if (mEnableMultilineInput)
					{

					}
					if (!mShiftDown)
						mSelecting = false;
					break;

				case EKeys.Home:
					if (!mShiftDown)
						mSelecting = false;
					else
						mSelecting = true;
					mCursorIndex = 0;
					break;
				case EKeys.End:
					if (!mShiftDown)
						mSelecting = false;
					else
						mSelecting = true;
					mCursorIndex = this.Text.Length;
					break;
				case EKeys.Back:
					if (mSelecting)
					{
						DeleteSelection();
					}
					else
					{
						if (this.Text != "" && mCursorIndex > 0)
						{
							this.Text = this.Text.Substring(0, mCursorIndex - 1) + this.Text.Substring(mCursorIndex, this.Text.Length - mCursorIndex);
							mCursorIndex--;
						}
					}
					break;
				case EKeys.Delete:
					if (mSelecting)
					{
						DeleteSelection();
					}
					else
					{
						if (this.Text != "" && mCursorIndex < this.Text.Length)
						{
							this.Text = this.Text.Substring(0, mCursorIndex) + this.Text.Substring(mCursorIndex + 1, this.Text.Length - mCursorIndex - 1);
						}
					}
					break;

				case EKeys.Enter:
					if (this.mEnableMultilineInput)
					{
						this.Text = this.Text + "\r\n";
						mCursorIndex++;
					}
					break;

				case EKeys.Escape:
					this.Unfocus();
					break;

				default:
					base.OnKeyDown(e);
					break;
			}
			return true;
		}

		private void DeleteSelection()
		{
			if (mSelectionStartIndex < mSelectionEndIndex)
			{
				this.Text = this.Text.Substring(0, mSelectionStartIndex) + this.Text.Substring(mSelectionEndIndex, this.Text.Length - mSelectionEndIndex);
				mCursorIndex -= mSelectionEndIndex - mSelectionStartIndex;
			}
			else
			{
				this.Text = this.Text.Substring(0, mSelectionEndIndex) + this.Text.Substring(mSelectionStartIndex, this.Text.Length - mSelectionStartIndex);
			}
			mSelecting = false;
		}

		protected override bool OnKeyUp(KeyEvent e)
		{
			switch (e.Key)
			{
				case EKeys.LShift:
				case EKeys.RShift:
				case EKeys.Shift:
					mShiftDown = false;
					break;
			}
			return base.OnKeyUp(e);
		}

		protected override bool OnKeyPress(KeyPressEvent e)
		{
			if (!base.Focused || !base.IsEnabledInHierarchy())
			{
				return base.OnKeyPress(e);
			}
			if (this.Text.Length < mMaxCharacters)
			{
				bool validChar = false;
				if (EngineApp.Instance.ScreenGuiRenderer != null)
				{
					validChar = (e.KeyChar >= ' ') && EngineApp.Instance.ScreenGuiRenderer.DefaultFont.IsCharacterInitialized(e.KeyChar);
				}
				else
				{
					// Non-Control character and within the basic ASCII range
					validChar = (e.KeyChar >= ' ') && (e.KeyChar < '\x0080');
				}
				if (validChar)
				{
					if (mSelecting)
					{
						DeleteSelection();
					}
					this.Text = this.Text.Substring(0, mCursorIndex) + e.KeyChar.ToString() + this.Text.Substring(mCursorIndex, this.Text.Length - mCursorIndex);
					mCursorIndex++;
				}
			}
			return true;

		}

		protected override bool OnMouseDown(EMouseButtons button)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			if (rect.IsContainsPoint(this.MousePosition) && IsEnabledInHierarchy())
			{
				Focus();
				return true;
			}
			Unfocus();
			return base.OnMouseDown(button);
		}

		protected override void OnFocused()
		{
			if (mEnableCursorBlink)
			{
				mLastCursorBlink = 0f;
				CursorVisible = true;
			}
			mEmptyText.Visible = false;
		}

		protected override void OnUnfocused()
		{
			if (EmptyText != "" && this.Text == "")
				mEmptyText.Visible = true;
			CursorVisible = false;
			mSelecting = false;
		}

		private void UpdateLabels()
		{
			float ex = 0, ey = 0, ix = 0, iy = 0;
			switch (EmptyTextHorizontalAlign)
			{
				case HorizontalAlign.Center:
					// We can't obey the padding without things looking weird.
					break;
				case HorizontalAlign.Left:
					ex = mPadding.Left;
					break;
				case HorizontalAlign.Right:
					ex = -mPadding.Right;
					break;
				default:
					throw new Exception("Unknown HorizontalAlign value for EmptyTextHorizontalAlign!");
			}
			switch (EmptyTextVerticalAlign)
			{
				case VerticalAlign.Center: break;
				case VerticalAlign.Top: ey = mPadding.Top; break;
				case VerticalAlign.Bottom: ey = -mPadding.Bottom; break;
				default:
					throw new Exception("Unknown VerticalAlign value for EmptyTextVerticalAlign!");
			}
			switch (TextHorizontalAlign)
			{
				case HorizontalAlign.Center: break;
				case HorizontalAlign.Left: ix = mPadding.Left; break;
				case HorizontalAlign.Right: ix = -mPadding.Right; break;
				default:
					throw new Exception("Unknown HorizontalAlign value for TextHorizontalAlign!");
			}
			switch (TextVerticalAlign)
			{
				case VerticalAlign.Center: break;
				case VerticalAlign.Top: iy = mPadding.Top; break;
				case VerticalAlign.Bottom: iy = -mPadding.Bottom; break;
				default:
					throw new Exception("Unknown VerticalAlign value for TextVerticalAlign!");
			}
			mInnerText.Position = new ScaleValue(ScaleType.Pixel, new Vec2(ix, iy));
			mEmptyText.Position = new ScaleValue(ScaleType.Pixel, new Vec2(ex, ey));
			mInnerText.Size = mEmptyText.Size = this.Size;
		}

		private void UpdateMask(string newText)
		{
			if (mEnableMasking)
			{
				if (realText.Length > 0)
				{
					if (newText.Length < realText.Length)
						realText = realText.Substring(0, newText.Length);
					else if (newText.Length > realText.Length)
						realText = realText + newText.Substring(realText.Length);
					else
						realText = newText;
				}
				else
				{
					realText = newText;
				}

				mInnerText.Text = new String(maskChar, realText.Length);
			}
			else
			{
				realText = newText;
				mInnerText.Text = newText;
			}
			if (realText != "" && mEmptyText.Visible)
				mEmptyText.Visible = false;

			if (realText.Length > mMaxCharacters)
			{
				realText = realText.Substring(0, mMaxCharacters);
				mInnerText.Text = realText;
			}
		}
	}
}
