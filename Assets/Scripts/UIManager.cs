using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InControl;
public class UIManager : GenericSingletonClass<UIManager> {

	public Sprite[] charactersPreviews;
	public float timeBetweenCountingScore;

	private bool isGameStarted = false;
	private GameManager gameManager;
	private MenuManager menuManager;
	private Transform scoresPanel;
	private Transform pausePanel;
	private List<Button> pauseMenuButtons = new List<Button>();
	private int pauseButtonSelectionPosition = 0;
	private bool gamePaused = false;
	private int playerInputNumber;
	private List<GameObject> charactersKills=new List<GameObject>();
	private List<PlayerPreview> playersPreviews = new List<PlayerPreview>();
	private uint lastMusicSwitchPauseState = 0;
	private uint lastMusicSwitchScoreState = 0;
	new void Awake(){
		base.Awake ();
	}

	void Start () {
		menuManager = MenuManager.Instance;
		gameManager = GameManager.Instance;
		scoresPanel = transform.Find ("Scores");
		pausePanel = transform.Find ("PauseMenu");
		for (int i = 0; i < pausePanel.childCount; i++) {
			pauseMenuButtons.Add (pausePanel.GetChild (i).GetComponent<Button> ());
		}
		for (int i = 0; i < gameManager.possiblePlayers; i++) {
			charactersKills.Add(scoresPanel.Find ("Characters").Find("Character"+(i+1)).gameObject);
		}
	}

