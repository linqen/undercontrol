using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour {
	UIManager uiManager;
	int inputNumber;
	void Awake(){
		uiManager = UIManager.Instance;
		inputNumber = GetComponent<PlayerInput> ().inputNumber;
	}

	void Update(){
		if (Input.GetButtonDown (Inputs.Start + inputNumber)) {
			uiManager.PauseGame ();
		}
	}
}
