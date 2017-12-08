using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerUp{Shield,HalfGrenadeCD,NoGravityGrenade}

public class PowerUpBehaviour : MonoBehaviour {

	public PowerUp powerUp;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag.Equals ("Player")) {
			switch (powerUp) {
			case PowerUp.Shield:
				col.GetComponent<PlayerLife> ().RecoverShield ();
				Debug.Log ("Shield");
				break;
			case PowerUp.HalfGrenadeCD:
				col.GetComponent<GrenadeThrowing> ().ReduceCooldown (2, 5.0f);
				Debug.Log ("Half CD");
				break;
			case PowerUp.NoGravityGrenade:
				col.GetComponent<GrenadeThrowing> ().NoGravityGrenade (5.0f);
				Debug.Log ("No gravity");
				break;
			}
			gameObject.SetActive (false);
		}
	}
}