	public void StartGame(List<GameObject>players){
		playersPreviews.Clear ();
		for (int i = 0; i < players.Count; i++) {
			playersPreviews.Add (players [i].GetComponent<PlayerPreview> ());
		}
		for (int i = 0; i < playersPreviews.Count; i++) {
			int characterIdentifier = playersPreviews [i].charPreviewPos-1;
			charactersKills [i].transform.Find("Character").GetComponent<Image> ().sprite = charactersPreviews [characterIdentifier];
		}
		isGameStarted = true;
	}
	public void FinishGame(){
		for (int i = 0; i < charactersKills.Count; i++) {			
			charactersKills [i].SetActive (false);
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform.Find("Kills")) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < childrensImages.Count; j++) {
				childrensImages [j].SetActive (false);
			}
		}
		isGameStarted = false;
	}

	public IEnumerator ShowActualScores(List<int> scores, List<int> negativeScores,float time){
		scoresPanel.gameObject.SetActive (true);
		for (int i = 0; i < scores.Count; i++) {
			charactersKills [i].SetActive (true);
		}
		for (int i = 0; i < scores.Count; i++) {
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform.Find("Kills")) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < scores[i]; j++) {
				if (j < childrensImages.Count) {
					if (!childrensImages [j].activeSelf) {
					yield return new WaitForSeconds (timeBetweenCountingScore);
						childrensImages [j].SetActive (true);
					}
				}
			}
			for (int j = scores[i]; j > (scores[i]-negativeScores[i]); j--) {
				yield return new WaitForSeconds (timeBetweenCountingScore);
				if ((j-1) < childrensImages.Count) {
					childrensImages [j - 1].SetActive (false);
				}
			}
		}
		AkSoundEngine.GetSwitch ("InGameMusic", gameObject, out lastMusicSwitchScoreState);
		AkSoundEngine.SetSwitch("InGameMusic","Score",gameObject);
		yield return new WaitForSeconds (time);

		AkSoundEngine.SetSwitch(AkSoundEngine.GetIDFromString("InGameMusic"),lastMusicSwitchScoreState,gameObject);

		for (int i = 0; i < scores.Count; i++) {			
			charactersKills [i].SetActive (false);
		}
		scoresPanel.gameObject.SetActive (false);
	}

	void Update(){
		if (isGameStarted && !scoresPanel.gameObject.activeSelf) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				if ((InputManager.Devices [i].Name == "GlobalKeyboard") && InputManager.Devices [i].Action2.WasPressed)
					PauseGame(gameManager.GetPlayerOneInputNumber());
			}
		}
		if (gamePaused) {
			if (InputManager.Devices [playerInputNumber].DPadUp.WasPressed) {
				//Move up
				if (pauseButtonSelectionPosition > 0) {
					pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (false);
					pauseButtonSelectionPosition--;
					AkSoundEngine.PostEvent("ChoosingMenuPauseUp",gameObject);
					EventSystem.current.SetSelectedGameObject(pauseMenuButtons[pauseButtonSelectionPosition].gameObject);
					pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (true);
				}
			}

			if (InputManager.Devices [playerInputNumber].DPadDown.WasPressed) {
				//Move Down
				if (pauseButtonSelectionPosition < pauseMenuButtons.Count-1) {
					pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (false);
					pauseButtonSelectionPosition++;
					AkSoundEngine.PostEvent("ChoosingMenuPauseDown",gameObject);
					EventSystem.current.SetSelectedGameObject(pauseMenuButtons[pauseButtonSelectionPosition].gameObject);
					pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (true);

				}
			}


			if (InputManager.Devices [playerInputNumber].Action1.WasReleased || WasGlobalAction1Pressed()) {
				pauseMenuButtons [pauseButtonSelectionPosition].onClick.Invoke ();
			}

		}
	}

	private bool WasGlobalAction1Pressed(){
		for (int i = 0; i < InputManager.Devices.Count; i++) {
			if ((InputManager.Devices [i].Name == "GlobalKeyboard") && InputManager.Devices [i].Action1.WasPressed)
				return true;
			else
				return false;
		}
		return false;
	}

	public void PauseGame(int rplayerInputNumber){
		if (gameManager.PauseGame ()) {
			AkSoundEngine.GetSwitch ("InGameMusic", gameObject, out lastMusicSwitchPauseState);
			AkSoundEngine.SetSwitch("InGameMusic","PauseMusic",gameObject);
			playerInputNumber = rplayerInputNumber;
			EventSystem.current.sendNavigationEvents = false;
			gamePaused = true;
			pausePanel.gameObject.SetActive (true);
			EventSystem.current.SetSelectedGameObject (null);
			EventSystem.current.SetSelectedGameObject (pauseMenuButtons [0].gameObject);
			pauseMenuButtons [0].transform.GetChild(0).gameObject.SetActive (true);
			if (gameManager.GetLaserManager ().SuddenDeath == true) {
				AkSoundEngine.PostEvent("PauseInGameLaserSound",gameObject);
			}
		} else if(gamePaused) {
			UnPauseGame ();
		}
	}

	public void UnPauseGame(){
		pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (false);
		EventSystem.current.sendNavigationEvents = true;
		pauseButtonSelectionPosition = 0;
		EventSystem.current.SetSelectedGameObject (null);
		EventSystem.current.SetSelectedGameObject (pauseMenuButtons [pauseButtonSelectionPosition].gameObject);
		gamePaused = false;
		pausePanel.gameObject.SetActive (false);
		AkSoundEngine.SetSwitch(AkSoundEngine.GetIDFromString("InGameMusic"),lastMusicSwitchPauseState,gameObject);

		if (gameManager.GetLaserManager ().SuddenDeath == true) {
			AkSoundEngine.PostEvent("StartInGameLaserSound",gameObject);
		}
		AkSoundEngine.PostEvent ("PressBackToGame", gameObject);
		gameManager.UnPauseGame ();
	}

	public void BackToMain(){
		pauseMenuButtons [pauseButtonSelectionPosition].transform.GetChild(0).gameObject.SetActive (false);
		EventSystem.current.sendNavigationEvents = true;
		pauseButtonSelectionPosition = 0;
		gamePaused = false;
		isGameStarted = false;
		pausePanel.gameObject.SetActive (false);
		gameManager.BackToMain ();
	}

}
