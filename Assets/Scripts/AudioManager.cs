using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class AudioManager : GenericSingletonClass<AudioManager> {
	public int effectsSourcesPoolLength;
	public float minPitchValue;
	public float maxPitchValue;
	public AudioClip mainMenuMusic;
	public AudioClip inGameMusic;
	public AudioClip[] inGameLaserMusic;
	public AudioClip[] selectedPlayerSounds;
	public AudioClip[] choosingPlayerSounds;
	public AudioClip[] jumpingSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] explosionSounds;
	public AudioClip[] hitRoofSounds;
	public AudioClip[] touchFloorSounds;

	public AudioSource musicSource;

	private List<AudioSource> efxSources = new List<AudioSource> ();
	private int actualEfxSourcePos = 0;

	new void Awake(){
		base.Awake ();
	}

	void Start () {
		musicSource.loop = true;
		for (int i = 0; i < effectsSourcesPoolLength; i++) {
			efxSources.Add (gameObject.AddComponent (typeof(AudioSource)) as AudioSource);
			efxSources [i].playOnAwake = false;
		}
	}

	private void RandomEfx(AudioClip[] sounds){
		float randomPitch = Random.Range (minPitchValue, maxPitchValue);
		int randomSoundPos = Random.Range (0, sounds.Length);
		if (actualEfxSourcePos == effectsSourcesPoolLength) {actualEfxSourcePos = 0;}
		efxSources [actualEfxSourcePos].pitch = randomPitch;
		efxSources[actualEfxSourcePos].PlayOneShot(sounds[randomSoundPos]);
		actualEfxSourcePos++;
	}

	public void MainMenuMusic(){
		musicSource.Stop ();
		musicSource.clip = mainMenuMusic;
		musicSource.Play ();
	}

	public void InGameMusic(){
		musicSource.Stop ();
		musicSource.clip = inGameMusic;
		musicSource.Play ();
	}
	public void InGameLaserMusic(){
		float currentTime = musicSource.time;
		musicSource.Stop ();
		musicSource.clip = inGameLaserMusic [Random.Range (0, inGameLaserMusic.Length)];
		musicSource.time = currentTime;
		musicSource.Play ();
	}

	public void PlayerJumping(){
		RandomEfx (jumpingSounds);
	}

	public void PlayerWalking(){
		RandomEfx (walkSounds);
	}

	public void PlayerHitRoof(){
		RandomEfx (hitRoofSounds);
	}

	public void PlayerTouchFloor(){
		RandomEfx (touchFloorSounds);
	}

	public void GrenadeExplode(){
		RandomEfx (explosionSounds);
	}
	public void SelectedSound(){
		RandomEfx (selectedPlayerSounds);
	}
	public void ChoosingSound(){
		RandomEfx (choosingPlayerSounds);
	}
}
