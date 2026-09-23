using Logic;
using Promo;
using UnityEngine;

namespace View
{
	public class GUIControl_PlayModeSelect : MonoBehaviour
	{
		public PlayModeSceneControl _sceneControl;

		public LoadingScreen _loadingScreen;

		public void StandartGameStarted()
		{
			SoundMgr.instance.ButtonPress();
			InitController.instance.ResetPoint();
			NewDataController.instance.SetGameMode(GameMode.Single);
			_loadingScreen.LoadScene("GameScene", showAds: true);
			Time.timeScale = 1f;
		}

		public void EnterShop_Clicked()
		{
			SoundMgr.instance.ButtonPress();
			_sceneControl.EnterShop();
			if (PromoModule.instance != null)
			{
				PromoModule.instance.Activate_PromoCanvas(stat: false);
			}
		}

		public void ExitShop_Clicked()
		{
			SoundMgr.instance.ButtonPress();
			_sceneControl.ExitShop();
			if (PromoModule.instance != null)
			{
				PromoModule.instance.Activate_PromoCanvas(stat: true);
			}
		}

		public void ReturnButton_Clicked()
		{
			SoundMgr.instance.ButtonPress();
			_loadingScreen.LoadScene("MainScene", showAds: true);
		}

		public void PvPGameStarted()
		{
			SoundMgr.instance.ButtonPress();
			NewDataController.instance.SetGameMode(GameMode.PvPOneScreen);
			_loadingScreen.LoadScene("PvPScene", showAds: true);
			Time.timeScale = 1f;
		}

		public void MouseDown()
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (UnityEngine.Input.GetTouch(i).phase == TouchPhase.Began)
				{
					InitController.instance.guiPressedFingerId = UnityEngine.Input.GetTouch(i).fingerId;
					break;
				}
			}
			InitController.instance.guiPressed = true;
		}

		public void MouseUp()
		{
			InitController.instance.guiPressedFingerId = -1f;
			InitController.instance.guiPressed = false;
		}
	}
}
