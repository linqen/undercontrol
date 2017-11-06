using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class AudioManager : GenericSingletonClass<AudioManager> {
	public int effectsSourcesPoolLength;
	public float minPitchValue;
	public float maxPitchValue;
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
