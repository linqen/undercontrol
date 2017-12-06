using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class GrenadeThrowing : MonoBehaviour {

	public GameObject grenadePrefab;
	public float grenadeCooldown;
	public float horizontalForce;
	public float verticalForce;

	private float localGrenadeCooldown;
	private PlayerMovement pMovement;
	private int inputNumber;
	private float currentCooldownGrenade;
	private int playerNumber;
	private int characterNumber;
	private Animator animator;
	private Coroutine reduceCDCoroutine=null;

	void Awake () {
		pMovement = GetComponent<PlayerMovement> ();
		inputNumber = GetComponent<PlayerInput> ().inputNumber;
		PlayerPreview pp = GetComponent<PlayerPreview> (); 
		playerNumber = pp.playerNumber;
		characterNumber = pp.charPreviewPos;
		animator = GetComponent<Animator> ();
	}

	void Start(){
		localGrenadeCooldown = grenadeCooldown;
	}

	void OnDisable(){
		localGrenadeCooldown = grenadeCooldown;
	}

	public void ReduceCooldown(int divisor,float time){
		localGrenadeCooldown /= divisor;
		if (reduceCDCoroutine != null) {
			StopCoroutine (reduceCDCoroutine);
		}
		reduceCDCoroutine = StartCoroutine (RecoverOriginalCooldown (time));
	}

	IEnumerator RecoverOriginalCooldown(float time){
		yield return new WaitForSeconds (time);
		localGrenadeCooldown = grenadeCooldown;
		reduceCDCoroutine = null;
	}

	void Update () {
		currentCooldownGrenade += Time.deltaTime;
		if (InputManager.Devices[inputNumber].Action2.IsPressed||InputManager.Devices[inputNumber].Action2.WasReleased && Time.timeScale!=0) {
			if (InputManager.Devices[inputNumber].Action2.WasReleased && currentCooldownGrenade>=localGrenadeCooldown) {
				Vector3 pos = new Vector3 ( transform.position.x+pMovement.LastDirection.x/3,
					transform.position.y,
					transform.position.z);
				GameObject grenade = Instantiate (grenadePrefab, pos, transform.rotation);
				GrenadeBehaviour gb = grenade.GetComponent<GrenadeBehaviour> ();
				gb.ThrowedByPlayerNumber = playerNumber;
				gb.GrenadeOfCharacterNumber = characterNumber;

				grenade.GetComponent<Rigidbody2D>().AddForce (new Vector2(pMovement.LastDirection.x*horizontalForce,pMovement.VerticalAxis*verticalForce), ForceMode2D.Impulse);	
				currentCooldownGrenade = 0;
				animator.SetTrigger ("Throw");
			}
			return;
		}
	}
}
