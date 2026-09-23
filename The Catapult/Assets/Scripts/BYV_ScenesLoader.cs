using Assets.All_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using View;

public class BYV_ScenesLoader : MonoBehaviour
{
	public static BYV_ScenesLoader Instance;

	private string Priv_NameScene = string.Empty;

	private LoadingScreen.StartLoading loadingDelegate;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Load_Scene(string scene_name, bool statusShowInter = true, LoadingScreen.StartLoading load = null)
	{
		Status_Scene.Inst.scene_forLoad = scene_name;
		loadingDelegate = load;
		UnityEngine.Debug.Log(" >>>>>>>---------- РАБОТАЕТ  Load_Scene  --------<<<<<<<<< ");
		if (!statusShowInter)
		{
			Forced_LoadScene(Status_Scene.Inst.scene_forLoad);
		}
		else if (scene_name != null)
		{
			if (!(scene_name == "MainScene"))
			{
				if (!(scene_name == "PlayModeSelect"))
				{
					if (!(scene_name == "GameScene"))
					{
						if (scene_name == "PvPScene")
						{
							Start_Admob.Instance.ShowInter_time();
						}
					}
					else
					{
						Start_Admob.Instance.ShowInter_time();
						My_GoogleAnalytics.Instance.Down_ButtonContinueAfterReward(3);
					}
				}
				else
				{
					Start_Admob.Instance.ShowInter_time();
				}
			}
			else if (Status_Scene.Inst.ADS_first_enter)
			{
				Status_Scene.Inst.ADS_first_enter = false;
				Start_Admob.Instance.Show_Inter_default();
			}
			else
			{
				Start_Admob.Instance.ShowInter_time();
			}
		}
		if (scene_name != null && !(scene_name == "MainScene") && !(scene_name == "PvPScene") && !(scene_name == "GameScene") && scene_name == "PlayModeSelect")
		{
		}
	}

	public void Forced_LoadScene(string name_sceneForLoad)
	{
		if (Priv_NameScene != name_sceneForLoad)
		{
			Priv_NameScene = name_sceneForLoad;
		}
		AsyncOperation loading = SceneManager.LoadSceneAsync(name_sceneForLoad);
		if (loadingDelegate != null)
		{
			loadingDelegate(loading);
		}
	}
}
