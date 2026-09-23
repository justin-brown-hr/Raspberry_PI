using System;
using UnityEngine;

namespace Destructible2D
{
	[RequireComponent(typeof(D2dDestructible))]
	[AddComponentMenu("Destructible 2D/D2D Requirements")]
	public class D2dRequirements : MonoBehaviour
	{
		public bool UseDamageMin;

		[Tooltip("The minimum Damage required")]
		public float DamageMin;

		public bool UseDamageMax;

		[Tooltip("The maximum Damage required")]
		public float DamageMax;

		public bool UseAlphaCountMin;

		[Tooltip("The minimum AlphaCount required")]
		public int AlphaCountMin;

		public bool UseAlphaCountMax;

		[Tooltip("The maximum AlphaCount required")]
		public int AlphaCountMax;

		public bool UseRemainingAlphaMin;

		[Tooltip("The minimum RemainingAlpha required")]
		[Range(0f, 1f)]
		public float RemainingAlphaMin;

		public bool UseRemainingAlphaMax;

		[Tooltip("The maximum RemainingAlpha required")]
		[Range(0f, 1f)]
		public float RemainingAlphaMax = 1f;

		[Tooltip("This gets fired when all the requirements have been met")]
		public D2dEvent OnRequirementMet;

		[SerializeField]
		private bool met;

		[NonSerialized]
		private D2dDestructible destructible;

		public void UpdateMet()
		{
			if (met == CheckMet())
			{
				return;
			}
			if (met)
			{
				met = false;
				return;
			}
			met = true;
			if (OnRequirementMet != null)
			{
				OnRequirementMet.Invoke();
			}
		}

		protected virtual void Update()
		{
			UpdateMet();
		}

		private bool CheckMet()
		{
			if (destructible == null)
			{
				destructible = GetComponent<D2dDestructible>();
			}
			if (UseDamageMin && destructible.Damage < DamageMin)
			{
				return false;
			}
			if (UseDamageMax && destructible.Damage > DamageMax)
			{
				return false;
			}
			if (UseAlphaCountMin && destructible.AlphaCount < AlphaCountMin)
			{
				return false;
			}
			if (UseAlphaCountMax && destructible.AlphaCount > AlphaCountMax)
			{
				return false;
			}
			if (UseRemainingAlphaMin && destructible.RemainingAlpha < RemainingAlphaMin)
			{
				return false;
			}
			if (UseRemainingAlphaMax && destructible.RemainingAlpha > RemainingAlphaMax)
			{
				return false;
			}
			return true;
		}
	}
}
