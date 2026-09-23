using UnityEngine;

namespace View
{
	internal class DebrisPartView : MonoBehaviour
	{
		public GameObject firstDebris;

		public GameObject secondDebris;

		public void RedirectJoint(HingeJoint2D joint)
		{
			if (joint.connectedBody != null)
			{
				HingeJoint2D hingeJoint2D = secondDebris.AddComponent<HingeJoint2D>();
				hingeJoint2D.connectedBody = joint.connectedBody;
				hingeJoint2D.anchor = joint.anchor;
				hingeJoint2D.connectedAnchor = joint.connectedAnchor;
			}
			else if (secondDebris != null)
			{
				secondDebris.transform.parent = null;
			}
			if (firstDebris != null)
			{
				firstDebris.transform.parent = null;
			}
		}
	}
}
