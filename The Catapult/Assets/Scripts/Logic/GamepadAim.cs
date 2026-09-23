using UnityEngine;

namespace Logic
{
	/// <summary>
	/// Synthesises the touch input the catapult aiming code already expects, driven
	/// from a gamepad instead of a finger.
	///
	/// The shot is defined by a single scalar: the drag length, which GameMath maps to
	/// power as d = 5 + (aimDistance - 1) * 1.82, clamped to [5, 25]. Direction is fixed
	/// per catapult. So a pad only needs one analog value plus a button, and this class
	/// converts that into the screen-space positions the existing code reads.
	///
	/// Controls: hold the fire button to draw back (power ramps up automatically), push
	/// the stick to override the power manually, release to launch.
	/// </summary>
	public static class GamepadAim
	{
		// Matches GameMath.MIN_DISTANCE / MAX_DISTANCE.
		private const float MIN_PULL = 1f;
		private const float MAX_PULL = 12f;

		// Seconds of holding the button to charge from minimum to maximum power.
		private const float CHARGE_TIME = 1.5f;

		// Stick deflection below this is treated as noise rather than intent.
		private const float STICK_DEADZONE = 0.2f;

		// Direction the synthetic drag runs in, in world space. Only its length is
		// used for power, but it must be consistent so the drag reads as one gesture.
		private static readonly Vector2 PULL_DIRECTION = new Vector2(-0.6f, -0.8f);

		private static float _charge;
		private static bool _wasHeld;

		/// <summary>True when at least one gamepad is connected.</summary>
		public static bool Connected
		{
			get
			{
				string[] names = Input.GetJoystickNames();
				for (int i = 0; i < names.Length; i++)
				{
					if (!string.IsNullOrEmpty(names[i]))
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>Current pull strength, 0-1. Useful for driving an on-screen power bar.</summary>
		public static float ChargeNormalised
		{
			get { return Mathf.InverseLerp(MIN_PULL, MAX_PULL, _charge); }
		}

		/// <summary>
		/// Any joystick button fires. Arcade encoders number their buttons
		/// unpredictably - the DragonRise stick on the cabinet reports generic
		/// BTN_TRIGGER..BTN_BASE6 codes, so listening for specific buttons missed it.
		/// </summary>
		private static bool FireHeld
		{
			get
			{
				return ArcadeInput.AnyButtonHeld()
					|| Input.GetAxisRaw("Fire1") > 0.5f;
			}
		}

		/// <summary>
		/// Produces the synthetic touch for this frame. Returns false when the pad is
		/// idle, so callers can fall through to their existing touch or mouse path.
		/// </summary>
		/// <param name="origin">The catapult the drag starts from.</param>
		/// <param name="cam">Camera used to convert world positions back to screen space.</param>
		/// <param name="touch">Synthetic touch carrying phase and screen position.</param>
		public static bool TryGetTouch(Transform origin, Camera cam, out Touch touch)
		{
			touch = default(Touch);
			if (origin == null || cam == null)
			{
				return false;
			}

			bool held = FireHeld;
			if (!held && !_wasHeld)
			{
				_charge = MIN_PULL;
				return false;
			}

			TouchPhase phase;
			if (held && !_wasHeld)
			{
				// Button just pressed: the gesture starts at the catapult itself, so the
				// aiming code records startPos there.
				_charge = MIN_PULL;
				phase = TouchPhase.Began;
			}
			else if (held)
			{
				// Stick overrides the auto-charge when the player pushes it, otherwise
				// power ramps on its own so a pad with a dead stick still works.
				float stick = -Input.GetAxis("Vertical");
				if (Mathf.Abs(stick) > STICK_DEADZONE)
				{
					_charge = Mathf.Lerp(MIN_PULL, MAX_PULL, Mathf.Clamp01(Mathf.Abs(stick)));
				}
				else
				{
					_charge = Mathf.Min(MAX_PULL, _charge + (MAX_PULL - MIN_PULL) * Time.deltaTime / CHARGE_TIME);
				}
				phase = TouchPhase.Moved;
			}
			else
			{
				// Button released this frame: fire at whatever power was reached.
				phase = TouchPhase.Ended;
			}

			_wasHeld = held;

			// On Began the drag has no length yet, so the touch sits exactly on the
			// catapult. Afterwards it sits _charge world-units away, which is precisely
			// the value Vector2.Distance(startPos, endPos) will recover.
			Vector2 world = (Vector2)origin.position;
			if (phase != TouchPhase.Began)
			{
				world += PULL_DIRECTION.normalized * _charge;
			}

			touch.position = cam.WorldToScreenPoint(new Vector3(world.x, world.y, 0f));
			touch.phase = phase;
			return true;
		}

		/// <summary>Clears state, e.g. when a round ends mid-draw.</summary>
		public static void Reset()
		{
			_charge = MIN_PULL;
			_wasHeld = false;
		}
	}
}
