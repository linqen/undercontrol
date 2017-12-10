using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerExternalCollider : MonoBehaviour {
	ArrowPointer parent;
	void Awake(){
		parent = GetComponentInParent<ArrowPointer> ();
	}

	void OnTriggerEnter2D(Collider2D col){
		parent.ExternalOnTriggerEnter2D (col);
	}
}
