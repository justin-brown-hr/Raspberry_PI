using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
	public class SoundMgr : MonoBehaviour
	{
		public static SoundMgr instance;

		private const int poolSize = 30;

		private AudioSource[] sourcePool = new AudioSource[30];

		private AudioSource musicDedicatedSource;

		private float soundTick;

		private int maxWoodAllowed = 5;

		private int maxWoodSplashAllowed = 3;

		private int woodCalled;

		private int woodSplashCalled;

		private AudioSource currentAimingSource;

		private AudioClip currentAimingSound;

		private AudioSource currentMovingSource;

		private AudioSource currentChargeSource;

		private AudioSource currentEntMovingSource;

		private AudioSource currentBossEncouter;

		public string purchaseSoundName;

		public string pressSoundName;

		public string[] backgroundMusicName;

		public string[] crowScreamName;

		public string[] woodPartHitName;

		public string[] stonePartHitName;

		public string[] playerHitName;

		public string[] crowHitName;

		public string[] crowHitAfterName;

		public string[] playerBleedName;

		public string[] playerDeathName;

		public string[] woodSplashName;

		public string[] stoneSplashName;

		public string[] playerSplashName;

		public string[] bombExplosionName;

		public string[] bigBombExplosionName;

		public string[] catapultLaunchName;

		public string[] catapultAimingName;

		public string[] stoneBrokeName;

		public string[] woodBrokeName;

		public string[] playerJumpName;

		public string[] catapultMovingName;

		public string[] bombSoundFireName;

		public string[] towerMoveName;

		public string[] fireWorksName;

		public string[] catapultUpgradeName;

		public string[] helmetEquipName;

		public string[] projectileEquipName;

		public string[] catapultEquipName;

		public string[] shieldEquipName;

		public string[] shieldReactName;

		public string[] projectileShootThornsName;

		public string[] FireProjectileName;

		public string[] barrelDestroyName;

		public string[] acidExplodeName;

		public string[] helmetHitName;

		public string[] shieldHitName;

		public string[] slotClearName;

		public string[] spoonDestroyName;

		public string[] bossIncomingName;

		private AudioSource[] towerSources;

		private bool bloodSoundAllowed;

		private List<AudioSource> bombSounds = new List<AudioSource>();

		private List<AudioSource> fireSounds = new List<AudioSource>();

		private List<AudioSource> fireDebrisSounds = new List<AudioSource>();

		public bool musicEnabled
		{
			get;
			private set;
		}

		public bool soundEnabled
		{
			get;
			private set;
		}

		private void Awake()
		{
			towerSources = new AudioSource[2];
			instance = this;
			woodCalled = 0;
			soundTick = 0f;
			currentAimingSource = null;
			currentAimingSound = null;
			bloodSoundAllowed = true;
			currentMovingSource = null;
			currentEntMovingSource = null;
			currentBossEncouter = null;
		}

		private void Start()
		{
			CreatePool();
			CheckSoundSettings();
		}

		private void CheckSoundSettings()
		{
			if (!PlayerPrefs.HasKey("Sound"))
			{
				PlayerPrefs.SetInt("Sound", 1);
				AudioListener.volume = 1f;
				soundEnabled = true;
			}
			else
			{
				soundEnabled = ((PlayerPrefs.GetInt("Sound") != 0) ? true : false);
				if (soundEnabled)
				{
					AudioListener.volume = 1f;
				}
				else
				{
					AudioListener.volume = 0f;
				}
			}
			if (!PlayerPrefs.HasKey("Music"))
			{
				PlayerPrefs.SetInt("Music", 1);
				musicEnabled = true;
				musicDedicatedSource.volume = 0.75f;
				return;
			}
			musicEnabled = ((PlayerPrefs.GetInt("Music") != 0) ? true : false);
			if (musicEnabled)
			{
				musicDedicatedSource.volume = 0.75f;
			}
			else
			{
				musicDedicatedSource.volume = 0f;
			}
		}

		public void HandleMusic()
		{
			if (musicEnabled)
			{
				musicEnabled = false;
				musicDedicatedSource.volume = 0f;
				musicDedicatedSource.Stop();
				PlayerPrefs.SetInt("Music", 0);
			}
			else
			{
				musicEnabled = true;
				musicDedicatedSource.volume = 0.75f;
				PlayMusic();
				PlayerPrefs.SetInt("Music", 1);
			}
		}

		public void HandleSound()
		{
			if (soundEnabled)
			{
				soundEnabled = false;
				AudioListener.volume = 0f;
				PlayerPrefs.SetInt("Sound", 0);
			}
			else
			{
				soundEnabled = true;
				AudioListener.volume = 1f;
				PlayerPrefs.SetInt("Sound", 1);
			}
		}

		public void PlayMusic()
		{
			if (musicEnabled)
			{
				musicDedicatedSource.loop = true;
				musicDedicatedSource.ignoreListenerVolume = true;
				musicDedicatedSource.clip = GetResource(backgroundMusicName[Random.Range(0, backgroundMusicName.Length)]);
				musicDedicatedSource.Play();
			}
		}

		private void CreatePool()
		{
			musicDedicatedSource = base.gameObject.AddComponent<AudioSource>();
			for (int i = 0; i < 30; i++)
			{
				AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
				sourcePool[i] = audioSource;
			}
		}

		private AudioSource GetFreeSource()
		{
			if (!soundEnabled)
			{
				return null;
			}
			AudioSource audioSource = null;
			for (int i = 0; i < 30; i++)
			{
				if (!sourcePool[i].isPlaying && sourcePool[i] != currentEntMovingSource && sourcePool[i] != currentMovingSource && sourcePool[i] != currentBossEncouter)
				{
					audioSource = sourcePool[i];
					audioSource.volume = 1f;
					audioSource.loop = false;
					break;
				}
			}
			return audioSource;
		}

		private AudioClip GetResource(string name)
		{
			AudioClip audioClip = null;
			string text = "Sounds/" + name;
			audioClip = Resources.Load<AudioClip>(text);
			if (audioClip == null)
			{
				UnityEngine.Debug.LogError("Wrong path: " + text);
			}
			return audioClip;
		}

		public void BarrelDestroy()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(barrelDestroyName[Random.Range(0, barrelDestroyName.Length)]);
				freeSource.Play();
			}
		}

		public void AcidExplode()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(acidExplodeName[Random.Range(0, acidExplodeName.Length)]);
				freeSource.Play();
			}
		}

		public void HelmetHit()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(helmetHitName[Random.Range(0, helmetHitName.Length)]);
				freeSource.Play();
			}
		}

		public void ShieldHit()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(shieldHitName[Random.Range(0, shieldHitName.Length)]);
				freeSource.Play();
			}
		}

		public void SlotClear()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(slotClearName[Random.Range(0, slotClearName.Length)]);
				freeSource.Play();
			}
		}

		public void SpoonDestroy()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(spoonDestroyName[Random.Range(0, spoonDestroyName.Length)]);
				freeSource.Play();
			}
		}

		public void WoodPartHit()
		{
			if (woodCalled < maxWoodAllowed)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					woodCalled++;
					AudioClip audioClip = freeSource.clip = GetResource(woodPartHitName[Random.Range(0, (int)woodPartHitName.LongLength)]);
					freeSource.Play();
				}
			}
		}

		public void StonePartHit()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				AudioClip audioClip = freeSource.clip = GetResource(stonePartHitName[Random.Range(0, (int)stonePartHitName.LongLength)]);
				freeSource.Play();
			}
		}

		public void WoodDestroy()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				AudioClip audioClip = freeSource.clip = GetResource(woodBrokeName[Random.Range(0, (int)woodBrokeName.LongLength)]);
				freeSource.Play();
			}
		}

		public void CatapultLaunch(bool playerShoot = false)
		{
			AudioSource freeSource = GetFreeSource();
			if (!(freeSource != null))
			{
				return;
			}
			if (currentAimingSource != null && playerShoot)
			{
				if (currentAimingSource.isPlaying && currentAimingSource.clip == currentAimingSound)
				{
					currentAimingSource.Stop();
				}
				currentAimingSource = null;
				currentAimingSound = null;
			}
			freeSource.clip = GetResource(catapultLaunchName[Random.Range(0, catapultLaunchName.Length)]);
			freeSource.Play();
		}

		public void BombExplosion()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(bombExplosionName[Random.Range(0, bombExplosionName.Length)]);
				freeSource.Play();
			}
		}

		public void BigBombExplosion()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(bigBombExplosionName[Random.Range(0, bigBombExplosionName.Length)]);
				freeSource.Play();
			}
		}

		public void WoodSplash()
		{
			if (woodSplashCalled < maxWoodSplashAllowed)
			{
				woodSplashCalled++;
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					freeSource.clip = GetResource(woodSplashName[Random.Range(0, woodSplashName.Length)]);
					freeSource.Play();
				}
			}
		}

		public void StickmanHit()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(playerHitName[Random.Range(0, playerHitName.Length)]);
				freeSource.Play();
			}
		}

		public void StoneBreak()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(stoneBrokeName[Random.Range(0, stoneBrokeName.Length)]);
				freeSource.Play();
			}
		}

		public void StoneSplash()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(stoneSplashName[Random.Range(0, stoneSplashName.Length)]);
				freeSource.Play();
			}
		}

		public void CrowScream()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(crowScreamName[Random.Range(0, crowScreamName.Length)]);
				freeSource.Play();
				freeSource = GetFreeSource();
				if (freeSource != null)
				{
					freeSource.clip = GetResource(crowScreamName[Random.Range(0, crowScreamName.Length)]);
					freeSource.PlayDelayed(0.5f);
				}
			}
		}

		public void PlayerBleed()
		{
			if (bloodSoundAllowed)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					freeSource.volume = 0.125f;
					freeSource.clip = GetResource(playerBleedName[Random.Range(0, playerBleedName.Length)]);
					freeSource.Play();
					bloodSoundAllowed = false;
				}
			}
			else
			{
				bloodSoundAllowed = true;
			}
		}

		public void PurchaseSound()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(purchaseSoundName);
				freeSource.Play();
			}
		}

		public void ButtonPress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(pressSoundName);
				freeSource.Play();
			}
		}

		public void UpgradePress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(catapultUpgradeName[Random.Range(0, catapultUpgradeName.Length)]);
				freeSource.Play();
			}
		}

		public void HelmetEquipPress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(helmetEquipName[Random.Range(0, helmetEquipName.Length)]);
				freeSource.Play();
			}
		}

		public void CatapultEquipPress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(catapultEquipName[Random.Range(0, catapultEquipName.Length)]);
				freeSource.Play();
			}
		}

		public void ProjectileEquipPress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(projectileEquipName[Random.Range(0, projectileEquipName.Length)]);
				freeSource.Play();
			}
		}

		public void ShieldEquipPress()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(shieldEquipName[Random.Range(0, shieldEquipName.Length)]);
				freeSource.Play();
			}
		}

		public void ShieldReact()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(shieldReactName[Random.Range(0, shieldReactName.Length)]);
				freeSource.Play();
			}
		}

		public void ShootThorns()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(projectileShootThornsName[Random.Range(0, projectileShootThornsName.Length)]);
				freeSource.Play();
			}
		}

		public void CrowHit()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(crowHitName[Random.Range(0, crowHitName.Length)]);
				freeSource.Play();
				CrowHitAfter();
			}
		}

		public void CrowHitAfter()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(crowHitAfterName[Random.Range(0, crowHitAfterName.Length)]);
				freeSource.PlayDelayed(0.5f);
			}
		}

		public void PlayerJump()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(playerJumpName[Random.Range(0, playerJumpName.Length)]);
				freeSource.Play();
			}
		}

		public void TowerMove(GameSides side)
		{
			towerSources[(int)side] = GetFreeSource();
			if (towerSources[(int)side] != null)
			{
				towerSources[(int)side].clip = GetResource(towerMoveName[Random.Range(0, towerMoveName.Length)]);
				towerSources[(int)side].Play();
			}
		}

		public void TowerStop(GameSides side)
		{
			if (towerSources[(int)side] != null)
			{
				towerSources[(int)side].Stop();
				towerSources[(int)side] = null;
			}
		}

		public void StickmanDeath()
		{
			AudioSource freeSource = GetFreeSource();
			if (freeSource != null)
			{
				freeSource.clip = GetResource(playerDeathName[Random.Range(0, playerDeathName.Length)]);
				freeSource.Play();
			}
		}

		public void AimingSound()
		{
			if (currentAimingSource == null)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					currentAimingSource = freeSource;
					currentAimingSound = GetResource(catapultAimingName[Random.Range(0, catapultAimingName.Length)]);
					currentAimingSource.clip = currentAimingSound;
					currentAimingSource.Play();
				}
			}
		}

		public void CatapultMoving()
		{
			if (currentMovingSource == null)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					currentMovingSource = freeSource;
					currentMovingSource.volume = 0.5f;
					currentMovingSource.loop = true;
					currentMovingSource.clip = GetResource(catapultMovingName[Random.Range(0, catapultMovingName.Length)]);
					currentMovingSource.Play();
				}
			}
		}

		public void StopCatapultMoving()
		{
			if (currentMovingSource != null)
			{
				currentMovingSource.Stop();
				currentMovingSource = null;
			}
		}

		public void BossCharge()
		{
			if (currentChargeSource == null)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					currentChargeSource = freeSource;
					currentChargeSource.volume = 0.5f;
					currentChargeSource.loop = true;
					currentChargeSource.clip = GetResource(catapultMovingName[Random.Range(0, catapultMovingName.Length)]);
					currentChargeSource.Play();
				}
			}
		}

		public void StopBossCharge()
		{
			if (currentChargeSource != null)
			{
				currentChargeSource.Stop();
				currentChargeSource = null;
			}
		}

		public AudioClip GetBombFireSound()
		{
			return GetResource(bombSoundFireName[Random.Range(0, bombSoundFireName.Length)]);
		}

		public AudioClip GetFireSound()
		{
			return GetResource(FireProjectileName[Random.Range(0, FireProjectileName.Length)]);
		}

		public void RegisterBombSound(AudioSource bomb)
		{
			bomb.clip = GetBombFireSound();
			bomb.volume = 0.75f;
			bomb.Play();
			bombSounds.Add(bomb);
		}

		public void UnregisterBombSound(AudioSource bomb)
		{
			bomb.Stop();
			bombSounds.Remove(bomb);
		}

		public void RegisterFireSound(AudioSource fire)
		{
			fire.clip = GetFireSound();
			fire.volume = 0.75f;
			fire.Play();
			fireSounds.Add(fire);
		}

		public void UnregisterFireSound(AudioSource fire)
		{
			fire.Stop();
			fireSounds.Remove(fire);
		}

		public void RegisterFireDebris(AudioSource source)
		{
			fireDebrisSounds.Add(source);
		}

		public void UnregisterFireDebris(AudioSource source)
		{
			fireDebrisSounds.Remove(source);
		}

		public void BossEncounterStart()
		{
			if (currentBossEncouter == null && PlayerPrefs.GetInt("Music") == 1 && bossIncomingName.Length > 0)
			{
				AudioSource freeSource = GetFreeSource();
				if (freeSource != null)
				{
					currentBossEncouter = freeSource;
					currentBossEncouter.loop = true;
					currentBossEncouter.clip = GetResource(bossIncomingName[Random.Range(0, bossIncomingName.Length)]);
					currentBossEncouter.Play();
				}
			}
		}

		public void BossEncounterEnd()
		{
			if (currentBossEncouter != null)
			{
				currentBossEncouter.Stop();
				currentBossEncouter = null;
			}
		}

		private void Update()
		{
			if (Time.timeScale == 0f && currentMovingSource != null)
			{
				currentMovingSource.volume = 0f;
			}
			else if (currentMovingSource != null && currentMovingSource.volume == 0f)
			{
				currentMovingSource.volume = 0.5f;
			}
			if (Time.timeScale == 0f && currentChargeSource != null)
			{
				currentChargeSource.volume = 0f;
			}
			else if (currentChargeSource != null && currentChargeSource.volume == 0f)
			{
				currentChargeSource.volume = 0.5f;
			}
			if (woodCalled <= 0 && woodSplashCalled <= 0)
			{
				return;
			}
			if (soundTick > 0.2f)
			{
				if (woodCalled > 0)
				{
					woodCalled--;
				}
				if (woodSplashCalled > 0)
				{
					woodSplashCalled--;
				}
				soundTick = 0f;
			}
			soundTick += Time.deltaTime;
		}

		public void PauseGame(bool pauseAll = false)
		{
			for (int i = 0; i < sourcePool.Length; i++)
			{
				if (sourcePool[i] != null && sourcePool[i].isPlaying)
				{
					sourcePool[i].Pause();
				}
			}
			if (pauseAll && musicEnabled)
			{
				musicDedicatedSource.Pause();
			}
			for (int j = 0; j < bombSounds.Count; j++)
			{
				if (bombSounds[j] != null)
				{
					bombSounds[j].Pause();
				}
			}
			for (int k = 0; k < fireSounds.Count; k++)
			{
				if (fireSounds[k] != null)
				{
					fireSounds[k].Pause();
				}
			}
			for (int l = 0; l < fireDebrisSounds.Count; l++)
			{
				if (fireDebrisSounds[l] != null)
				{
					fireDebrisSounds[l].Pause();
				}
			}
		}

		public void UnPauseGame()
		{
			if (musicEnabled)
			{
				musicDedicatedSource.UnPause();
			}
			if (!soundEnabled)
			{
				return;
			}
			for (int i = 0; i < sourcePool.Length; i++)
			{
				if (sourcePool[i] != null)
				{
					sourcePool[i].UnPause();
				}
			}
			for (int j = 0; j < bombSounds.Count; j++)
			{
				if (bombSounds[j] != null)
				{
					bombSounds[j].UnPause();
				}
			}
			for (int k = 0; k < fireSounds.Count; k++)
			{
				if (fireSounds[k] != null)
				{
					fireSounds[k].UnPause();
				}
			}
			for (int l = 0; l < fireDebrisSounds.Count; l++)
			{
				if (fireDebrisSounds[l] != null)
				{
					fireDebrisSounds[l].UnPause();
				}
			}
		}

		public void StopAllSounds()
		{
			for (int i = 0; i < sourcePool.Length; i++)
			{
				if (sourcePool[i] != null)
				{
					sourcePool[i].Stop();
				}
			}
			for (int j = 0; j < bombSounds.Count; j++)
			{
				if (bombSounds[j] != null)
				{
					bombSounds[j].Stop();
				}
			}
			for (int k = 0; k < fireSounds.Count; k++)
			{
				if (fireSounds[k] != null)
				{
					fireSounds[k].Stop();
				}
			}
			for (int l = 0; l < fireDebrisSounds.Count; l++)
			{
				if (fireDebrisSounds[l] != null)
				{
					fireDebrisSounds[l].Stop();
				}
			}
		}
	}
}
