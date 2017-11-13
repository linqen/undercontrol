using System.Collections.Generic;
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
		characterSelect.SetActive (false);
		for (int i = 0; i < menuPlayerSelector.Count; i++) {
			menuPlayerSelector [i].GetComponent<SelectorBehaviour> ().ClearValues ();
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
		int rounds = int.Parse(EventSystem.current.currentSelectedGameObject.name);
		menuBackground.SetActive (false);
		roundsSelect.SetActive (false);
		StopCoroutine (gameManager.RoundSelection ());
		gameManager.GameStart("Map1",rounds);
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		characterSelect.SetActive (true);
		StartCoroutine(gameManager.CharSelection ());
	}

	public void GoBack(){
		if (characterSelect.activeSelf) {
			mainMenu.SetActive (true);
			characterSelect.SetActive (false);
			gameManager.StopCharSelection ();
			for (int i = 0; i < menuPlayerSelector.Count; i++) {
				menuPlayerSelector [i].GetComponent<SelectorBehaviour> ().ClearValues ();
			}
			EventSystem.current.SetSelectedGameObject (null);
			EventSystem.current.SetSelectedGameObject(mainMenu.transform.Find ("Options").Find("Play").gameObject);
		} else if (roundsSelect.activeSelf) {
			characterSelect.SetActive (true);
			roundsSelect.SetActive (false);
			StartCoroutine (gameManager.CharSelection ());
		}
	}

	public void BackToMain(){
		mainMenu.SetActive (true);
		menuBackground.SetActive (true);
		audioManager.MainMenuMusic ();
		EventSystem.current.SetSelectedGameObject(mainMenu.transform.Find ("Options").Find("Play").gameObject);
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

	public bool SelectPreview(PlayerPreview playerPreview, PlayerInput playerInput){
		SelectorBehaviour selector = menuPlayerSelector [playerPreview.charPreviewPos-1].GetComponent<SelectorBehaviour> ();
		if (!selector.selected) {
			selector.SelectSelector ((Sprite)Resources.Load<Sprite> ("Selectors/" + playerPreview.playerNumber));
			selector.transform.Find ("Selected").gameObject.SetActive (true);
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
