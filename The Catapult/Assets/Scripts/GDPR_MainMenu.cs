using Assets.All_Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GDPR_MainMenu : MonoBehaviour
{
	public Toggle toggle_AdMob;

	public Toggle toggle_GoogleAnl;

	public static GDPR_MainMenu Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		StartVisible_toggle_AdMob();
	}

	private void StartVisible_toggle_AdMob()
	{
		toggle_AdMob.isOn = Status_Scene.Inst.GetAcces_GDPR();
		toggle_GoogleAnl.isOn = Status_Scene.Inst.GetAcces_GoogleAnlGDPR();
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
	}

	public void OpenPrivacePolicy()
	{
		Application.OpenURL("http://byvgames.com/private-policy.html");
	}
}
