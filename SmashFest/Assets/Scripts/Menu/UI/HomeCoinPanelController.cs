using Service;
using UnityEngine;

namespace Menu.UI
{
	public class HomeCoinPanelController : MonoBehaviour
	{
		[SerializeField]
		private FlowButton button;

		private void Start()
		{
			if (button != null)
			{
				button.OnClick.RemoveListener(OnButtonClick);
				button.OnClick.AddListener(OnButtonClick);
			}
		}

		private void OnButtonClick()
		{
			ServiceLocator.Get<MenuController>()?.SelectPage(PageType.Shop);
		}
	}
}
