using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrowing : MonoBehaviour {

	public GameObject grenadePrefab;
	public float grenadeCooldown;

	private PlayerMovement pMovement;
	private PlayerInput input;
	private float currentCooldownGrenade;
	private int playerNumber;
	private Animator animator;

	void Awake () {
		pMovement = GetComponent<PlayerMovement> ();
		input = GetComponent<PlayerInput> ();
		playerNumber = GetComponent<PlayerPreview> ().playerNumber;
		animator = GetComponent<Animator> ();
	}
	
	void Update () {
		currentCooldownGrenade += Time.deltaTime;
		if (Input.GetButton (input.Fire)||Input.GetButtonUp (input.Fire)) {
			if (Input.GetButtonUp(input.Fire)&&currentCooldownGrenade>=grenadeCooldown) {
				Vector3 pos = new Vector3 ( transform.position.x+pMovement.HorizontalAxis,
					transform.position.y+1,
					transform.position.z);
				GameObject grenade = Instantiate (grenadePrefab, pos, transform.rotation);
				grenade.GetComponent<GrenadeBehaviour> ().ThrowedByPlayerNumber = playerNumber;
				grenade.GetComponent<Rigidbody2D>().AddForce (new Vector2(pMovement.LastDirection.x*3,pMovement.VerticalAxis+2), ForceMode2D.Impulse);	
				currentCooldownGrenade = 0;
				animator.SetTrigger ("Throw");
			}
			return;
		}
	}
}
