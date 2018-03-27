using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDB : GenericSingletonClass<SettingsDB>{
	Dictionary<string,bool> isKeyBoolean = new Dictionary<string,bool>();

	int resolutionsLength=0;
	int actualResolutionPos=0;
	MenuManager menuManager;

	new void Awake(){
		base.Awake ();
	}

	void Start(){
		menuManager = MenuManager.Instance;
		isKeyBoolean.Add ("Resolution", false);
		isKeyBoolean.Add ("Quality", false);
		isKeyBoolean.Add ("Music", true);
		isKeyBoolean.Add ("FX", true);

	}

	/// <summary>
	/// Checks if preference exist.
	/// </summary>
	/// <param name="prefName">Preference name.</param>
	/// <param name="defaultValue">Default value in case that the preference didn't exist.</param>
	void CheckIfPrefExist(string prefName, int defaultValue){
		if (!PlayerPrefs.HasKey (prefName)) {
			PlayerPrefs.SetInt (prefName, defaultValue);
		}
	}

	public void Initialize(){
		resolutionsLength = Screen.resolutions.Length;
		for (int i = 0; i < resolutionsLength; i++) {
			if (Screen.resolutions [i].Equals(Screen.currentResolution)) {
				actualResolutionPos = i;
			}
		}

		if (Application.isEditor) //Does application is running in editor?
		{
			PlayerPrefs.DeleteAll ();
		}

		CheckIfPrefExist ("Resolution", actualResolutionPos);
		CheckIfPrefExist ("Quality", QualitySettings.GetQualityLevel ());
		CheckIfPrefExist ("Music", 1);
		CheckIfPrefExist ("FX", 1);


		int boolean = PlayerPrefs.GetInt ("Music");
		if (boolean == 1) {
			Camera.main.GetComponent<AkAudioListener> ().enabled = true;
		} else {
			Camera.main.GetComponent<AkAudioListener> ().enabled = false;
		}

		boolean = PlayerPrefs.GetInt ("FX");
		if (boolean == 1) {
			//Enable FX
		} else {
			//Disable FX
		}

	}

	private string SettingValueToString(string key){
		string returnValue = "";
		switch (key) {
		case "Resolution":
			returnValue = Screen.currentResolution.ToString ().Split(new char[]{'@'})[0];
			break;
		case "Quality":
			returnValue = QualitySettings.names [QualitySettings.GetQualityLevel ()];
			break;
		default:
			//Used for PlayerPrefs that are 1 or 0, (ON or OFF)
			if (PlayerPrefs.GetInt (key) == 1) {
				returnValue = "On";
			} else if (PlayerPrefs.GetInt (key) == 0){
				returnValue = "Off";
			}
			break;
		}
		return returnValue;
	}

	public string GetValue(string key){
		string returnValue = "";
		if (PlayerPrefs.HasKey (key)) {
			returnValue = SettingValueToString (key);
		}
		return returnValue;
	}

	/// <summary>
	/// Sets the value.
	/// </summary>
	/// <returns>True if change affect the actual config.</returns>
	/// <param name="key">Key.</param>
	/// <param name="orientation">orientation -1 is to get value at left, 1 to get the value at right</param>
	public bool SetValue(string key,int orientation){
		if ((orientation == -1 || orientation == 1) && PlayerPrefs.HasKey (key)) {
			bool isBoolean;
			isKeyBoolean.TryGetValue (key, out isBoolean);
			if (isBoolean) {
				bool boolValue;
				if (PlayerPrefs.GetInt (key) == 1) {
					PlayerPrefs.SetInt (key, 0);
					boolValue = false;
				} else {
					PlayerPrefs.SetInt (key, 1);
					boolValue = true;
				}
				switch (key) {
				case "Music":
					Camera.main.GetComponent<AkAudioListener> ().enabled = boolValue;
					break;
				case "FX":
					//DisableFX
					break;
				}
			} else {
				switch (key) {
				case "Resolution":
					actualResolutionPos += orientation;
					if (actualResolutionPos >= resolutionsLength) {
						actualResolutionPos = 0;
					} else if (actualResolutionPos < 0) {
						actualResolutionPos = resolutionsLength - 1;
					}
					Resolution newResolution = Screen.resolutions [actualResolutionPos];
					Screen.SetResolution (newResolution.width, newResolution.height, true);
					break;
				case "Quality":
					int actualLevel = QualitySettings.GetQualityLevel ();
					if (orientation == 1) {
						QualitySettings.IncreaseLevel (true);
					} else {
						QualitySettings.DecreaseLevel (true);
					}
					if (actualLevel == QualitySettings.GetQualityLevel ())
						return false;
					break;
				}
			}
		}
		return true;
	}

}
