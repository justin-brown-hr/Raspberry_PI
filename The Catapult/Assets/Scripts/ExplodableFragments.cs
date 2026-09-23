using System.Collections.Generic;
using UnityEngine;

public class ExplodableFragments : ExplodableAddon
{
	public override void OnFragmentsGenerated(List<GameObject> fragments)
	{
		foreach (GameObject fragment in fragments)
		{
			Explodable explodable = fragment.AddComponent<Explodable>();
			explodable.shatterType = base.explodable.shatterType;
			explodable.fragmentLayer = base.explodable.fragmentLayer;
			explodable.sortingLayerName = base.explodable.sortingLayerName;
			explodable.orderInLayer = base.explodable.orderInLayer;
			fragment.layer = base.explodable.gameObject.layer;
			explodable.fragmentInEditor();
		}
	}
}
