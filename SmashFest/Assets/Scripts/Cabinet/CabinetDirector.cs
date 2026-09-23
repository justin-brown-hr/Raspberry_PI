using System.Collections;
using Audio;
using Gameplay;
using Scene;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cabinet
{
	// Arcade cabinet flow, layered over the original game:
	// attract mode -> any button -> 3 levels -> celebration -> attract mode.
	// Owns an always-on-top overlay canvas with the crosshair and all cabinet messages.
	public class CabinetDirector : MonoBehaviour
	{
		public enum State
		{
			Attract,
			Playing,
			Interlude,
			Celebration,
			Stopped
		}

		// Easy, short levels picked for kids (difficulty 0, few objects, 20 balls each)
		public static readonly int[] Levels = { 1, 2, 3 };

		private const float IdleTimeout = 45f;
		private const float MessageInterval = 3f;
		private const float InterludeDuration = 3f;
		private const float CelebrationDuration = 6f;

		private static readonly string[] AttractMessages =
		{
			"Mexa a <color=#FFD54A>ALAVANCA</color>\npara mover a mira",
			"Aperte o <color=#FFD54A>BOTÃO</color>\npara lançar a bola",
			"Derrube <color=#FFD54A>TUDO</color>\nda mesa!",
			"São <color=#FFD54A>3 FASES</color>\ndivertidas!",
			"Mire bem e\n<color=#FFD54A>DERRUBE TUDO!</color>"
		};

		private static readonly string[] WinCheers = { "MUITO BEM!", "INCRÍVEL!", "SENSACIONAL!" };

		private static CabinetDirector _instance;

		private State _state = State.Attract;
		private int _levelSlot;
		private float _lastInputTime;
		private bool _levelFinishing;

		private TMP_FontAsset _font;
		private RectTransform _crosshair;
		private GameObject _attractPanel;
		private TextMeshProUGUI _attractMessage;
		private TextMeshProUGUI _attractPrompt;
		private GameObject _bannerPanel;
		private TextMeshProUGUI _bannerTitle;
		private TextMeshProUGUI _bannerSubtitle;
		private GameObject _hud;
		private TextMeshProUGUI _hudLevel;

		public static bool Enabled => true;

		public static bool IsPlaying => _instance != null && _instance._state == State.Playing;

		public static int CurrentLevelIndex => Levels[_instance != null ? _instance._levelSlot : 0];

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Create()
		{
			if (_instance != null)
			{
				return;
			}
			GameObject go = new GameObject("CabinetDirector");
			DontDestroyOnLoad(go);
			_instance = go.AddComponent<CabinetDirector>();
		}

		private void Awake()
		{
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Cursor.visible = false;
			BuildUI();
			EnterAttract(false);
		}

		private void Update()
		{
			CabinetInput.Tick();
			if (CabinetInput.AnyInput)
			{
				_lastInputTime = Time.unscaledTime;
			}

			switch (_state)
			{
				case State.Attract:
					if (CabinetInput.FireDown && !SceneHandler.IsInLoading)
					{
						StartGame();
					}
					break;
				case State.Playing:
					if (Time.unscaledTime - _lastInputTime > IdleTimeout && !_levelFinishing)
					{
						EnterAttract(true);
					}
					break;
			}

			UpdateCrosshair();
		}

		// ---- Called by GameController ----

		public static void OnLevelWon()
		{
			if (_instance != null)
			{
				_instance.FinishLevel(true);
			}
		}

		public static void OnLevelFailed()
		{
			if (_instance != null)
			{
				_instance.FinishLevel(false);
			}
		}

		// Hard stop from outside (e.g. the claw machine became busy). Shows the message until Resume().
		public static void StopGame(string message)
		{
			if (_instance == null)
			{
				return;
			}
			_instance.StopAllCoroutines();
			_instance._state = State.Stopped;
			_instance._levelFinishing = false;
			_instance.ShowBanner(message, string.Empty);
			_instance._attractPanel.SetActive(false);
			_instance._hud.SetActive(false);
			SetGameHud(false);
		}

		public static void Resume()
		{
			if (_instance != null && _instance._state == State.Stopped)
			{
				_instance.EnterAttract(true);
			}
		}

		// ---- Flow ----

		private void EnterAttract(bool reloadLevel)
		{
			StopAllCoroutines();
			_state = State.Attract;
			_levelSlot = 0;
			_levelFinishing = false;
			_bannerPanel.SetActive(false);
			_hud.SetActive(false);
			_attractPanel.SetActive(true);
			SetGameHud(false);
			if (reloadLevel)
			{
				SceneHandler.LoadScene(SceneType.Gameplay, true);
			}
			StartCoroutine(AttractRoutine());
		}

		private void StartGame()
		{
			StopAllCoroutines();
			CabinetInput.ConsumeFire();
			PlaySfx(Audio.AudioType.CoinAppear);
			_attractPanel.SetActive(false);
			_levelSlot = 0;
			_lastInputTime = Time.unscaledTime;
			// The attract background is already level 1, untouched: start playing it right away
			BeginLevel(false);
		}

		private void BeginLevel(bool reload)
		{
			_state = State.Playing;
			_levelFinishing = false;
			_bannerPanel.SetActive(false);
			_hud.SetActive(true);
			_hudLevel.text = string.Format("FASE {0}/{1}", _levelSlot + 1, Levels.Length);
			SetGameHud(true);
			CabinetInput.ResetCrosshair();
			if (reload)
			{
				SceneHandler.LoadScene(SceneType.Gameplay, true);
			}
		}

		private void FinishLevel(bool won)
		{
			if (_state != State.Playing || _levelFinishing)
			{
				return;
			}
			_levelFinishing = true;
			StartCoroutine(FinishLevelRoutine(won));
		}

		private IEnumerator FinishLevelRoutine(bool won)
		{
			_state = State.Interlude;
			_hud.SetActive(false);
			SetGameHud(false);
			PlaySfx(Audio.AudioType.Win);

			bool last = _levelSlot >= Levels.Length - 1;
			if (last)
			{
				_state = State.Celebration;
				ShowBanner("PARABÉNS!", "Você completou\nas 3 fases!");
				yield return new WaitForSecondsRealtime(CelebrationDuration);
				EnterAttract(true);
				yield break;
			}

			string title = won ? WinCheers[_levelSlot % WinCheers.Length] : "BOA TENTATIVA!";
			ShowBanner(title, string.Format("Fase {0} concluída!\nPreparando a fase {1}...", _levelSlot + 1, _levelSlot + 2));
			yield return new WaitForSecondsRealtime(InterludeDuration);
			_levelSlot++;
			BeginLevel(true);
		}

		private IEnumerator AttractRoutine()
		{
			int index = 0;
			while (true)
			{
				_attractMessage.text = AttractMessages[index % AttractMessages.Length];
				index++;
				float t = 0f;
				while (t < MessageInterval)
				{
					t += Time.unscaledDeltaTime;
					float pop = t < 0.25f ? Mathf.SmoothStep(0.6f, 1f, t / 0.25f) : 1f;
					_attractMessage.rectTransform.localScale = Vector3.one * pop;
					float pulse = 1f + 0.06f * Mathf.Sin(Time.unscaledTime * 5f);
					_attractPrompt.rectTransform.localScale = Vector3.one * pulse;
					yield return null;
				}
			}
		}

		// ---- UI ----

		private void ShowBanner(string title, string subtitle)
		{
			_bannerTitle.text = title;
			_bannerSubtitle.text = subtitle;
			_bannerPanel.SetActive(true);
			StartCoroutine(PopRoutine(_bannerTitle.rectTransform));
		}

		private static IEnumerator PopRoutine(RectTransform target)
		{
			float t = 0f;
			while (t < 0.35f)
			{
				t += Time.unscaledDeltaTime;
				float k = t / 0.35f;
				// overshoot pop: 0.3 -> 1.15 -> 1
				float s = k < 0.7f ? Mathf.Lerp(0.3f, 1.15f, k / 0.7f) : Mathf.Lerp(1.15f, 1f, (k - 0.7f) / 0.3f);
				target.localScale = Vector3.one * s;
				yield return null;
			}
			target.localScale = Vector3.one;
		}

		private void UpdateCrosshair()
		{
			bool visible = _state == State.Playing && !SceneHandler.IsInLoading;
			_crosshair.gameObject.SetActive(visible);
			if (!visible)
			{
				return;
			}
			Vector2 c = CabinetInput.Crosshair;
			_crosshair.anchorMin = c;
			_crosshair.anchorMax = c;
			_crosshair.anchoredPosition = Vector2.zero;
			float pulse = 1f + 0.08f * Mathf.Sin(Time.unscaledTime * 6f);
			_crosshair.localScale = Vector3.one * pulse;
			_crosshair.localRotation = Quaternion.Euler(0f, 0f, Time.unscaledTime * 30f);
		}

		// The game's own HUD (ball counter) belongs to gameplay only
		private static void SetGameHud(bool visible)
		{
			ServiceLocator.Get<GameController>()?.SetCabinetHudVisible(visible);
		}

		private static void PlaySfx(Audio.AudioType type)
		{
			ServiceLocator.Get<AudioHelper>()?.PlaySfx(type);
		}

		private void BuildUI()
		{
			Font source = Resources.Load<Font>("Cabinet/Fredoka-Bold");
			_font = source != null ? TMP_FontAsset.CreateFontAsset(source) : TMP_Settings.defaultFontAsset;

			GameObject canvasGo = new GameObject("CabinetCanvas", typeof(Canvas), typeof(CanvasScaler));
			canvasGo.transform.SetParent(transform, false);
			Canvas canvas = canvasGo.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 5000;
			CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1080f, 1920f);
			scaler.matchWidthOrHeight = 0.5f;
			RectTransform root = (RectTransform)canvasGo.transform;

			// Crosshair
			Image cross = CreateImage("Crosshair", root, MakeCrosshairTexture(256), Color.white);
			cross.raycastTarget = false;
			_crosshair = cross.rectTransform;
			_crosshair.sizeDelta = new Vector2(190f, 190f);

			// HUD: level indicator
			_hud = new GameObject("CabinetHud", typeof(RectTransform));
			_hud.transform.SetParent(root, false);
			Stretch((RectTransform)_hud.transform);
			_hudLevel = CreateText("Level", (RectTransform)_hud.transform, 64f, new Color(1f, 0.84f, 0.29f));
			Anchor(_hudLevel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(700f, 100f));

			// Attract mode
			_attractPanel = CreatePanel("Attract", root, new Color(0.05f, 0.08f, 0.25f, 0.72f));
			RectTransform attract = (RectTransform)_attractPanel.transform;
			TextMeshProUGUI title = CreateText("Title", attract, 150f, new Color(1f, 0.55f, 0.1f));
			title.text = "SMASH\nFEST";
			title.lineSpacing = -20f;
			title.outlineWidth = 0.25f;
			title.outlineColor = new Color32(90, 20, 0, 255);
			Anchor(title.rectTransform, new Vector2(0.5f, 0.82f), Vector2.zero, new Vector2(1000f, 420f));
			_attractMessage = CreateText("Message", attract, 84f, Color.white);
			Anchor(_attractMessage.rectTransform, new Vector2(0.5f, 0.63f), Vector2.zero, new Vector2(1000f, 300f));
			_attractPrompt = CreateText("Prompt", attract, 72f, new Color(0.3f, 1f, 0.4f));
			_attractPrompt.text = "APERTE O BOTÃO\nPARA JOGAR!";
			Anchor(_attractPrompt.rectTransform, new Vector2(0.5f, 0.17f), Vector2.zero, new Vector2(1000f, 240f));

			// Interlude / celebration / stop banner
			_bannerPanel = CreatePanel("Banner", root, new Color(0.05f, 0.08f, 0.25f, 0.78f));
			RectTransform banner = (RectTransform)_bannerPanel.transform;
			_bannerTitle = CreateText("Title", banner, 120f, new Color(1f, 0.84f, 0.29f));
			_bannerTitle.outlineWidth = 0.2f;
			_bannerTitle.outlineColor = new Color32(90, 40, 0, 255);
			Anchor(_bannerTitle.rectTransform, new Vector2(0.5f, 0.58f), Vector2.zero, new Vector2(1040f, 300f));
			_bannerSubtitle = CreateText("Subtitle", banner, 68f, Color.white);
			Anchor(_bannerSubtitle.rectTransform, new Vector2(0.5f, 0.42f), Vector2.zero, new Vector2(1000f, 300f));
			_bannerPanel.SetActive(false);
		}

		private TextMeshProUGUI CreateText(string name, RectTransform parent, float size, Color color)
		{
			GameObject go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(parent, false);
			TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
			text.font = _font;
			text.fontSize = size;
			text.color = color;
			text.alignment = TextAlignmentOptions.Center;
			text.enableWordWrapping = true;
			text.raycastTarget = false;
			return text;
		}

		private static GameObject CreatePanel(string name, RectTransform parent, Color color)
		{
			Image image = CreateImage(name, parent, null, color);
			image.raycastTarget = false;
			Stretch(image.rectTransform);
			return image.gameObject;
		}

		private static Image CreateImage(string name, RectTransform parent, Texture2D texture, Color color)
		{
			GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
			go.transform.SetParent(parent, false);
			Image image = go.GetComponent<Image>();
			if (texture != null)
			{
				image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			}
			image.color = color;
			return image;
		}

		private static void Stretch(RectTransform rt)
		{
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
		}

		private static void Anchor(RectTransform rt, Vector2 anchor, Vector2 position, Vector2 size)
		{
			rt.anchorMin = anchor;
			rt.anchorMax = anchor;
			rt.anchoredPosition = position;
			rt.sizeDelta = size;
		}

		// Red/white target reticle: ring + four ticks + centre dot, with a white outline for contrast
		private static Texture2D MakeCrosshairTexture(int size)
		{
			Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
			tex.wrapMode = TextureWrapMode.Clamp;
			Color red = new Color(0.95f, 0.1f, 0.15f, 1f);
			Color white = Color.white;
			Color clear = new Color(1f, 1f, 1f, 0f);
			float c = (size - 1) * 0.5f;
			float ringR = size * 0.33f;
			float ringW = size * 0.045f;
			float outline = size * 0.02f;
			float tickW = size * 0.035f;
			float dotR = size * 0.06f;
			Color[] pixels = new Color[size * size];
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float dx = x - c;
					float dy = y - c;
					float r = Mathf.Sqrt(dx * dx + dy * dy);
					bool onTick = (Mathf.Abs(dx) < tickW && Mathf.Abs(dy) > ringR * 0.55f && Mathf.Abs(dy) < size * 0.48f)
						|| (Mathf.Abs(dy) < tickW && Mathf.Abs(dx) > ringR * 0.55f && Mathf.Abs(dx) < size * 0.48f);
					bool nearTick = (Mathf.Abs(dx) < tickW + outline && Mathf.Abs(dy) > ringR * 0.55f - outline && Mathf.Abs(dy) < size * 0.48f + outline)
						|| (Mathf.Abs(dy) < tickW + outline && Mathf.Abs(dx) > ringR * 0.55f - outline && Mathf.Abs(dx) < size * 0.48f + outline);
					float ringDist = Mathf.Abs(r - ringR);
					Color col = clear;
					if (ringDist < ringW + outline || nearTick || r < dotR + outline)
					{
						col = white;
					}
					if (ringDist < ringW || onTick || r < dotR)
					{
						col = red;
					}
					pixels[y * size + x] = col;
				}
			}
			tex.SetPixels(pixels);
			tex.Apply();
			return tex;
		}
	}
}
