using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Logic
{
	/// <summary>
	/// Drives the single-player flow from an arcade joystick, so the cabinet needs
	/// no mouse or touchscreen.
	///
	/// The game's menus are touch-first: Unity only navigates them from a joystick
	/// when a button is pre-selected, and none is. So rather than generic menu
	/// navigation, each screen maps "any button" to its one sensible action:
	///
	///   MainScene       -> Play               (the "ButtonPlay" object)
	///   PlayModeSelect  -> 1 Player           (StandartGameStarted)
	///   GameScene, over -> restart the round  (GameMenuControl.ReloadGame)
	///   GameScene, live -> fire               (handled by GamepadAim)
	///
	/// Any button counts: arcade encoders number their buttons unpredictably, and
	/// this one (DragonRise) reports generic BTN_TRIGGER..BTN_BASE6 codes rather
	/// than gamepad codes. Also logs every press so the wiring can be read from
	/// logcat without being at the cabinet.
	/// </summary>
	public class ArcadeInput : MonoBehaviour
	{
		/// <summary>Unity exposes JoystickButton0..19.</summary>
		private const int BUTTON_COUNT = 20;

		/// <summary>
		/// After a menu action, ignore input briefly. Otherwise one press on the main
		/// menu would carry through the next screen's load and skip straight past it.
		/// </summary>
		private const float ACTION_COOLDOWN = 1.5f;

		private float _cooldownUntil;
		private string _lastJoystickNames = "";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			if (FindObjectOfType<ArcadeInput>() != null)
			{
				return;
			}
			GameObject host = new GameObject("ArcadeInput");
			host.AddComponent<ArcadeInput>();
			DontDestroyOnLoad(host);
		}

		/// <summary>True on the frame any joystick button goes down.</summary>
		public static bool AnyButtonDown(out int button)
		{
			for (int i = 0; i < BUTTON_COUNT; i++)
			{
				if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
				{
					button = i;
					return true;
				}
			}
			button = -1;
			return false;
		}

		/// <summary>True while any joystick button is held.</summary>
		public static bool AnyButtonHeld()
		{
			for (int i = 0; i < BUTTON_COUNT; i++)
			{
				if (Input.GetKey(KeyCode.JoystickButton0 + i))
				{
					return true;
				}
			}
			return false;
		}

		private void Update()
		{
			LogJoystickChanges();

			int button;
			if (!AnyButtonDown(out button))
			{
				return;
			}

			string scene = SceneManager.GetActiveScene().name;
			Debug.Log("ARCADE: button " + button + " down in " + scene);

			if (Time.unscaledTime < _cooldownUntil)
			{
				return;
			}

			if (scene == "MainScene")
			{
				if (ClickNamed("ButtonPlay"))
				{
					Cooldown("Play");
				}
			}
			else if (scene == "PlayModeSelect")
			{
				View.GUIControl_PlayModeSelect select = FindObjectOfType<View.GUIControl_PlayModeSelect>();
				if (select != null)
				{
					select.StandartGameStarted();
					Cooldown("1 Player");
				}
			}
			else if (scene == "GameScene")
			{
				GameMenuControl menu = FindObjectOfType<GameMenuControl>();
				if (menu != null && menu.gameOverPanel != null && menu.gameOverPanel.activeSelf)
				{
					menu.ReloadGame(true);
					Cooldown("restart round");
				}
				// Otherwise a round is live and GamepadAim turns the press into a shot.
			}
		}

		private void Cooldown(string action)
		{
			_cooldownUntil = Time.unscaledTime + ACTION_COOLDOWN;
			Debug.Log("ARCADE: action -> " + action);
		}

		private static bool ClickNamed(string objectName)
		{
			GameObject target = GameObject.Find(objectName);
			Button button = target != null ? target.GetComponent<Button>() : null;
			if (button == null || !button.interactable)
			{
				Debug.Log("ARCADE: no clickable '" + objectName + "' found");
				return false;
			}
			button.onClick.Invoke();
			return true;
		}

		/// <summary>Logs when a controller is plugged in or removed.</summary>
		private void LogJoystickChanges()
		{
			string names = string.Join(" | ", Input.GetJoystickNames());
			if (names != _lastJoystickNames)
			{
				_lastJoystickNames = names;
				Debug.Log("ARCADE: joysticks = [" + names + "]");
			}
		}
	}
}
