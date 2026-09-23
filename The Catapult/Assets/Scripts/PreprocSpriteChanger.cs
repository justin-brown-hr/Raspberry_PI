using UnityEngine;
using UnityEngine.UI;

public class PreprocSpriteChanger : MonoBehaviour
{
	private Image referencedImage;

	public Sprite IOSSprite;

	public Sprite AndroidSprite;

	private void Start()
	{
		referencedImage = GetComponent<Image>();
		referencedImage.sprite = AndroidSprite;
	}
}
