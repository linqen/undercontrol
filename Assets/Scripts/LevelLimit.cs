using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLimit : MonoBehaviour {
	public int shootPower;

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag.Equals("Player")){
			col.gameObject.GetComponent<PlayerLife>().NotifyHit(shootPower);
		}
	}
}
