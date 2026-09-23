using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	internal class ShieldSlotControl : MonoBehaviour
	{
		public PlayerShop shop;

		public Button clearButton;

		public Image shieldSprite;

		public void SetSprite(Sprite _sprite)
		{
			shieldSprite.sprite = _sprite;
			if (_sprite == null)
			{
				clearButton.gameObject.SetActive(value: false);
				shieldSprite.color = new Color(1f, 1f, 1f, 0f);
			}
			else
			{
				clearButton.gameObject.SetActive(value: true);
				shieldSprite.color = new Color(1f, 1f, 1f, 1f);
			}
		}

		public void ClearSlot()
		{
			SetSprite(null);
			shop.ClearShieldSlot();
		}
	}
}
