using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour {
	LasersManager laserManager;
	void Start(){
		laserManager = GetComponentInParent(typeof(LasersManager)) as LasersManager;
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.tag.Equals("LevelLimit")){
			laserManager.Invert ();
		}
	}
}