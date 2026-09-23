using UnityEngine;
using UnityEngine.EventSystems;

public class ResetTimerAmination : EventTrigger
{
	public override void OnDrag(PointerEventData data)
	{
		if ((bool)contentController.Instance)
		{
			contentController.Instance.timer = 0f;
			contentController instance = contentController.Instance;
			Vector3 localPosition = contentController.Instance.content.GetComponent<RectTransform>().transform.localPosition;
			instance.positionYContent = (int)localPosition.y;
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if (!contentController.Instance)
		{
			return;
		}
		Vector3 localPosition = contentController.Instance.content.GetComponent<RectTransform>().transform.localPosition;
		int num = (int)localPosition.y % 190;
		if (num > 95)
		{
			Transform transform = contentController.Instance.content.GetComponent<RectTransform>().transform;
			Vector3 localPosition2 = contentController.Instance.content.transform.localPosition;
			transform.localPosition = new Vector3(0f, (int)localPosition2.y + (190 - num), 0f);
			contentController instance = contentController.Instance;
			Vector3 localPosition3 = contentController.Instance.content.GetComponent<RectTransform>().transform.localPosition;
			instance.positionYContent = (int)localPosition3.y;
		}
		else
		{
			Transform transform2 = contentController.Instance.content.GetComponent<RectTransform>().transform;
			Vector3 localPosition4 = contentController.Instance.content.transform.localPosition;
			transform2.localPosition = new Vector3(0f, (int)localPosition4.y - num, 0f);
			contentController instance2 = contentController.Instance;
			Vector3 localPosition5 = contentController.Instance.content.GetComponent<RectTransform>().transform.localPosition;
			instance2.positionYContent = (int)localPosition5.y;
		}
		contentController.Instance.nextNumber = contentController.Instance.positionYContent;
		if (contentController.Instance.down)
		{
			if (contentController.Instance.nextNumber + 190 >= contentController.Instance.countImage * 190)
			{
				contentController.Instance.down = false;
				contentController.Instance.nextNumber -= 190;
			}
			else
			{
				contentController.Instance.nextNumber += 190;
			}
		}
		else if (contentController.Instance.positionYContent <= 0)
		{
			contentController.Instance.down = true;
		}
		else if (contentController.Instance.nextNumber - 190 <= -190)
		{
			contentController.Instance.down = true;
			contentController.Instance.nextNumber += 190;
		}
		else
		{
			contentController.Instance.nextNumber -= 190;
		}
	}
}
