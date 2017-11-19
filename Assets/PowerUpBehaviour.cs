using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerUp{Shield,HalfGrenadeCD}

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
			}
			gameObject.SetActive (false);
		}
	}
}
