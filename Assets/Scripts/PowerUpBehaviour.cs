using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerUp{Shield,HalfGrenadeCD,NoGravityGrenade,FasterMovement}

public class PowerUpBehaviour : MonoBehaviour {

	public PowerUp powerUp;
	public float halfCDTime=5.0f;
	public float noGravityGrenadeTime=5.0f;
	public float fasterMovementMultiplier=1.7f;
	public float fasterMovementTime=5.0f;


	void OnTriggerEnter2D(Collider2D col){
		if (col.tag.Equals ("Player")) {
			switch (powerUp) {
			case PowerUp.Shield:
				col.GetComponent<PlayerLife> ().RecoverShield ();
				Debug.Log ("Shield");
				break;
			case PowerUp.HalfGrenadeCD:
				col.GetComponent<GrenadeThrowing> ().ReduceCooldown (2, halfCDTime);
				Debug.Log ("Half CD");
				break;
			case PowerUp.NoGravityGrenade:
				col.GetComponent<GrenadeThrowing> ().NoGravityGrenade (noGravityGrenadeTime);
				Debug.Log ("No gravity");
				break;
			case PowerUp.FasterMovement:
				col.GetComponent<PlayerMovement> ().FasterMovement(fasterMovementMultiplier, fasterMovementTime);
				Debug.Log ("Faster Movement");
				break;
			}
			gameObject.SetActive (false);
		}
	}
}
