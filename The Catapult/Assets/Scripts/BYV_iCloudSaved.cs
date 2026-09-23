using DG.Tweening;
using FSG.iOSKeychain;
using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BYV_iCloudSaved : MonoBehaviour
{
	public static BYV_iCloudSaved Instance;

	private string mAutoSaveName = "Autosaved_catapult";

	private IAchievement[] buff;

	private string id_get = string.Empty;

	private double precent_progres;

	private bool stat_rec;

	private Queue<KeyValuePair<string, float>> incrementSet;

	private bool dataSendingStarted;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void OnAuthenticate(string result)
	{
		UnityEngine.Debug.Log("========================================> Syncronizing <=======");
		BYV_LeaderBoard.Instance.AuthenticateUser();
		DOVirtual.DelayedCall(1f, delegate
		{
			Synchronizing();
		});
	}

	private void Synchronizing()
	{
		for (int i = 0; i < AchievementManager.instance.listOfAchievements.Length; i++)
		{
			AchieventData achieventData = AchievementManager.instance.listOfAchievements[i];
			if (!achieventData.isAchieved)
			{
				UnityEngine.Debug.Log("========================================> Syncronizing (" + i + ")-> " + achieventData.name + " <=======");
				AchievementManager.instance.RecheckFirstBought(i);
			}
		}
	}

	public void IOS_AchivmentsSend_Incriment(string achievementId, float percent)
	{
		if (incrementSet == null)
		{
			incrementSet = new Queue<KeyValuePair<string, float>>();
		}
		if (!dataSendingStarted)
		{
			dataSendingStarted = true;
			incrementSet.Enqueue(new KeyValuePair<string, float>(achievementId, percent));
			UnityEngine.Debug.Log("========================================> (1) Added first achievement - " + achievementId);
			StartCoroutine(SubmittingData());
		}
		else
		{
			incrementSet.Enqueue(new KeyValuePair<string, float>(achievementId, percent));
			UnityEngine.Debug.Log("========================================> (1) Added achievement - " + achievementId);
		}
	}

	private IEnumerator SubmittingData()
	{
		if (incrementSet.Count == 0)
		{
			dataSendingStarted = false;
			yield break;
		}
		yield return new WaitForSeconds(0.05f);
		KeyValuePair<string, float> pair = incrementSet.Dequeue();
		KTAchievementData data = KTGameCenter.SharedCenter().FetchAchievementData(pair.Key);
		if (data != null)
		{
			UnityEngine.Debug.Log("========================================> (2) Got achievement" + data.identifier + " and check completion = " + data.isComplete);
			if (data.isComplete)
			{
				yield break;
			}
		}
		else
		{
			UnityEngine.Debug.Log("========================================> (2) Cannot check achievement completion");
		}
		SubmitData(pair);
	}

	private void SubmitData(KeyValuePair<string, float> pair)
	{
		UnityEngine.Debug.Log("========================================> (3) Sending data - " + pair.Key);
		KTGameCenter.SharedCenter().SubmitIncrementalAchievement(pair.Value, pair.Key, showBanner: true);
	}

	private void AchievementSubmitted(string achId, string error)
	{
		UnityEngine.Debug.Log("========================================> (4) Achievement " + achId + " submitted");
		if (error == string.Empty)
		{
			UnityEngine.Debug.Log("========================================> (5) No error");
			CheckReward(achId);
		}
		else
		{
			UnityEngine.Debug.Log("========================================> (5) Error -> " + error);
			StartCoroutine(SubmittingData());
		}
	}

	private void CheckReward(string achievementId)
	{
		UnityEngine.Debug.Log("========================================> (6) Start achievement check");
		KTAchievementData kTAchievementData = KTGameCenter.SharedCenter().FetchAchievementData(achievementId);
		if (kTAchievementData != null)
		{
			UnityEngine.Debug.Log("========================================> (7) Got achievement" + kTAchievementData.identifier + " progress -> " + kTAchievementData.percantageComplete + " and completion = " + kTAchievementData.isComplete);
			if (kTAchievementData.isComplete)
			{
				UnityEngine.Debug.Log("========================================> (8) Recieve reward");
				AchievementManager.instance.RecieveReward(achievementId);
			}
		}
		else
		{
			UnityEngine.Debug.Log("========================================> (7) Cannot check achievement");
		}
		UnityEngine.Debug.Log("========================================> (9) Next step");
		StartCoroutine(SubmittingData());
	}

	private string ICloud_Read()
	{
		string result = string.Empty;
		try
		{
			result = Keychain.GetValue(mAutoSaveName);
			return result;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("----------------> Произошла ошибка получения данных от ICloud <---------------" + ex.ToString());
			return result;
		}
	}

	public void ICloud_Write(string str_data)
	{
		try
		{
			Keychain.SetValue(mAutoSaveName, str_data);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("----------------> Произошла ошибка записи данных в ICloud <---------------" + ex.ToString());
		}
	}

	public void SaveDataToCloud(string data)
	{
		if (data != string.Empty)
		{
			ICloud_Write(data);
		}
	}

	public void LoadDataFromCloud()
	{
		string text = ICloud_Read();
		if (text != string.Empty)
		{
			SavegameManager.instance.LoadGameData(text);
		}
		else
		{
			UnityEngine.Debug.Log("----------------> Данные от ICloud не получены <---------------");
		}
	}
}
