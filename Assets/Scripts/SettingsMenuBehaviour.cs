using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InControl;

public class SettingsMenuBehaviour : MonoBehaviour {

	public Color highLightedColor;
	public Color normalColor;

	SettingsDB settingsDB;
	MenuManager menuManager;
	List<GameObject> actualList = new List<GameObject>();
	int actualPos=0;
	int lastPos=-1;
	bool onSettings;

	void Start(){
		menuManager = MenuManager.Instance;
		settingsDB = SettingsDB.Instance;
	}

	public void StartSettings (GameObject settings) {
		foreach (Transform child in settings.transform)
		{
			actualList.Add (child.gameObject);
			child.gameObject.SetActive (true);
		}
		if (actualList != null && actualList.Count != 0) {
			for (int i = 0; i < actualList.Count; i++) {
				actualList [i].SetActive (true);
			}
			StartCoroutine (SettingsCycle ());
		} else {
			Debug.Log ("Option List is null or empty");
		}
	}

	IEnumerator SettingsCycle(){
		yield return null;
		ResetValues ();
		HighLight (actualList [0]);
		onSettings = true;

		//Start cheking Input
		while(onSettings) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				if (InputManager.Devices [i].DPadUp.WasPressed) {
					//Move up
					if (actualPos > 0) {
						lastPos = actualPos;
						actualPos--;
						AkSoundEngine.PostEvent ("ChoosingMenuSoundUp", gameObject);
						HighLight (actualList [actualPos]);
					}
				}
				if (InputManager.Devices [i].DPadDown.WasPressed) {
					//Move Down
					if (actualPos < actualList.Count-1) {
						lastPos = actualPos;
						actualPos++;
						AkSoundEngine.PostEvent ("ChoosingMenuSoundDown", gameObject);
						HighLight (actualList [actualPos]);
					}
				}

				//Changing Settings
				if (InputManager.Devices [i].DPadRight.WasPressed) {
					if(actualList[actualPos].CompareTag("SettingConfig")){
						string key = actualList [actualPos].transform.Find ("Key").GetComponent<Text> ().text;
						bool wasChangeEffected = settingsDB.SetValue (key, 1);
						if(wasChangeEffected)
							AkSoundEngine.PostEvent ("ChoosingMenuSoundUp", gameObject);
						yield return null;
						actualList [actualPos].transform.Find ("Value").GetComponent<Text>().text = settingsDB.GetValue(key);
					}
				}
				if (InputManager.Devices [i].DPadLeft.WasPressed) {
						if(actualList[actualPos].CompareTag("SettingConfig")){
						string key = actualList [actualPos].transform.Find ("Key").GetComponent<Text> ().text;
						bool wasChangeEffected = settingsDB.SetValue (key, -1);
						if(wasChangeEffected)
							AkSoundEngine.PostEvent ("ChoosingMenuSoundDown", gameObject);
						yield return null;
						actualList [actualPos].transform.Find ("Value").GetComponent<Text>().text = settingsDB.GetValue (key);
					}
				}
				//Changing Settings

				if (InputManager.Devices[i].Action1.WasPressed) {
					yield return null;
					if (actualList [actualPos].CompareTag ("SettingButton")) {
						AkSoundEngine.PostEvent ("SelectedMenuSound", gameObject);
						actualList [actualPos].GetComponent<Button> ().onClick.Invoke ();
					} else if (actualList [actualPos].CompareTag ("SettingFlow")) {
						if (actualList [actualPos].transform.Find ("Back").GetComponent<Text> ().color == highLightedColor) {
							GoBack ();
						}
					}
				}
				if (InputManager.Devices[i].Action2.WasPressed) {
					yield return null;
					GoBack ();
				}
			}
			yield return null;
		}
		yield return null;
	}

	void HighLight(GameObject go){
		//HighLight actual Button or Setting
		if (go.CompareTag ("SettingButton")) {
			EventSystem.current.SetSelectedGameObject (null);
			EventSystem.current.SetSelectedGameObject (go);
		} else if (go.CompareTag ("SettingConfig")) {
			go.transform.Find ("Key").GetComponent<Text> ().color = highLightedColor;
			EventSystem.current.SetSelectedGameObject (null);
			if (lastPos != -1) {
				if (actualList [lastPos].CompareTag ("SettingConfig")) {
					actualList [lastPos].transform.Find ("Key").GetComponent<Text> ().color = normalColor;
				} else if (actualList [lastPos].CompareTag ("SettingFlow")) {
					actualList [lastPos].transform.Find ("Back").GetComponent<Text> ().color = normalColor;
				}
			}
		} else if (go.CompareTag ("SettingFlow")) {
			go.transform.Find ("Back").GetComponent<Text> ().color = highLightedColor;
			EventSystem.current.SetSelectedGameObject (null);
			if (lastPos != -1) {
				if (actualList [lastPos].CompareTag ("SettingConfig")) {
					actualList [lastPos].transform.Find ("Key").GetComponent<Text> ().color = normalColor;
				}
			}
		}

	}

	public void EnterSetting(){
		for (int i = 0; i < actualList.Count; i++) {
			actualList [i].GetComponent<Text> ().enabled = false;
		}
		Transform parent = actualList[actualPos].transform;
		actualList.Clear ();
		foreach (Transform child in parent)
		{
			actualList.Add (child.gameObject);
			child.gameObject.SetActive (true);
			if (child.CompareTag ("SettingConfig")) {
				child.Find ("Value").GetComponent<Text> ().text = settingsDB.GetValue (child.Find ("Key").GetComponent<Text> ().text);
			}
		}
		ResetValues ();
		HighLight (actualList [actualPos]);
	}
	void GoBack(){
		//Clear Colors
		if (actualList [actualPos].CompareTag ("SettingConfig")) {
			actualList [actualPos].transform.Find ("Key").GetComponent<Text> ().color = normalColor;
		} else if (actualList [actualPos].CompareTag ("SettingFlow")) {
			actualList [actualPos].transform.Find ("Back").GetComponent<Text> ().color = normalColor;
		}

		//Go Last Menu
		Transform parent = actualList[0].transform.parent;
		for (int i = 0; i < actualList.Count; i++) {
			actualList [i].SetActive (false);
		}
		actualList.Clear ();

		if (parent.CompareTag ("SettingMenu")) {
			ResetValues ();
			onSettings = false;
			menuManager.GoBack ();
		} else {
			foreach (Transform child in parent.parent) {
				actualList.Add (child.gameObject);
				child.GetComponent<Text> ().enabled = true;
				Button hasButton = child.GetComponent<Button> ();
				if (hasButton != null) {
					hasButton.enabled = false;
					hasButton.enabled = true;
				}
			}
			ResetValues ();
			HighLight (actualList [actualPos]);
		}

	}
	void ResetValues(){
		actualPos = 0;
		lastPos = -1;
	}
}
