using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("Layout/Auto Grid Layout Group", 152)]
public class AutoGridLayout : GridLayoutGroup
{
	[SerializeField]
	private bool m_IsColumn;

	[SerializeField]
	private int m_Column = 1;

	[SerializeField]
	private int m_Row = 1;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		float num = -1f;
		float num2 = -1f;
		if (m_IsColumn)
		{
			num = m_Column;
			if (num <= 0f)
			{
				num = 1f;
			}
			num2 = Mathf.CeilToInt((float)base.transform.childCount / num);
		}
		else
		{
			num2 = m_Row;
			if (num2 <= 0f)
			{
				num2 = 1f;
			}
			num = Mathf.CeilToInt((float)base.transform.childCount / num2);
		}
		float height = base.rectTransform.rect.height;
		float num3 = num2 - 1f;
		Vector2 spacing = base.spacing;
		float num4 = height - num3 * spacing.y - (float)(base.padding.top + base.padding.bottom);
		float width = base.rectTransform.rect.width;
		float num5 = num - 1f;
		Vector2 spacing2 = base.spacing;
		float num6 = width - num5 * spacing2.x - (float)(base.padding.right + base.padding.left);
		Vector2 vector2 = base.cellSize = new Vector2(num6 / num, num4 / num2);
	}
}
