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
	public AudioClip inGameLaserSound;
	public AudioClip selectPlayerSound;
	public AudioClip choosingPlayerSoundsLeft;
	public AudioClip choosingPlayerSoundsRight;
	public AudioClip selectMenuSound;
	public AudioClip choosingMenuUp;
	public AudioClip choosingMenuDown;
	public AudioClip[] jumpingSounds;
	public AudioClip[] walkSounds;
	public AudioClip[] explosionSounds;
	public AudioClip[] hitRoofSounds;
	public AudioClip[] touchFloorSounds;
	public AudioClip[] deathSounds;

	public AudioSource musicSource;
	private AudioSource laserAudioEfx;
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
		laserAudioEfx = gameObject.AddComponent<AudioSource> ();
	}

	private void RandomEfx(AudioClip[] sounds){
		float randomPitch = Random.Range (minPitchValue, maxPitchValue);
		int randomSoundPos = Random.Range (0, sounds.Length);
		if (actualEfxSourcePos == effectsSourcesPoolLength) {actualEfxSourcePos = 0;}
		efxSources [actualEfxSourcePos].pitch = randomPitch;
		efxSources[actualEfxSourcePos].PlayOneShot(sounds[randomSoundPos]);
		actualEfxSourcePos++;
	}
	private void RandomEfx(AudioClip sound){
		float randomPitch = Random.Range (minPitchValue, maxPitchValue);
		if (actualEfxSourcePos == effectsSourcesPoolLength) {actualEfxSourcePos = 0;}
		efxSources [actualEfxSourcePos].pitch = randomPitch;
		efxSources[actualEfxSourcePos].PlayOneShot(sound);
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
	public void StartInGameLaserSound(){
		laserAudioEfx.pitch = Random.Range (minPitchValue, maxPitchValue);
		laserAudioEfx.clip = inGameLaserSound;
		laserAudioEfx.Play ();
	}
	public void StopInGameLaserSound(){
		laserAudioEfx.Stop ();
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

	public void DeathSound(){
		RandomEfx (deathSounds);
	}

	public void SelectedPlayerSound(){
		RandomEfx (selectPlayerSound);
	}
	public void ChoosingPlayerSoundLeft(){
		RandomEfx (choosingPlayerSoundsRight);
	}
	public void ChoosingPlayerSoundRight(){
		RandomEfx (choosingPlayerSoundsRight);
	}

	public void SelectedMenuSound(){
		RandomEfx (selectMenuSound);
	}
	public void ChoosingMenuSoundUp(){
		RandomEfx (choosingMenuUp);
	}
	public void ChoosingMenuSoundDown(){
		RandomEfx (choosingMenuDown);
	}
}
