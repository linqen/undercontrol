using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager> {
	public GameObject playerPrefab;
	[Range(1,8)]
	public int possiblePlayers;
	public List<Sprite> availablePlayersPreviews = new List<Sprite>();

	List<Sprite> onUsePlayersPreviews = new List<Sprite>();
	List<GameObject> players = new List<GameObject>();
	List<Transform> spawnPoints = new List<Transform>();
	bool charSelection = false;
	bool pressStart = false;
	MenuManager menuManager;
	int deadPlayers=0;
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
		deadPlayers++;
		playerObject.SetActive (false);
		if (players.Count -1 == deadPlayers) {
			//Prepare the envoirement to re-play
			for (int i = 0; i < players.Count; i++) {
				players [i].SetActive (false);
				players [i].GetComponent<PlayerPreview> ().selected = false;
				players [i].GetComponent<PlayerLife> ().ResetLife();
			}
			for (int i = 0; i < onUsePlayersPreviews.Count; i++) {
				availablePlayersPreviews.Add (onUsePlayersPreviews[i]);
			}
			onUsePlayersPreviews.Clear ();
			deadPlayers = 0;
			SceneManager.UnloadSceneAsync ("Map1");
			menuManager.BackToMain ();
		}
	}

	public void CharSelection(){
		charSelection = true;
		menuManager.SetImagePreview (players[0].GetComponent<PlayerPreview>());
	}

	public void GameStart(string rmapName){
		StartCoroutine (OnGameStart (rmapName));
	}
	private IEnumerator OnGameStart(string rmapName){
		bool loadStarted=false;
		if (!loadStarted) {
			SceneManager.LoadScene (rmapName,LoadSceneMode.Additive);
			loadStarted = true;
			yield return null;
		}
		Transform spawnPointsParent;
		spawnPointsParent = GameObject.Find ("SpawnPoints").transform;
		spawnPoints.Clear ();
		foreach (Transform spawnPoint in spawnPointsParent) {
			spawnPoints.Add (spawnPoint);
		}
		for (int i = 0; i < players.Count; i++) {
			int randomSpawn = Random.Range (0, (spawnPoints.Count - 1));
			players [i].transform.position = spawnPoints [randomSpawn].position;
			players [i].SetActive (true);
			spawnPoints.RemoveAt (randomSpawn);
		}
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
						Debug.Log ("Used Enter pressed "+menuManager.GetCharacterFromPreview(j));
						SelectActualPlayer(players [j].GetComponent<PlayerPreview> ());
						if (players.Count>=2&&
							onUsePlayersPreviews.Count==players.Count) {
							charSelection = false;
							menuManager.CharacterSelectionFinished ();
						}
						inputUsed = true;
					}
				}
				if (!inputUsed) {
					GameObject newPlayer = CreatePlayer (i);
					menuManager.SetImagePreview(newPlayer.GetComponent<PlayerPreview> ());
					players.Add (newPlayer);
				}
			}
		}
		for (int i = 0; i < players.Count; i++) {
			PlayerManager pm = players [i].GetComponent<PlayerManager> ();
			int actualInput = pm.PlayerInput.GetInputNumber ();
			PlayerPreview pp = players [i].GetComponent<PlayerPreview>();
			if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				Input.GetAxisRaw (Inputs.Horizontal + actualInput) > 0.5f &&
				!pp.selected) {
				//Move Right
				pp.SetCharPreview(GetNextUnusedPlayer(pp.charPreview));
				menuManager.SetImagePreview(pp);
			} 
		else if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				Input.GetAxisRaw (Inputs.Horizontal + actualInput) < -0.5f &&
				!pp.selected) {
				//Move Left
				pp.SetCharPreview(GetPreviousUnusedPlayer(pp.charPreview));
				menuManager.SetImagePreview(pp);
			}
		}
	}

	private GameObject CreatePlayer(int inputNumber){
		GameObject newPlayer =Instantiate(playerPrefab);
		PlayerManager pm = newPlayer.GetComponent<PlayerManager> ();
		int playerNumber = (players.Count + 1);
		newPlayer.name = ("Player" + (playerNumber));
		PlayerPreview pp = newPlayer.AddComponent<PlayerPreview> ();
		pp.SetPreview(playerNumber,GetNextUnusedPlayer());
		pm.PlayerInput=newPlayer.AddComponent<PlayerInput>();
		pm.PlayerInput.SetInputNumber (inputNumber);
		return newPlayer;
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

	//Previews Managment
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
	//Previews Managment
}