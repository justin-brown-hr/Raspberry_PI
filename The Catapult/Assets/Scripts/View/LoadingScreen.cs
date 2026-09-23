using Logic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class LoadingScreen : MonoBehaviour
	{
		public delegate void StartLoading(AsyncOperation loading);

		public Image background;

		public Text loadingText;

		public GraphicRaycaster _menuRaycaster;

		private string[] loadingTexts;

		private bool sceneLoaded = true;

		private int pulseState = 1;

		public void LoadScene(string sceneName, bool showAds = false)
		{
			_menuRaycaster.enabled = false;
			InitController.instance.guiPressed = true;
			background.gameObject.SetActive(value: true);
			StartCoroutine(DarknessScreen());
			StartLoading load = StartSceneLoading;
			BYV_ScenesLoader.Instance.Load_Scene(sceneName, showAds, load);
		}

		private IEnumerator DarknessScreen()
		{
			sceneLoaded = false;
			while (true)
			{
				Color color = background.color;
				if (!(color.a < 0.55f))
				{
					break;
				}
				Image image = background;
				Color color2 = background.color;
				image.color = new Color(0f, 0f, 0f, color2.a + Time.deltaTime * 3f);
				yield return null;
			}
			StartCoroutine(StatusPulsing());
		}

		private IEnumerator StatusPulsing()
		{
			while (!sceneLoaded)
			{
				Color color = loadingText.color;
				if (color.a > 0.05f && pulseState == 0)
				{
					Text text = loadingText;
					Color color2 = loadingText.color;
					float r = color2.r;
					Color color3 = loadingText.color;
					float g = color3.g;
					Color color4 = loadingText.color;
					float b = color4.b;
					Color color5 = loadingText.color;
					text.color = new Color(r, g, b, color5.a - Time.deltaTime * 3f);
				}
				else
				{
					pulseState = 1;
				}
				Color color6 = loadingText.color;
				if (color6.a < 0.75f && pulseState == 1)
				{
					Text text2 = loadingText;
					Color color7 = loadingText.color;
					float r2 = color7.r;
					Color color8 = loadingText.color;
					float g2 = color8.g;
					Color color9 = loadingText.color;
					float b2 = color9.b;
					Color color10 = loadingText.color;
					text2.color = new Color(r2, g2, b2, color10.a + Time.deltaTime * 3f);
				}
				else
				{
					pulseState = 0;
				}
				yield return null;
			}
		}

		private void StartSceneLoading(AsyncOperation loading)
		{
			StartCoroutine(WaitForSceneLoad(loading));
		}

		private IEnumerator WaitForSceneLoad(AsyncOperation loading)
		{
			yield return new WaitForSeconds(1f);
			while (!loading.isDone)
			{
				yield return null;
			}
			sceneLoaded = true;
		}
	}
}
