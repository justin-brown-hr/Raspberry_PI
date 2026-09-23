using Assets.All_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GDPR : MonoBehaviour
{
	public GameObject GDPR_Panel;

	public Toggle toggle_AdMob;

	public Toggle toggle_GoogleAnl;

	private bool is_player_in_EU;

	private string[] Contry_EU = new string[31]
	{
		"AT",
		"BE",
		"BG",
		"HU",
		"GB",
		"GR",
		"DE",
		"DK",
		"IT",
		"IE",
		"ES",
		"CY",
		"LU",
		"LV",
		"LT",
		"MT",
		"NL",
		"PT",
		"PL",
		"RO",
		"SI",
		"SK",
		"FR",
		"FI",
		"HR",
		"CZ",
		"SE",
		"EE",
		"IS",
		"LI",
		"NO"
	};

	private void Start()
	{
		Status_Scene.Inst.Inspection_dataToglesGDPR();
		is_player_in_EU = false;
		GDPR_Panel.SetActive(value: false);
		string region = PreciseLocale.GetRegion();
		if (!PlayerPrefs.HasKey("gdpr"))
		{
			for (int i = 0; i < Contry_EU.Length; i++)
			{
				if (region == Contry_EU[i])
				{
					PlayerPrefs.SetInt("gdpr", 0);
					GDPR_Panel.SetActive(value: true);
					is_player_in_EU = true;
					break;
				}
			}
			if (!is_player_in_EU)
			{
				YesButton();
			}
		}
		else if (PlayerPrefs.GetInt("gdpr") == 0)
		{
			GDPR_Panel.SetActive(value: true);
		}
		else
		{
			SceneManager.LoadScene("InitScene");
		}
	}

	public void YesButton()
	{
		Status_Scene.Inst.SetAcces_GDPR(stat_GDPR: true);
		Status_Scene.Inst.SetAcces_GoogleAnlGDPR(GoogleAnlstat_GDPR: true);
		PlayerPrefs.SetInt("gdpr", 1);
		SceneManager.LoadScene("InitScene");
	}

	public void NoButton()
	{
		Status_Scene.Inst.SetAcces_GDPR(stat_GDPR: false);
		Status_Scene.Inst.SetAcces_GoogleAnlGDPR(GoogleAnlstat_GDPR: false);
		PlayerPrefs.SetInt("gdpr", 1);
		SceneManager.LoadScene("InitScene");
	}

	public void SubmitButton()
	{
		if (toggle_AdMob.isOn)
		{
			Status_Scene.Inst.SetAcces_GDPR(stat_GDPR: true);
		}
		else
		{
			Status_Scene.Inst.SetAcces_GDPR(stat_GDPR: false);
		}
		if (toggle_GoogleAnl.isOn)
		{
			Status_Scene.Inst.SetAcces_GoogleAnlGDPR(GoogleAnlstat_GDPR: true);
		}
		else
		{
			Status_Scene.Inst.SetAcces_GoogleAnlGDPR(GoogleAnlstat_GDPR: false);
		}
		PlayerPrefs.SetInt("gdpr", 1);
		SceneManager.LoadScene("InitScene");
	}

	public void OpenPrivacePolicy()
	{
		Application.OpenURL("http://byvgames.com/private-policy.html");
	}
}
