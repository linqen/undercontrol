using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PauseGame : MonoBehaviour {
	UIManager uiManager;
	int inputNumber;
	void Awake(){
		uiManager = UIManager.Instance;
		inputNumber = GetComponent<PlayerInput> ().inputNumber;
	}

	void Update(){
		if (InputManager.Devices[inputNumber].GetControl(InputControlType.Start).WasPressed) {
			uiManager.PauseGame ();
		}
	}
}
