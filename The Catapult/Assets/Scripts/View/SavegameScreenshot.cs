using Logic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class SavegameScreenshot : MonoBehaviour
	{
		public RectTransform scaledBackground;

		public Image displayImage;

		public Image shotEffect;

		public Animator animator;

		public Text coinCountText;

		public Image backgroundShade;

		public GameObject photoCanvas;

		public CanvasScaler referencedScaler;

		private bool needPhotoEffect;

		private bool takeNextFrameShot;

		private bool canTakeScreenshot;

		private void Awake()
		{
			displayImage.gameObject.SetActive(value: false);
			shotEffect.gameObject.SetActive(value: false);
			scaledBackground.gameObject.SetActive(value: false);
			backgroundShade.gameObject.SetActive(value: false);
			canTakeScreenshot = true;
			takeNextFrameShot = false;
		}

		public void TakeScreensot(bool needEffect = true)
		{
			if (canTakeScreenshot)
			{
				canTakeScreenshot = false;
				Vector2 sizeDelta = referencedScaler.GetComponent<RectTransform>().sizeDelta;
				Vector2 vector = new Vector2(Screen.width, Screen.height);
				scaledBackground.localScale = new Vector3(1f, 1f, 1f);
				scaledBackground.localRotation = new Quaternion(0f, 0f, 0f, 1f);
				scaledBackground.sizeDelta = sizeDelta;
				scaledBackground.transform.localPosition = new Vector3(0f, 0f, 0f);
				animator.enabled = false;
				animator.gameObject.SetActive(value: false);
				coinCountText.text = NewDataController.instance.GetPlayerMoney().ToString();
				StartCoroutine(FlashEffect());
			}
		}

		private IEnumerator FlashEffect()
		{
			shotEffect.color = new Color(1f, 1f, 1f, 0f);
			shotEffect.gameObject.SetActive(value: true);
			while (true)
			{
				Color color = shotEffect.color;
				if (!(color.a < 0.95f))
				{
					break;
				}
				Image image = shotEffect;
				Color color2 = shotEffect.color;
				image.color = new Color(1f, 1f, 1f, color2.a + 20f * Time.deltaTime);
				yield return null;
			}
			takeNextFrameShot = true;
			yield return new WaitForSeconds(0.2f);
			shotEffect.color = new Color(1f, 1f, 1f, 1f);
			while (true)
			{
				Color color3 = shotEffect.color;
				if (!(color3.a > 0.05f))
				{
					break;
				}
				Image image2 = shotEffect;
				Color color4 = shotEffect.color;
				image2.color = new Color(1f, 1f, 1f, color4.a - 4f * Time.deltaTime);
				yield return null;
			}
			shotEffect.color = new Color(1f, 1f, 1f, 0f);
			shotEffect.gameObject.SetActive(value: false);
		}

		private IEnumerator ShotEffect()
		{
			Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
			texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, recalculateMipMaps: false);
			texture.Apply();
			SavegameManager.instance.SaveGameData(texture);
			Sprite photo = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
			backgroundShade.color = new Color(0f, 0f, 0f, 0.6f);
			backgroundShade.gameObject.SetActive(value: true);
			scaledBackground.gameObject.SetActive(value: true);
			displayImage.gameObject.SetActive(value: true);
			displayImage.sprite = photo;
			yield return new WaitForSeconds(0.5f);
			float randomRotation = UnityEngine.Random.Range(-0.2f, 0.2f);
			Quaternion rotation = base.transform.rotation;
			float x = rotation.x;
			Quaternion rotation2 = base.transform.rotation;
			float y = rotation2.y;
			Quaternion rotation3 = base.transform.rotation;
			float z = rotation3.z + randomRotation;
			Quaternion rotation4 = base.transform.rotation;
			Quaternion rot = new Quaternion(x, y, z, rotation4.w);
			float t2 = 0f;
			while (true)
			{
				Vector3 localScale = scaledBackground.localScale;
				if (!(localScale.x > 0.5f))
				{
					break;
				}
				scaledBackground.localRotation = Quaternion.Slerp(base.transform.localRotation, rot, t2);
				t2 += 2f * Time.deltaTime;
				RectTransform rectTransform = scaledBackground;
				Vector3 localScale2 = scaledBackground.localScale;
				float x2 = localScale2.x - Time.deltaTime;
				Vector3 localScale3 = scaledBackground.localScale;
				rectTransform.localScale = new Vector3(x2, localScale3.y - Time.deltaTime, 1f);
				yield return null;
			}
			animator.enabled = true;
			animator.playbackTime = 0f;
			animator.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(2f);
			t2 = 0f;
			Vector3 pos2 = new Vector2(Screen.width / 2, Screen.height / 2);
			pos2 += scaledBackground.localPosition;
			while (t2 < 1f)
			{
				Color color = backgroundShade.color;
				if (color.a > 0.05f)
				{
					Image image = backgroundShade;
					Color color2 = shotEffect.color;
					image.color = new Color(0f, 0f, 0f, color2.a - Time.deltaTime);
				}
				t2 += Time.deltaTime / 2f;
				scaledBackground.transform.localPosition = Vector3.Lerp(scaledBackground.transform.localPosition, pos2, t2);
				Vector3 localScale4 = scaledBackground.localScale;
				if (localScale4.x > 0.05f)
				{
					RectTransform rectTransform2 = scaledBackground;
					Vector3 localScale5 = scaledBackground.localScale;
					float x3 = localScale5.x - 3f * Time.deltaTime;
					Vector3 localScale6 = scaledBackground.localScale;
					rectTransform2.localScale = new Vector3(x3, localScale6.y - 3f * Time.deltaTime, 1f);
				}
				yield return null;
			}
			backgroundShade.gameObject.SetActive(value: false);
			scaledBackground.gameObject.SetActive(value: false);
			canTakeScreenshot = true;
		}

		private void OnPostRender()
		{
			if (takeNextFrameShot)
			{
				StartCoroutine(ShotEffect());
				takeNextFrameShot = false;
			}
		}
	}
}
