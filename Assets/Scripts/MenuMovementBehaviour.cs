using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuMovementBehaviour : MonoBehaviour {

	List<GameObject> mainMenuOptions = new List<GameObject>();
	List<GameObject> possibleRoundsToSelect = new List<GameObject>();
	int actualPos=0;
	bool mainMenuNavigation = false;
	bool roundSelection = false;

	void Start () {
		foreach (Transform child in transform.Find ("MainMenu").Find ("Options")) {
			if (child.gameObject.activeSelf) {
				mainMenuOptions.Add (child.gameObject);
			}
		}
		foreach (Transform child in transform.Find ("RoundsSelect").Find ("PossibleRounds")) {
			if (child.gameObject.activeSelf) {
				possibleRoundsToSelect.Add (child.gameObject);
			}
		}
	}

	public void MainMenuOptionsNavigation(int firstSelectedPos){
		actualPos = firstSelectedPos;
		EventSystem.current.SetSelectedGameObject (null);
		EventSystem.current.SetSelectedGameObject (mainMenuOptions [actualPos]);
		mainMenuNavigation = true;
		StartCoroutine (MainMenuSelection ());
	}
	public void RoundSelectionNavigation(int firstSelectedPos){
		actualPos = firstSelectedPos;
		EventSystem.current.SetSelectedGameObject (null);
		EventSystem.current.SetSelectedGameObject (possibleRoundsToSelect [actualPos]);
		roundSelection = true;
		StartCoroutine (RoundSelection ());
	}

	IEnumerator MainMenuSelection(){
		yield return null;
		while(mainMenuNavigation) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				if (InputManager.Devices [i].DPadUp.WasPressed) {
					//Move up
					if (actualPos > 0) {
						actualPos--;
						EventSystem.current.SetSelectedGameObject (mainMenuOptions [actualPos]);
					}
				}

				if (InputManager.Devices [i].DPadDown.WasPressed) {
					//Move Down
					if (actualPos < mainMenuOptions.Count-1) {
						actualPos++;
						EventSystem.current.SetSelectedGameObject (mainMenuOptions [actualPos]);
					}
				}

				if (InputManager.Devices [i].Action1.WasPressed) {
					actualPos = 0;
					mainMenuNavigation = false;
					mainMenuOptions [actualPos].GetComponent<Button> ().onClick.Invoke ();
				}

			}
			yield return null;
		}
	}

	IEnumerator RoundSelection(){
		yield return null;
		while (roundSelection) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				if (InputManager.Devices [i].DPadLeft.WasPressed) {
					if (actualPos > 0) {
						actualPos--;
						EventSystem.current.SetSelectedGameObject (possibleRoundsToSelect [actualPos]);
					}
				}

				if (InputManager.Devices [i].DPadRight.WasPressed) {
					if (actualPos < mainMenuOptions.Count-1) {
						actualPos++;
						EventSystem.current.SetSelectedGameObject (possibleRoundsToSelect [actualPos]);
					}
				}

				if (InputManager.Devices [i].Action1.WasPressed) {
					actualPos = 0;
					roundSelection = false;
					possibleRoundsToSelect [actualPos].GetComponent<Button> ().onClick.Invoke ();
				}

			}
			yield return null;
		}
	}
}
