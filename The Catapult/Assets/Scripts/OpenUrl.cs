using Promo;
using UnityEngine;

public class OpenUrl : MonoBehaviour
{
	public string packageName;

	public string nameGame;

	public string idIosPackage;

	public void Set_Url(string pn, string ng, string iosId)
	{
		nameGame = ng;
		packageName = pn;
		idIosPackage = iosId;
	}

	public void OpenUrlMarket()
	{
		Application.OpenURL("market://details?id=" + packageName);
		My_GoogleAnalytics.Instance.Click_Promo_Module(nameGame);
		PromoModule.PressPromo(packageName);
		contentController.Instance.countImage--;
		contentController.Instance.positionYContent = 0;
		contentController.Instance.nextNumber = 190;
		contentController.Instance.content.GetComponent<RectTransform>().transform.localPosition = Vector3.zero;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!PromoModule.instance.PromoCanvas.GetComponent<Canvas>().worldCamera)
		{
			PromoModule.instance.PromoCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
		}
		if ((bool)contentController.Instance && (bool)contentController.Instance.ViewPort.GetComponent<RectTransform>() && (bool)GetComponent<RectTransform>() && RectTransformUtility.RectangleContainsScreenPoint(contentController.Instance.ViewPort.GetComponent<RectTransform>(), GetComponent<RectTransform>().position))
		{
			NamesGame.Instance.GetName(nameGame);
		}
	}
}
