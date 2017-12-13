using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InControl;
public class MenuManager : GenericSingletonClass<MenuManager> {

	public List<GameObject> menuPlayerSelector = new List<GameObject>();

	List<Sprite> availableStartKeys = new List<Sprite> ();
	MenuMovementBehaviour menuMovementBehaviour;
	UIManager uiManager;
	GameManager gameManager;
	GameObject pressStart;
	GameObject menuBackground;
	GameObject mainMenu;
	GameObject characterSelect;
	GameObject credits;
	GameObject mapSelect;
	GameObject roundsSelect;
	GameObject countdown;
	GameObject laserAdvice;
	int possiblePlayers;
	new void Awake(){
		base.Awake ();
		menuMovementBehaviour = GetComponent<MenuMovementBehaviour> ();
	}
	void Start () {
		uiManager = UIManager.Instance;
		gameManager = GameManager.Instance;
		pressStart = transform.Find ("PressStart").gameObject;
		menuBackground = transform.Find ("GeneralBackground").gameObject;
		mainMenu = transform.Find ("MainMenu").gameObject;
		credits = transform.Find ("Credits").gameObject;
		mapSelect = transform.Find ("MapSelect").gameObject;
		characterSelect = transform.Find ("CharacterSelect").gameObject;
		roundsSelect = transform.Find ("RoundsSelect").gameObject;
		countdown = transform.Find ("Countdown").gameObject;
		laserAdvice = transform.Find ("LaserAdvice").gameObject;
		gameManager.PressStartButton ();
		AkSoundEngine.SetSwitch ("MainMenuMusic", "PressStartMenu",gameObject);
		AkSoundEngine.PostEvent ("Menu_music", gameObject);
		Cursor.visible = false;
	}

	public void StartPressed(){
		pressStart.SetActive (false);
		menuBackground.SetActive (true);
		mainMenu.SetActive (true);
		menuMovementBehaviour.MainMenuOptionsNavigation(0);
		AkSoundEngine.SetSwitch ("MainMenuMusic", "MainMenu",gameObject);
	}

