using UnityEngine;
using UnityEngine.UI;

public class contentController : MonoBehaviour
{
	public static contentController Instance;

	public GameObject content;

	public GameObject prefImage;

	public GameObject PromoCanvas;

	public GameObject ViewPort;

	public int positionYContent;

	public int nextNumber = 190;

	public int countImage;

	public float timer;

	public bool down = true;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
	}

	private void Update()
	{
		if (countImage > 0)
		{
			if (!PromoCanvas.GetComponent<Canvas>().enabled)
			{
				PromoCanvas.GetComponent<Canvas>().enabled = true;
			}
			if (countImage <= 1)
			{
				return;
			}
			timer += Time.deltaTime;
			if (!(timer >= 5f))
			{
				return;
			}
			if (down)
			{
				positionYContent += 5;
				content.GetComponent<RectTransform>().transform.localPosition = new Vector3(0f, positionYContent, 0f);
				if (positionYContent >= nextNumber)
				{
					timer = 0f;
					if (nextNumber + 190 >= countImage * 190)
					{
						down = false;
						positionYContent = nextNumber;
						nextNumber -= 190;
					}
					else
					{
						positionYContent = nextNumber;
						nextNumber += 190;
					}
				}
				return;
			}
			if (positionYContent <= 0)
			{
				down = true;
				return;
			}
			positionYContent -= 5;
			content.GetComponent<RectTransform>().transform.localPosition = new Vector3(0f, positionYContent, 0f);
			if (positionYContent <= nextNumber)
			{
				timer = 0f;
				if (nextNumber - 190 <= -190)
				{
					down = true;
					positionYContent = nextNumber;
					nextNumber += 190;
				}
				else
				{
					positionYContent = nextNumber;
					nextNumber -= 190;
				}
			}
		}
		else if (PromoCanvas.GetComponent<Canvas>().enabled)
		{
			PromoCanvas.GetComponent<Canvas>().enabled = false;
		}
	}

	public void LoadImageToPromo(Sprite spr, string url, string nameGame, string iosId)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefImage, content.transform, worldPositionStays: true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<Image>().preserveAspect = true;
		gameObject.GetComponent<Image>().sprite = spr;
		gameObject.GetComponent<OpenUrl>().Set_Url(url, nameGame, iosId);
		UnityEngine.Debug.Log("LoadImageToPromo");
		countImage++;
		gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
	}
}
