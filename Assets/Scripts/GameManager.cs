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
	int numberOfRounds;
	string actualMapName;
	new void Awake(){
		base.Awake ();
	}

	void Start(){
		menuManager = MenuManager.Instance;
		menuManager.SetPossiblePlayers (possiblePlayers);
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
		playerObject.transform.rotation = Quaternion.identity;
		//playerObject.GetComponent<Rigidbody2D>().velocity=Vector2.zero;
		//playerObject.GetComponent<Rigidbody2D>().angularVelocity=0.0f;
		playerObject.SetActive (false);
		if (players.Count -1 == deadPlayers) {
			numberOfRounds--;
			//Prepare the envoirement to re-play
			for (int i = 0; i < players.Count; i++) {
				players [i].SetActive (false);
				players [i].GetComponent<PlayerLife> ().ResetLife();
			}
			deadPlayers = 0;
			if (numberOfRounds==0) {
				//End of rounds, back to selection
				for (int i = 0; i < players.Count; i++) {
					players [i].GetComponent<PlayerPreview> ().selected = false;
				}
				for (int i = 0; i < onUsePlayersPreviews.Count; i++) {
					availablePlayersPreviews.Add (onUsePlayersPreviews [i]);
				}
				onUsePlayersPreviews.Clear ();
				SceneManager.UnloadSceneAsync (actualMapName);
				menuManager.BackToMain ();
			} else {
				//GetNextMap
				StartCoroutine(NextRound());
			}
		}
	}

	private IEnumerator NextRound(){
		yield return new WaitForSecondsRealtime (3.0f);
		SceneManager.UnloadSceneAsync (actualMapName);
		GameStart ("Map1", numberOfRounds);

	}

	public void CharSelection(){
		charSelection = true;
		menuManager.SetImagePreview (players[0].GetComponent<PlayerPreview>(),
			players[0].GetComponent<PlayerInput>());
	}

	public void GameStart(string rmapName, int numberOfRounds){
		this.numberOfRounds = numberOfRounds;
		StartCoroutine (OnGameStart (rmapName));
	}
	private IEnumerator OnGameStart(string rmapName){
		actualMapName = rmapName;
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
					if (players [j].GetComponent<PlayerInput> ().GetInputNumber () == i) {
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
					menuManager.SetImagePreview(newPlayer.GetComponent<PlayerPreview> (),
						newPlayer.GetComponent<PlayerInput>());
					players.Add (newPlayer);
				}
			}
		}
		for (int i = 0; i < players.Count; i++) {
			PlayerPreview pp = players [i].GetComponent<PlayerPreview>();
			int actualInput = players[i].GetComponent<PlayerInput>().GetInputNumber ();
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
		PlayerPreview pp = newPlayer.AddComponent<PlayerPreview> ();
		int playerNumber = (players.Count + 1);
		newPlayer.name = ("Player" + (playerNumber));
		PlayerInput pi = newPlayer.AddComponent<PlayerInput>();
		pi.SetInputNumber (inputNumber);
		pp.SetPreview(playerNumber,GetNextUnusedPlayer());
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