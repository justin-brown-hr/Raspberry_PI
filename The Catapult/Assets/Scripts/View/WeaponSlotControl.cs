using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	internal class WeaponSlotControl : MonoBehaviour
	{
		public PlayerShop shop;

		public Button clearButton;

		public Image projectileSprite;

		public int slotIndex;

		public void SetSprite(Sprite _sprite)
		{
			projectileSprite.sprite = _sprite;
			if (_sprite == null)
			{
				clearButton.gameObject.SetActive(value: false);
				projectileSprite.color = new Color(1f, 1f, 1f, 0f);
			}
			else
			{
				clearButton.gameObject.SetActive(value: true);
				projectileSprite.color = new Color(1f, 1f, 1f, 1f);
			}
		}

		public void ClearSlot()
		{
			SetSprite(null);
			shop.ClearWeaponSlot(slotIndex);
		}
	}
}
