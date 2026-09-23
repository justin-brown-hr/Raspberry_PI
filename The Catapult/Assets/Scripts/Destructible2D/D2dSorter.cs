using System;
using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
	[AddComponentMenu("Destructible 2D/D2D Sorter")]
	public class D2dSorter : MonoBehaviour
	{
		[NonSerialized]
		private Renderer tempRenderer;

		public int SortingOrder
		{
			get
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				return tempRenderer.sortingOrder;
			}
			set
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				tempRenderer.sortingOrder = value;
			}
		}

		public int SortingLayerID
		{
			get
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				return tempRenderer.sortingLayerID;
			}
			set
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				tempRenderer.sortingLayerID = value;
			}
		}

		public string SortingLayerName
		{
			get
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				return tempRenderer.sortingLayerName;
			}
			set
			{
				if (tempRenderer == null)
				{
					tempRenderer = GetComponent<Renderer>();
				}
				tempRenderer.sortingLayerName = value;
			}
		}
	}
}
