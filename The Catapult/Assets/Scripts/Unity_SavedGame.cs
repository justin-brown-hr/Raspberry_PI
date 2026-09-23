using DG.Tweening;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Logic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using View;

public class Unity_SavedGame : MonoBehaviour
{
	public static Unity_SavedGame Instance;

	private string mAutoSaveName = "Autosaved_catapult";

	private SavedGameRequestStatus _status;

	private ISavedGameMetadata _game;

	private SelectUIStatus ansver_status;

	private string pp_statusPlayService = "statplayservice";

	private bool synchronizeAfterError;

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
		synchronizeAfterError = false;
		if (!PlayerPrefs.HasKey(pp_statusPlayService))
		{
			PlayerPrefs.SetInt(pp_statusPlayService, 0);
		}
		if (GetStatus_PlayService())
		{
			Invoke("PlayService_SignIN", 1f);
		}
	}

	public void PlayService_SignOUT()
	{
		PlayerPrefs.SetInt(pp_statusPlayService, 0);
	//	PlayGamesPlatform.Instance.SignOut();
	}

	public void PlayService_SignIN()
	{
	//	PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Bviuild();
		//PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.Activate();
		Invoke("Start_Social", 1f);
	}

	private void Start_Social()
	{
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				OpenSavedGame(mAutoSaveName);
				UnityEngine.Debug.Log("< -------------- We Auteriset  SAVED GAME services ----------------- >");
			}
			else
			{
				UnityEngine.Debug.Log("< -------------- NO auterised SAVED GAME services ----------------- >");
			}
		});
	}

	public bool GetStatus_PlayService()
	{
		bool result = false;
		if (PlayerPrefs.HasKey(pp_statusPlayService) && PlayerPrefs.GetInt(pp_statusPlayService) == 1)
		{
			result = true;
		}
		return result;
	}

	private void OpenSavedGame(string filename)
	{
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		savedGame.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}

	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			UnityEngine.Debug.Log("------- Сохраненная игра считалась успешно и все хорошо ------");
			_status = SavedGameRequestStatus.Success;
			_game = game;
			if (!synchronizeAfterError)
			{
				DOVirtual.DelayedCall(2f, delegate
				{
					AchievementManager.instance.LoadServiceAchievementsStatus();
				});
			}
			else
			{
				synchronizeAfterError = false;
			}
			if (!GetStatus_PlayService())
			{
				AchievementManager.instance.ServiceConnectedManually();
			}
			PlayerPrefs.SetInt(pp_statusPlayService, 1);
			if (GUIControl_MainMenu.Instance != null && SceneManager.GetActiveScene().name == "MainScene")
			{
				GUIControl_MainMenu.Instance.CheckPGamesConnection();
			}
		}
		else
		{
			UnityEngine.Debug.Log("------- Сохраненная игра НЕ считалась успешно и все плохо ------");
		}
	}

	public void ShowSelectUI()
	{
		uint maxDisplayedSavedGames = 5u;
		bool showCreateSaveUI = false;
		bool showDeleteSaveUI = true;
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		savedGame.ShowSelectSavedGameUI(mAutoSaveName, maxDisplayedSavedGames, showCreateSaveUI, showDeleteSaveUI, SavedGameSelected);
	}

	public void SavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
	{
		if (status == SelectUIStatus.SavedGameSelected)
		{
			ansver_status = status;
			_game = game;
			GUIControl_MainMenu.Instance.OpenLoadgamePanel();
		}
		else
		{
			UnityEngine.Debug.LogWarning("< ------------------- Error selecting save game: " + status + " ---------------------->");
			OpenSavedGame(mAutoSaveName);
		}
	}

	public void SavedGame_Selected(bool ansver)
	{
		if (ansver && _game != null)
		{
			string text = _game.Filename;
			UnityEngine.Debug.Log(" ------- BYV ------- opening saved game >>> : " + _game);
			if (text == null || text.Length == 0)
			{
				text = "save" + DateTime.Now.ToBinary();
			}
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(text, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, SavedGameOpened);
		}
		else
		{
			UnityEngine.Debug.LogWarning("< ------------------- Error selecting save game: " + ansver_status + " ---------------------->");
			OpenSavedGame(mAutoSaveName);
		}
	}

	public void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
		}
		else
		{
			UnityEngine.Debug.LogWarning("< ------ Error opening game: " + status + " -------- >");
		}
	}

	public void SavedGameLoaded(SavedGameRequestStatus status, byte[] data)
	{
		UnityEngine.Debug.Log("----- пользователь выбрал сохраненную игру ------");
		if (status == SavedGameRequestStatus.Success)
		{
			UnityEngine.Debug.Log("SaveGameLoaded, success=" + status);
			try
			{
				SavegameManager.instance.LoadGameData(data);
			}
			catch
			{
				synchronizeAfterError = true;
				OpenSavedGame(mAutoSaveName);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Error reading game: " + status);
		}
	}

	public void Save_GameInCloud(byte[] savedArray, Texture2D image)
	{
		ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
		if (_status == SavedGameRequestStatus.Success)
		{
			SavedGameMetadataUpdate.Builder builder = default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(TimeSpan.MinValue).WithUpdatedDescription("Saved game at " + DateTime.Now);
			if (image != null)
			{
				Texture2D tex = Resize(image, image.width / 2, image.height / 2);
				byte[] newPngCoverImage = tex.EncodeToPNG();
				builder = builder.WithUpdatedPngCoverImage(newPngCoverImage);
			}
			SavedGameMetadataUpdate updateForMetadata = builder.Build();
			savedGame.CommitUpdate(_game, updateForMetadata, savedArray, SavedGameWritten);
		}
		else
		{
			UnityEngine.Debug.Log("< ----- Erorr Saved_GAmeInCloud ------>");
		}
	}

	public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
	{
		source.filterMode = FilterMode.Point;
		RenderTexture temporary = RenderTexture.GetTemporary(newWidth, newHeight);
		temporary.filterMode = FilterMode.Point;
		RenderTexture.active = temporary;
		Graphics.Blit(source, temporary);
		Texture2D texture2D = new Texture2D(newWidth, newHeight);
		texture2D.ReadPixels(new Rect(0f, 0f, newWidth, newWidth), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		return texture2D;
	}

	public void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			UnityEngine.Debug.LogWarning("--------- BYV ------------ Статус сохранения игры >>> : " + status);
			OpenSavedGame(mAutoSaveName);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Error saving game: " + status);
		}
	}
}
