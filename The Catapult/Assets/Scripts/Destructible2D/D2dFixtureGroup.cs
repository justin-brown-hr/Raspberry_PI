using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Destructible 2D/D2D Fixture Group")]
	public class D2dFixtureGroup : MonoBehaviour
	{
		[Tooltip("The fixtures tracked by this group")]
		public List<D2dFixture> Fixtures;

		[Tooltip("Automatically destroy this component if all fixtures are removed?")]
		public bool AutoDestroy = true;

		public D2dEvent OnAllFixturesRemoved;

		public void UpdateFixtures()
		{
			if (Fixtures.Count <= 0)
			{
				return;
			}
			for (int num = Fixtures.Count - 1; num >= 0; num--)
			{
				D2dFixture fixture = Fixtures[num];
				if (!FixtureIsConnected(fixture))
				{
					Fixtures.RemoveAt(num);
				}
			}
			if (Fixtures.Count == 0)
			{
				if (OnAllFixturesRemoved != null)
				{
					OnAllFixturesRemoved.Invoke();
				}
				if (AutoDestroy)
				{
					D2dHelper.Destroy(this);
				}
			}
		}

		protected virtual void Update()
		{
			UpdateFixtures();
		}

		private bool FixtureIsConnected(D2dFixture fixture)
		{
			if (fixture != null)
			{
				Transform transform = fixture.transform;
				while (transform != null)
				{
					if (transform == base.transform)
					{
						return true;
					}
					transform = transform.parent;
				}
			}
			return false;
		}
	}
}
