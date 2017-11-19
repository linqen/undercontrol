using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuManager : GenericSingletonClass<MenuManager> {

	public List<GameObject> menuPlayerSelector = new List<GameObject>();

	List<Sprite> availableStartKeys = new List<Sprite> ();
	UIManager uiManager;
	GameManager gameManager;
	AudioManager audioManager;
	GameObject pressStart;
	GameObject menuBackground;
	GameObject mainMenu;
	GameObject characterSelect;
	GameObject mapSelect;
	GameObject roundsSelect;
	GameObject countdown;
	GameObject laserAdvice;
	int possiblePlayers;
	new void Awake(){
		base.Awake ();
	}
	void Start () {
		uiManager = UIManager.Instance;
		gameManager = GameManager.Instance;
		audioManager = AudioManager.Instance;
		pressStart = transform.Find ("PressStart").gameObject;
		menuBackground = transform.Find ("GeneralBackground").gameObject;
		mainMenu = transform.Find ("MainMenu").gameObject;
		mapSelect = transform.Find ("MapSelect").gameObject;
		characterSelect = transform.Find ("CharacterSelect").gameObject;
		roundsSelect = transform.Find ("RoundsSelect").gameObject;
		countdown = transform.Find ("Countdown").gameObject;
		laserAdvice = transform.Find ("LaserAdvice").gameObject;
		gameManager.PressStartButton ();
		Cursor.visible = false;
	}

	public void StartPressed(){
		pressStart.SetActive (false);
		menuBackground.SetActive (true);
		mainMenu.SetActive (true);
		EventSystem.current.SetSelectedGameObject(mainMenu.transform.Find ("Options").Find("Play").gameObject);
	}

	public void CharacterSelectionFinished(){
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			menuPlayerSelector [i].GetComponent<Image> ().color = Color.black;
			menuPlayerSelector [i].transform.Find ("Character"+(i+1)).gameObject.SetActive (false);
		}
		roundsSelect.SetActive (true);
		GameObject roundSelected = roundsSelect.transform.Find ("PossibleRounds").Find ("5").gameObject;
		if (EventSystem.current.currentSelectedGameObject == roundSelected) {
			EventSystem.current.SetSelectedGameObject (null);
		}
		EventSystem.current.SetSelectedGameObject (roundSelected);
		StartCoroutine (gameManager.RoundSelection ());
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
		gameManager.GameStart(1,rounds);
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		characterSelect.SetActive (true);
		StartCoroutine(gameManager.CharSelection ());
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
			ClearSelectorsData ();
			StartCoroutine (gameManager.CharSelection ());
		}
		else if (characterSelect.activeSelf) {
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
			EventSystem.current.SetSelectedGameObject (null);
			EventSystem.current.SetSelectedGameObject(mainMenu.transform.Find ("Options").Find("Play").gameObject);
		}
	}

	public void BackToMain(){
		mainMenu.SetActive (true);
		menuBackground.SetActive (true);
		audioManager.MainMenuMusic ();
		EventSystem.current.SetSelectedGameObject(mainMenu.transform.Find ("Options").Find("Play").gameObject);
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
			keySelector.GetComponent<Image> ().sprite = ((Sprite)Resources.Load<Sprite> ("InputMap/" + playerInput.inputNumber));
			keySelector.SetActive (true);
			playerPreview.selected = true;
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
