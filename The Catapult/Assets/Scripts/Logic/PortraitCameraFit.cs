using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic
{
	/// <summary>
	/// Refits orthographic cameras for the portrait arcade cabinet.
	///
	/// Unity's orthographic size fixes the *vertical* half-height, so the horizontal
	/// view scales with aspect ratio. The game was authored landscape (size 20 at
	/// ~1.84 aspect = 73.5 world units across). On a 9:16 cabinet screen that same
	/// size shows only 22.5 units across, which clips the enemy tower off the left
	/// edge.
	///
	/// This keys the camera off a target *width* instead, so the playfield stays
	/// horizontally intact whatever the screen shape. It attaches itself at runtime,
	/// so no scene files need editing.
	/// </summary>
	public class PortraitCameraFit : MonoBehaviour
	{
		/// <summary>
		/// World units the camera should show horizontally. The landscape build shows
		/// ~73.5, but the action only spans roughly the middle 45, so a smaller target
		/// keeps sprites reasonably large while still fitting both catapults in.
		/// </summary>
		/// How much wider to make the view in portrait, as a multiple of whatever
		/// orthographic size the scene author chose.
		///
		/// This must be RELATIVE, not an absolute world width: the scenes use different
		/// base sizes (GameScene 20, MainScene 10 and 8). Forcing one absolute width
		/// zoomed the menu camera out ~3x, pushing its content off-screen.
		///
		/// Measured on device at 608x1080: at 1.5x the playfield occupied only the left
		/// 480 of 608 pixels, leaving ~7 world units of dead space on the right. 1.24x
		/// narrows the view to the content, which also spreads the tower and the player
		/// further apart on screen.
		public const float PORTRAIT_ZOOM_OUT = 1.24f;

		/// The zoom level at which the enemy tower sat exactly on the left edge. Used to
		/// keep that edge fixed while narrowing, so the trim comes off the right only.
		private const float EDGE_REFERENCE_ZOOM = 1.5f;

		/// <summary>Guard against a pathological aspect producing an absurd zoom.</summary>
		private const float MAX_SIZE = 60f;

		private Camera _camera;
		private float _lastAspect;
		private float _authoredSize = -1f;
		private float _authoredX = float.NaN;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			Apply();
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Apply();
		}

		/// <summary>
		/// Only the gameplay scenes need refitting. The menu was framed correctly in
		/// portrait already, and zooming it out pushed its content off-screen and
		/// exposed the edges of the background art.
		/// </summary>
		private static bool IsGameplayScene()
		{
			string name = SceneManager.GetActiveScene().name;
			return name == "GameScene" || name == "PvPScene";
		}

		private static void Apply()
		{
			if (!IsGameplayScene())
			{
				return;
			}

			Camera[] cameras = Camera.allCameras;
			for (int i = 0; i < cameras.Length; i++)
			{
				Camera cam = cameras[i];
				if (cam == null || !cam.orthographic)
				{
					continue;
				}
				if (cam.GetComponent<PortraitCameraFit>() == null)
				{
					cam.gameObject.AddComponent<PortraitCameraFit>();
				}
			}
		}

		private void Awake()
		{
			_camera = GetComponent<Camera>();
			// Capture what the scene author set, before we touch it.
			if (_camera != null && _authoredSize < 0f)
			{
				_authoredSize = _camera.orthographicSize;
				_authoredX = _camera.transform.position.x;
			}
			Refit();
		}

		private void Update()
		{
			// Aspect can change when the cabinet display or Waydroid resolution changes.
			if (_camera != null && !Mathf.Approximately(_camera.aspect, _lastAspect))
			{
				Refit();
			}
		}

		private void Refit()
		{
			if (_camera == null || !_camera.orthographic)
			{
				return;
			}

			_lastAspect = _camera.aspect;

			// Landscape already frames correctly - leave it exactly as authored.
			if (_lastAspect >= 1f)
			{
				return;
			}

			_camera.orthographicSize = Mathf.Clamp(
				_authoredSize * PORTRAIT_ZOOM_OUT, _authoredSize, MAX_SIZE);

			// Narrowing the view trims equally from both sides, which would clip the
			// tower. Shift left by half the removed width so the left edge stays put
			// and the whole trim comes off the empty right-hand side.
			if (!float.IsNaN(_authoredX))
			{
				float shift = _authoredSize * _lastAspect * (EDGE_REFERENCE_ZOOM - PORTRAIT_ZOOM_OUT);
				Vector3 pos = _camera.transform.position;
				pos.x = _authoredX - shift;
				_camera.transform.position = pos;
			}

		}
	}
}
