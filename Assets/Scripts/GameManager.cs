using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager> {
	public GameObject playerPrefab;
	[Range(1,8)]
	public int possiblePlayers;

	public List<Sprite> availablePlayersPreviews = new List<Sprite>();
	List<Sprite> onUsePlayersPreviews = new List<Sprite>();
	List<GameObject> players = new List<GameObject>();
	bool charSelection = false;
	bool pressStart = false;
	MenuManager menuManager;

	new void Awake(){
		menuManager = MenuManager.Instance;
	}

	void Update () {
		if (pressStart) {
			OnPressStart ();
		}
		if (charSelection) {
			OnCharSelection ();
		}
	}

	public void ReportDeath(GameObject playerObject){
		Destroy (playerObject);
		SceneManager.UnloadSceneAsync ("Map1");
		menuManager.BackToMain ();
	}

	public void CharSelection(){
		Debug.Log ("charSelection");
		charSelection = true;
		menuManager.SetImagePreview (players[0].GetComponent<PlayerManager>().Preview);
	}

	public void GameStart(){
		SceneManager.LoadScene ("Map1",LoadSceneMode.Additive);
		Instantiate (playerPrefab);
	}
	public void PressStartButton(){
		pressStart = true;
	}

	private void OnPressStart(){
		for (int i = 1; i <= possiblePlayers; i++) {
			if (Input.GetButtonDown (Inputs.Start+i)) {
				Debug.Log ("Enter pressed");
				GameObject newPlayer = CreatePlayer (i);
				players.Add (newPlayer);
				menuManager.StartPressed ();
				pressStart = false;
			}
		}
	}

	private void OnCharSelection(){
		for (int i = 1; i <= possiblePlayers; i++) {
			//Detect new players entering the game
			if (Input.GetButtonDown (Inputs.Start+i)) {
				bool inputUsed = false;
				for (int j = 0; j < players.Count; j++) {
					if (players [j].GetComponent<PlayerManager> ().PlayerInput.GetInputNumber () == i) {
						//players [j].GetComponent<PlayerManager> ().SetCharacter ();
						Debug.Log ("Used Enter pressed "+menuManager.GetCharacterFromPreview(j));
						//players [j].GetComponent<PlayerManager> ().preview.onUse = true;
						SelectActualPlayer(players [j].GetComponent<PlayerManager> ().Preview);
						if (players.Count>=2&&
							onUsePlayersPreviews.Count==players.Count) {
							charSelection = false;
							menuManager.CharacterSelectionFinished ();
							Debug.Log ("ARRANCA EL JUEGO");
						}
						inputUsed = true;
					}
				}
				if (!inputUsed) {
					Debug.Log ("New Enter pressed");
					GameObject newPlayer = CreatePlayer (i);
					menuManager.SetImagePreview(newPlayer.GetComponent<PlayerManager> ().Preview);
					players.Add (newPlayer);
				}
			}
		}
		for (int i = 0; i < players.Count; i++) {
			PlayerManager pm = players [i].GetComponent<PlayerManager> ();
			int actualInput = pm.PlayerInput.GetInputNumber ();
			PlayerPreview pp = pm.Preview;
			if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				Input.GetAxisRaw (Inputs.Horizontal + actualInput) > 0.5f &&
				!pm.Preview.selected) {
				//Mover derecha
				PlayerPreview newPreview=new PlayerPreview(pp.playerNumber,GetNextUnusedPlayer(pp.charPreview));
				menuManager.SetImagePreview(newPreview);
				players[i].GetComponent<PlayerManager>().Preview=newPreview;
			} 
		else if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				Input.GetAxisRaw (Inputs.Horizontal + actualInput) < -0.5f &&
				!pm.Preview.selected) {
				//Mover Izquierda
				PlayerPreview newPreview=new PlayerPreview(pp.playerNumber,GetPreviousUnusedPlayer(pp.charPreview));
				menuManager.SetImagePreview(newPreview);
				players[i].GetComponent<PlayerManager>().Preview=newPreview;
			}
		}
	}

	private GameObject CreatePlayer(int inputNumber){
		GameObject newPlayer =Instantiate(playerPrefab);
		PlayerManager pm = newPlayer.GetComponent<PlayerManager> ();
		int playerNumber = (players.Count + 1);
		newPlayer.name = ("Player" + (playerNumber));
		pm.Preview=new PlayerPreview(playerNumber,GetNextUnusedPlayer());
		pm.PlayerInput=new PlayerInput(inputNumber);
		return newPlayer;
	}
	
	private Sprite GetNextUnusedPlayer(){
		return availablePlayersPreviews [0];
	}
	private Sprite GetNextUnusedPlayer(Sprite actualPreview){
		int previewPosition = availablePlayersPreviews.IndexOf (actualPreview);
		previewPosition++;
		if (previewPosition == availablePlayersPreviews.Count) {
			previewPosition = 0;
		}
		return availablePlayersPreviews [previewPosition];
	}
	private Sprite GetPreviousUnusedPlayer(Sprite actualPreview){
		int previewPosition = availablePlayersPreviews.IndexOf (actualPreview);
		previewPosition--;
		if (previewPosition < 0) {
			previewPosition = availablePlayersPreviews.Count-1;
		}
		return availablePlayersPreviews [previewPosition];
	}

	private bool SelectActualPlayer(PlayerPreview playerPreview){
		Sprite preview = playerPreview.charPreview;
		for (int i = 0; i < availablePlayersPreviews.Count; i++) {
			if (preview.name.Equals (availablePlayersPreviews [i].name)) {
				onUsePlayersPreviews.Add (availablePlayersPreviews [i]);
				availablePlayersPreviews.RemoveAt (i);
				playerPreview.selected = true;
				return true;
			}
		}
		return false;
	}
}