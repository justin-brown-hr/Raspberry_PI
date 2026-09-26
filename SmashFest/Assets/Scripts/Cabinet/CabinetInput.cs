using UnityEngine;

namespace Cabinet
{
	// Arcade cabinet input: a joystick-driven crosshair and one fire button.
	// Keyboard (arrows + space) and mouse work too, for bench testing over VNC.
	public static class CabinetInput
	{
		private const float Deadzone = 0.25f;
		private const float BaseSpeed = 0.35f;
		private const float MaxSpeed = 0.9f;
		private const float AccelerationTime = 0.6f;

		// Crosshair area in viewport space; keeps it over the play field, below the top HUD
		public static readonly Rect Bounds = new Rect(0.08f, 0.25f, 0.84f, 0.6f);

		private static readonly string[] HorizontalAxes = { "Horizontal", "Debug Horizontal" };
		private static readonly string[] VerticalAxes = { "Vertical", "Debug Vertical" };

		// Client: the crosshair must START OUTSIDE the stack, so the player has to aim.
		// Low and to the left, clear of the table.
		public static readonly Vector2 StartPosition = new Vector2(0.14f, 0.28f);

		private static Vector2 _crosshair = StartPosition;
		private static float _heldTime;
		private static Vector3 _lastMousePosition;
		private static int _updatedFrame = -1;

		public static Vector2 Crosshair => _crosshair;

		public static bool FireDown { get; private set; }

		public static bool AnyInput { get; private set; }

		// Test driver hook (-cabinetTest): overrides the stick and presses fire
		private static Vector2? _testTarget;
		private static bool _testFire;
		private static bool _testStart;

		public static void SetTestInput(Vector2? target, bool fire)
		{
			_testTarget = target;
			_testFire |= fire;
		}

		public static void SetTestStart()
		{
			_testStart = true;
		}

		// Swallow this frame's press so the button that starts a game doesn't also fire
		public static void ConsumeFire()
		{
			FireDown = false;
		}

		// The START button is a different physical button from FIRE. The wristband
		// Raspberry closes a relay across START for 1 second, so a credit arrives here
		// as an ordinary button press.
		public static bool StartDown { get; private set; }

		// Last button seen, for the on-screen button test (-cabinetButtons)
		public static string LastButton { get; private set; } = "-";

		private static readonly KeyCode[] FireKeys =
		{
			KeyCode.JoystickButton0, KeyCode.Space, KeyCode.LeftControl, KeyCode.Z
		};

		private static readonly KeyCode[] StartKeys =
		{
			KeyCode.JoystickButton1, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Alpha1
		};

		public static void ResetCrosshair()
		{
			_crosshair = StartPosition;
			_heldTime = 0f;
		}

		public static void Tick()
		{
			if (_updatedFrame == Time.frameCount)
			{
				return;
			}
			_updatedFrame = Time.frameCount;

			Vector2 stick = new Vector2(ReadAxis(HorizontalAxes), ReadAxis(VerticalAxes));
			if (stick.magnitude < Deadzone)
			{
				stick = Vector2.zero;
				_heldTime = 0f;
			}
			else
			{
				stick = Vector2.ClampMagnitude(stick, 1f);
				_heldTime += Time.unscaledDeltaTime;
			}

			float speed = Mathf.Lerp(BaseSpeed, MaxSpeed, Mathf.Clamp01(_heldTime / AccelerationTime));
			_crosshair += stick * speed * Time.unscaledDeltaTime;

			bool mouseMoved = false;
			Vector3 mousePosition = Input.mousePosition;
			if ((mousePosition - _lastMousePosition).sqrMagnitude > 1f && Screen.width > 0 && Screen.height > 0)
			{
				_crosshair = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
				mouseMoved = true;
			}
			_lastMousePosition = mousePosition;

			bool testInput = false;
			if (_testTarget.HasValue)
			{
				_crosshair = Vector2.MoveTowards(_crosshair, _testTarget.Value, MaxSpeed * Time.unscaledDeltaTime);
				testInput = true;
			}

			_crosshair.x = Mathf.Clamp(_crosshair.x, Bounds.xMin, Bounds.xMax);
			_crosshair.y = Mathf.Clamp(_crosshair.y, Bounds.yMin, Bounds.yMax);

			FireDown = ReadKeys(FireKeys) || Input.GetMouseButtonDown(0) || _testFire;
			StartDown = ReadKeys(StartKeys) || _testStart;
			_testStart = false;
			_testFire = false;
			RecordLastButton();
			AnyInput = FireDown || StartDown || stick != Vector2.zero || mouseMoved || testInput;
		}

		private static float ReadAxis(string[] axes)
		{
			float value = 0f;
			for (int i = 0; i < axes.Length; i++)
			{
				float axis = Input.GetAxisRaw(axes[i]);
				if (Mathf.Abs(axis) > Mathf.Abs(value))
				{
					value = axis;
				}
			}
			return value;
		}

		private static bool ReadKeys(KeyCode[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (Input.GetKeyDown(keys[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Names whatever was pressed, so someone at the cabinet can press each button
		// and we can see which code it sends.
		private static void RecordLastButton()
		{
			for (KeyCode key = KeyCode.JoystickButton0; key <= KeyCode.JoystickButton19; key++)
			{
				if (Input.GetKeyDown(key))
				{
					LastButton = key.ToString();
					return;
				}
			}
			if (Input.anyKeyDown)
			{
				foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(key))
					{
						LastButton = key.ToString();
						return;
					}
				}
			}
		}

	}
}
