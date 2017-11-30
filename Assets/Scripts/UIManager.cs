using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : GenericSingletonClass<UIManager> {

	public Sprite[] charactersPreviews;
	public Sprite[] grenades;


	GameManager gameManager;
	MenuManager menuManager;
	Transform scoresPanel;
	Transform pausePanel;
	List<Button> pauseMenuButtons = new List<Button>();
	int pauseButtonSelectionPosition = 0;
	bool gamePaused = false;

	List<GameObject> charactersKills=new List<GameObject>();
	List<PlayerPreview> playersPreviews = new List<PlayerPreview>();
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
			charactersKills.Add(scoresPanel.Find ((i + 1).ToString ()).Find("Character").gameObject);
		}
	}

	public void StartGame(List<GameObject>players){
		playersPreviews.Clear ();
		for (int i = 0; i < players.Count; i++) {
			playersPreviews.Add (players [i].GetComponent<PlayerPreview> ());
		}
		for (int i = 0; i < playersPreviews.Count; i++) {
			int characterIdentifier = playersPreviews [i].charPreviewPos-1;
			charactersKills [i].GetComponent<Image> ().sprite = charactersPreviews [characterIdentifier];
			foreach (Transform possibleKill in charactersKills[i].transform) {
				possibleKill.GetComponent<Image>().sprite = grenades [characterIdentifier];
			}
		}
	}

	public IEnumerator ShowActualScores(List<int> scores,float time){
		for (int i = 0; i < scores.Count; i++) {
			charactersKills [i].SetActive (true);
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < scores[i]; j++) {
				childrensImages [j].SetActive (true);
			}
		}
		yield return new WaitForSeconds (3f);
		for (int i = 0; i < scores.Count; i++) {			
			charactersKills [i].SetActive (false);
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < scores[i]; j++) {
				childrensImages [j].SetActive (false);
			}
		}
	}

	void Update(){
		if (gamePaused) {
			bool canMoveJoySelection = true;
			for (int i = 1; i <= gameManager.possiblePlayers; i++) {
				float axisRawValue = Input.GetAxisRaw (Inputs.Vertical + i);
				if (Input.GetButtonDown (Inputs.Vertical + i) && axisRawValue > 0.5f) {
					//Move Up
					if (pauseButtonSelectionPosition > 0) {
						pauseButtonSelectionPosition--;
					}
				} else if (Input.GetButtonDown (Inputs.Vertical + i) && axisRawValue < 0.5f) {
					//Move Down
					if (pauseButtonSelectionPosition < pauseMenuButtons.Count-1) {
						pauseButtonSelectionPosition++;
					}
				} else {
					//Joystick case
					if (axisRawValue > 0.5f && !Input.GetButton (Inputs.Vertical + i) && canMoveJoySelection == true) {
						//Move Up
						canMoveJoySelection = false;
						if (pauseButtonSelectionPosition > 0) {
							pauseButtonSelectionPosition--;
						}

					} else if (axisRawValue < -0.5f &&
						!Input.GetButton (Inputs.Vertical + i) && canMoveJoySelection == true) {
						//Move Down
						canMoveJoySelection = false;
						if (pauseButtonSelectionPosition < pauseMenuButtons.Count-1) {
							pauseButtonSelectionPosition++;
						}

					} else if (axisRawValue == 0.0f) {
						canMoveJoySelection = true;
					}
					EventSystem.current.SetSelectedGameObject(pauseMenuButtons[pauseButtonSelectionPosition].gameObject);
				}

				if (Input.GetButtonUp (Inputs.Jump + i)) {
					pauseMenuButtons [pauseButtonSelectionPosition].onClick.Invoke ();
				}

			}

		}
	}

	public void PauseGame(){
		if (gameManager.PauseGame ()) {
			EventSystem.current.sendNavigationEvents = false;
			gamePaused = true;
			pausePanel.gameObject.SetActive (true);
			EventSystem.current.SetSelectedGameObject (null);
			EventSystem.current.SetSelectedGameObject (pauseMenuButtons [0].gameObject);
		} else if(gamePaused) {
			UnPauseGame ();
		}
	}

	public void UnPauseGame(){
		EventSystem.current.sendNavigationEvents = true;
		pauseButtonSelectionPosition = 0;
		EventSystem.current.SetSelectedGameObject (null);
		EventSystem.current.SetSelectedGameObject (pauseMenuButtons [pauseButtonSelectionPosition].gameObject);
		gamePaused = false;
		pausePanel.gameObject.SetActive (false);
		gameManager.UnPauseGame ();
	}

	public void BackToMain(){
		EventSystem.current.sendNavigationEvents = true;
		pauseButtonSelectionPosition = 0;
		gamePaused = false;
		pausePanel.gameObject.SetActive (false);
		gameManager.BackToMain ();
	}

}
