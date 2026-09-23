using UnityEngine;

namespace Assets.All_Scripts
{
	public class Status_Scene
	{
		public string scene_forLoad = "MainMenu";

		public float count_load_level1_scene;

		public int enter_SsceneMainMenu;

		public bool status_MatchMaking = true;

		public bool status_Close_mMatch = true;

		public int score_left;

		public int score_right;

		public bool status_delete_strels;

		public string my_location = string.Empty;

		public GameObject new_enemy_gameObj;

		public GameObject new_player_gameObj;

		public bool stat_exit_mainGame;

		public bool MainCamera_orientation = true;

		public bool status_HP_enemy = true;

		public bool status_HP_player = true;

		public bool ADS_first_enter = true;

		public bool status_timer_shows_after_dead;

		public bool status_visible_small_banner = true;

		public bool status_create_inv_native_rezult;

		public string what_is_this_scene = string.Empty;

		public bool status_order_invitation = true;

		public bool status_first_shot_after_dead = true;

		public static Status_Scene Inst = new Status_Scene();

		public bool Status_RateGame;

		private bool Admob_statusGDPR = true;

		private bool GoogleAnl_statusGDPR = true;

		public string status_NoADS => "status_noads";

		public void SetAcces_GDPR(bool stat_GDPR)
		{
			if (stat_GDPR)
			{
				PlayerPrefs.SetInt("visiblToggleAdMob", 0);
				Admob_statusGDPR = true;
			}
			else
			{
				PlayerPrefs.SetInt("visiblToggleAdMob", 1);
				Admob_statusGDPR = false;
			}
		}

		public bool GetAcces_GDPR()
		{
			return Admob_statusGDPR;
		}

		public void SetAcces_GoogleAnlGDPR(bool GoogleAnlstat_GDPR)
		{
			if (GoogleAnlstat_GDPR)
			{
				PlayerPrefs.SetInt("visiblToggleGoogleAnl", 0);
				GoogleAnl_statusGDPR = true;
			}
			else
			{
				PlayerPrefs.SetInt("visiblToggleGoogleAnl", 1);
				GoogleAnl_statusGDPR = false;
			}
		}

		public bool GetAcces_GoogleAnlGDPR()
		{
			return GoogleAnl_statusGDPR;
		}

		public void Inspection_dataToglesGDPR()
		{
			if (!PlayerPrefs.HasKey("visiblToggleAdMob"))
			{
				Admob_statusGDPR = true;
			}
			else if (PlayerPrefs.GetInt("visiblToggleAdMob") == 0)
			{
				Admob_statusGDPR = true;
			}
			else
			{
				Admob_statusGDPR = false;
			}
			if (!PlayerPrefs.HasKey("visiblToggleGoogleAnl"))
			{
				GoogleAnl_statusGDPR = true;
			}
			else if (PlayerPrefs.GetInt("visiblToggleGoogleAnl") == 0)
			{
				GoogleAnl_statusGDPR = true;
			}
			else
			{
				GoogleAnl_statusGDPR = false;
			}
		}
	}
}