	public void CharacterSelectionFinished(){
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			menuPlayerSelector [i].GetComponent<Image> ().color = Color.black;
			menuPlayerSelector [i].transform.Find ("Character"+(i+1)).gameObject.SetActive (false);
		}
		roundsSelect.SetActive (true);
		menuMovementBehaviour.RoundSelectionNavigation (1);
		StartCoroutine (gameManager.RoundSelection ());
		AkSoundEngine.SetSwitch ("MainMenuMusic", "KillSelectionMenu",gameObject);
	}
	public void RoundsSelect(){
		//Clear data
		ClearSelectorsData();
		//
		int rounds = int.Parse(EventSystem.current.currentSelectedGameObject.name);
		characterSelect.SetActive (false);
		menuBackground.SetActive (false);
		roundsSelect.SetActive (false);
		StopCoroutine (gameManager.RoundSelection ());

		AkSoundEngine.SetState ("StateOfMusic", "InGame");
		AkSoundEngine.PostEvent ("InGameMusic", gameObject);
		gameManager.GameStart(1,rounds);
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		characterSelect.SetActive (true);
		menuMovementBehaviour.StopMainMenuSelection ();
		StartCoroutine(gameManager.CharSelection ());
		AkSoundEngine.SetSwitch ("MainMenuMusic", "CharacterSelectionMenu",gameObject);
	}

	private void ClearSelectorsData(){
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			menuPlayerSelector [i].GetComponent<Image> ().color = Color.white;
			menuPlayerSelector [i].transform.Find ("Character"+(i+1)).gameObject.SetActive (true);
			menuPlayerSelector [i].transform.Find ("Selected").gameObject.SetActive (false);
			menuPlayerSelector [i].GetComponent<SelectorBehaviour> ().ClearValues ();
			menuPlayerSelector [i].transform.Find ("KeyMap").gameObject.SetActive (false);
		}
	}

	public void GoBack(){
		if (roundsSelect.activeSelf) {
			characterSelect.SetActive (true);
			roundsSelect.SetActive (false);
			menuMovementBehaviour.StopRoundSelection ();
			ClearSelectorsData ();
			AkSoundEngine.SetSwitch ("MainMenuMusic", "CharacterSelectionMenu",gameObject);
			StartCoroutine (gameManager.CharSelection ());
		} else if (characterSelect.activeSelf) {
			for (int i = 0; i < menuPlayerSelector.Count; i++) {
				menuPlayerSelector [i].transform.Find ("Selected").gameObject.SetActive (false);
				menuPlayerSelector [i].GetComponent<SelectorBehaviour> ().ClearValues ();
				menuPlayerSelector [i].transform.Find ("KeyMap").gameObject.SetActive (false);
			}
			mainMenu.SetActive (true);
			characterSelect.SetActive (false);
			gameManager.StopCharSelection ();
			for (int i = 0; i < menuPlayerSelector.Count; i++) {
				menuPlayerSelector [i].GetComponent<SelectorBehaviour> ().ClearValues ();
			}
			menuMovementBehaviour.MainMenuOptionsNavigation (0);
			AkSoundEngine.SetSwitch ("MainMenuMusic", "MainMenu",gameObject);
		} else if (credits.activeSelf) {
			mainMenu.SetActive (true);
			credits.SetActive (false);
			AkSoundEngine.SetSwitch ("MainMenuMusic", "MainMenu",gameObject);
			menuMovementBehaviour.MainMenuOptionsNavigation (1);
		}
	}

	public void BackToMain(){
		mainMenu.SetActive (true);
		menuBackground.SetActive (true);
		AkSoundEngine.StopAll (gameObject);
		AkSoundEngine.SetSwitch ("MainMenuMusic", "MainMenu",gameObject);
		AkSoundEngine.SetState ("StateOfMusic", "Menu");
		AkSoundEngine.PostEvent ("Menu_music", gameObject);
		menuMovementBehaviour.MainMenuOptionsNavigation(0);
	}
	public void ShowCountdown(float time){
		StartCoroutine (Countdown (time));
	}
	private IEnumerator Countdown(float time){
		float currentTime = time;
		countdown.SetActive (true);
		Text countdownText = countdown.GetComponent<Text> ();
		while (currentTime >= 0) {
			currentTime -= Time.unscaledDeltaTime;
			int rounded = Mathf.RoundToInt (currentTime);
			if (rounded == 0) {
				countdownText.text = "GO!";
			} else {
				countdownText.text = rounded.ToString ();
			}
			yield return null;
		}
		countdown.SetActive (false);
	}
	public void GoNextPreview(PlayerPreview playerPreview){
		int currentPos = playerPreview.charPreviewPos;
		if(currentPos!=0){
			menuPlayerSelector [currentPos - 1].GetComponent<SelectorBehaviour> ().RemoveSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
		}
		currentPos++;
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			if (currentPos > menuPlayerSelector.Count) {
				currentPos = 1;
			}
			SelectorBehaviour selector = menuPlayerSelector [currentPos - 1].GetComponent<SelectorBehaviour> ();
			if (!selector.selected) {
				selector.AddSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
				playerPreview.charPreviewPos = currentPos;
				AkSoundEngine.PostEvent ("ChoosingPlayerSoundRight", gameObject);
				break;
			} else {
				currentPos++;
			}
		}
	}

	public void GoPreviousPreview(PlayerPreview playerPreview){
		int currentPos = playerPreview.charPreviewPos;
		if(currentPos!=0){
			menuPlayerSelector [currentPos - 1].GetComponent<SelectorBehaviour> ().RemoveSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
		}
		currentPos--;
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			if (currentPos <= 0) {
				currentPos = menuPlayerSelector.Count;
			}
			SelectorBehaviour selector = menuPlayerSelector [currentPos - 1].GetComponent<SelectorBehaviour> ();
			if (!selector.selected) {
				selector.AddSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
				playerPreview.charPreviewPos = currentPos;
				AkSoundEngine.PostEvent ("ChoosingPlayerSoundLeft", gameObject);
				break;
			} else {
				currentPos--;
			}
		}
	}

	public void ShowLasersAdvice(){
		StartCoroutine (ShowLasersAdv ());
	}
	private IEnumerator ShowLasersAdv(){
		laserAdvice.SetActive (true);
		yield return new WaitForSeconds (0.25f);
		laserAdvice.SetActive (false);
		yield return new WaitForSeconds (0.25f);
		laserAdvice.SetActive (true);
		yield return new WaitForSeconds (0.25f);
		laserAdvice.SetActive (false);
	}


	public void Credits(){
		mainMenu.SetActive (false);
		credits.SetActive (true);
		menuMovementBehaviour.StopMainMenuSelection ();
		AkSoundEngine.SetSwitch ("MainMenuMusic", "CreditsMenu",gameObject);

		StartCoroutine (WaitToExitCredits());
	}
	private IEnumerator WaitToExitCredits(){
		bool loop = true;
		while (loop) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				if (InputManager.Devices [i].AnyButton.WasPressed) {
					GoBack ();
					loop = false;
				}
			}
			yield return null;
		}
	}

	public void StopLasersAdvice(){
		StopCoroutine (ShowLasersAdv ());
		laserAdvice.SetActive (false);
	}
	public bool SelectPreview(PlayerPreview playerPreview, PlayerInput playerInput){
		SelectorBehaviour selector = menuPlayerSelector [playerPreview.charPreviewPos-1].GetComponent<SelectorBehaviour> ();
		if (!selector.selected) {
			selector.SelectSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
			selector.transform.Find ("Selected").gameObject.SetActive (true);
			GameObject keySelector = selector.transform.Find ("KeyMap").gameObject;
			keySelector.GetComponent<Image> ().sprite = ((Sprite)Resources.Load<Sprite> ("InputMap/" + InputManager.Devices[playerInput.inputNumber].Name));
			keySelector.SetActive (true);
			playerPreview.selected = true;
			AkSoundEngine.PostEvent ("SelectedPlayerSound", gameObject);
			return true;
		} else {
			//Reproducir sonido
			return false;
		}
	}

	public void SetPossiblePlayers(int possiblePlayers){
		this.possiblePlayers = possiblePlayers;
		for (int i = 1; i < possiblePlayers+1; i++) {
			availableStartKeys.Add ((Sprite)Resources.Load<Sprite> ("Keys/" + Inputs.Start+i));
		}
	}

	public void ExitGame(){
		Application.Quit ();
	}
}
