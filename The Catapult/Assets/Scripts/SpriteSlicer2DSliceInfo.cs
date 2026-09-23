using System.Collections.Generic;
using UnityEngine;

public class SpriteSlicer2DSliceInfo
{
	private List<GameObject> m_ChildObjects = new List<GameObject>();

	public GameObject SlicedObject
	{
		get;
		set;
	}

	public Vector2 SliceEnterWorldPosition
	{
		get;
		set;
	}

	public Vector2 SliceExitWorldPosition
	{
		get;
		set;
	}

	public List<GameObject> ChildObjects
	{
		get
		{
			return m_ChildObjects;
		}
		set
		{
			m_ChildObjects = value;
		}
	}
}
