using Logic;
using UnityEngine;

namespace View
{
	public class FireSounds : MonoBehaviour
	{
		private AudioSource _source;

		private void Start()
		{
			_source = base.gameObject.GetComponent<AudioSource>();
			SoundMgr.instance.RegisterFireDebris(_source);
		}

		private void OnDestroy()
		{
			SoundMgr.instance.UnregisterFireDebris(_source);
		}
	}
}
