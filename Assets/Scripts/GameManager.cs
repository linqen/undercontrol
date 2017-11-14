using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager> {
	public GameObject playerPrefab;
	[Range(1,8)]
	public int possiblePlayers;
	public float secondsToWaitAfterDeath;
	public float suddenDeathTime;
	public float onlyTwoPlayersGameSuddenDeathTime;
	public float countdownBeforePlay;
	public List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController> ();

	int playersReady=0;
	List<GameObject> players = new List<GameObject>();
	List<Transform> spawnPoints = new List<Transform>();
	List<GameObject> deathReportPlayers = new List<GameObject>();
	List<int> deathReportKilledBy = new List<int>();
	LasersManager lasersManager;
	bool charSelection = false;
	bool pressStart = false;
	List<int> scores = new List<int> ();
	MenuManager menuManager;
	UIManager uiManager;
	AudioManager audioManager;
	int deadPlayers=0;
	int numberOfRounds;
	string actualMapName;
	Coroutine startedDeathsReport = null;
	new void Awake(){
		base.Awake ();
	}

	void Start(){
		uiManager = UIManager.Instance;
		audioManager = AudioManager.Instance;
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

	public void ReportDeath(GameObject playerObject, int killedByPlayerNumber){
		playerObject.SetActive (false);
		deathReportPlayers.Add (playerObject);
		deathReportKilledBy.Add (killedByPlayerNumber);
		if (startedDeathsReport != null) {
			StopCoroutine (startedDeathsReport);
		}
		startedDeathsReport = StartCoroutine (ReportDeath());
	}

	private IEnumerator ReportDeath(){
		yield return new WaitForSeconds (secondsToWaitAfterDeath);
		ProcessDeaths();
		startedDeathsReport = null;
	}

	private void ProcessDeaths(){
		for (int k = 0; k < deathReportPlayers.Count; k++) {
			deadPlayers++;
			if (players.Count - deadPlayers == 2) {
				lasersManager.StartLasers (suddenDeathTime);
			}
			deathReportPlayers[k].transform.rotation = Quaternion.identity;
			if (deathReportKilledBy[k] != 0 ) {
				if (deathReportKilledBy[k] == deathReportPlayers[k].GetComponent<PlayerPreview> ().playerNumber &&
					scores [deathReportKilledBy[k] - 1] != 0) {
					scores [deathReportKilledBy[k] - 1]--;
				}else if (deathReportKilledBy[k] != deathReportPlayers[k].GetComponent<PlayerPreview> ().playerNumber) {scores [deathReportKilledBy[k] - 1]++;}
			}
			if (players.Count -1 <= deadPlayers && 
				deathReportPlayers.Count-1 == k) {
				lasersManager.DisableLasers ();
				//Prepare the envoirement to re-play
				for (int i = 0; i < players.Count; i++) {
					players [i].SetActive (false);
					players [i].GetComponent<PlayerLife> ().ResetPlayer();
					StartCoroutine(uiManager.ShowActualScores(scores,3));
				}
				deadPlayers = 0;
				bool someoneWin = false;
				for (int i = 0; i < scores.Count; i++) {
					if (scores [i] >= numberOfRounds) {
						someoneWin = true;
					}
				}

				if (someoneWin) {
					StartCoroutine (FinishGame ());
				} else {
					//GetNextMap
					StartCoroutine(NextRound());
				}
			}
		}
		deathReportPlayers.Clear ();
		deathReportKilledBy.Clear ();
	}
	private IEnumerator FinishGame(){
		yield return new WaitForSeconds (3.0f);
		//End of rounds, back to selection
		SceneManager.UnloadSceneAsync (actualMapName);
		menuManager.BackToMain ();
	}

	private IEnumerator NextRound(){
		yield return new WaitForSecondsRealtime (3.0f);
		SceneManager.UnloadSceneAsync (actualMapName);
		GameStart ("Map1", numberOfRounds);

	}

	public IEnumerator CharSelection(){
		//This is maded to wait one frame and doesn't capture the actual GetKeyDown
		yield return null;
		charSelection = true;
		if (players [0].GetComponent<PlayerPreview> ().charPreviewPos == 0) {
			menuManager.GoNextPreview (players [0].GetComponent<PlayerPreview> ());
		}
		for (int i = 0; i < players.Count; i++) {
			StartCoroutine(players [i].GetComponent<PlayerMovement> ().CharSelection ());
		}
	}

	public void GameStart(string rmapName, int numberOfRounds){
		this.numberOfRounds = numberOfRounds;
		audioManager.InGameMusic ();
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
		
		Time.timeScale = 0;
		menuManager.ShowCountdown (countdownBeforePlay);
		uiManager.StartGame (players);
		yield return new WaitForSecondsRealtime (countdownBeforePlay);
		Time.timeScale = 1;
		//Lasers
		lasersManager = GameObject.Find("Lasers").GetComponent<LasersManager>();
		if (players.Count == 2) {
			lasersManager.StartLasers (onlyTwoPlayersGameSuddenDeathTime);
		}
	}
	public void PressStartButton(){
		pressStart = true;
	}

	private void OnPressStart(){
		for (int i = 1; i <= possiblePlayers; i++) {
			if (Input.GetButtonDown (Inputs.Start+i)) {
				GameObject newPlayer = CreatePlayer (i);
				PlayerInput pi = newPlayer.GetComponent<PlayerInput> ();
				StandaloneInputModule im = EventSystem.current.currentInputModule.GetComponent<StandaloneInputModule> ();
				im.verticalAxis = pi.Vertical;
				im.horizontalAxis = pi.Horizontal;
				im.submitButton = pi.Start;
				im.cancelButton = pi.Fire;
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
					PlayerInput currentPlayerInput = players [j].GetComponent<PlayerInput> ();
					if (currentPlayerInput.GetInputNumber() == i) {
						bool result = menuManager.SelectPreview(players [j].GetComponent<PlayerPreview> (),currentPlayerInput);
						if (result) {
							playersReady++;
							PlayerPreview playerPreview = players [j].GetComponent<PlayerPreview> ();
							players [playerPreview.playerNumber - 1].GetComponent<Animator> ().runtimeAnimatorController = animators [playerPreview.charPreviewPos-1];
							audioManager.SelectedSound();
						}
						if (players.Count>=2&&
							playersReady==players.Count) {
							FinishPlayersSelection ();
							menuManager.CharacterSelectionFinished ();
						}
						inputUsed = true;
					}
				}
				if (!inputUsed) {
					GameObject newPlayer = CreatePlayer (i);
					menuManager.GoNextPreview(newPlayer.GetComponent<PlayerPreview> ());
					StartCoroutine (newPlayer.GetComponent<PlayerMovement> ().CharSelection ());
					players.Add (newPlayer);
				}
			}
			//Cancel button is pressed
			if (Input.GetButtonDown (Inputs.Fire + i)) {
				menuManager.GoBack ();
			}
		}
	}

	public IEnumerator RoundSelection(){
		bool waiting = true;
		int mainPlayerInput = players [0].GetComponent<PlayerInput> ().GetInputNumber ();
		while (waiting) {
			if(Input.GetButtonDown(Inputs.Fire+mainPlayerInput)){
				menuManager.GoBack();
				waiting = false;
			}
			yield return null;
		}
	}

	public void StopCharSelection(){
		ClearPlayerSelectionValues ();
		FinishPlayersSelection ();
	}

	void ClearPlayerSelectionValues(){
		for (int i = 0; i < players.Count; i++) {
			players [i].GetComponent<PlayerPreview> ().selected = false;
			scores [i] = 0;
		}
		playersReady=0;
		charSelection = false;
	}

	private GameObject CreatePlayer(int inputNumber){
		GameObject newPlayer =Instantiate(playerPrefab);
		PlayerPreview pp = newPlayer.AddComponent<PlayerPreview> ();
		int playerNumber = (players.Count + 1);
		newPlayer.name = ("Player" + (playerNumber));
		PlayerInput pi = newPlayer.AddComponent<PlayerInput>();
		pi.SetInputNumber (inputNumber);
		pp.SetPreview(playerNumber,0);
		scores.Add (0);
		return newPlayer;
	}

	private void FinishPlayersSelection(){
		ClearPlayerSelectionValues ();
		for (int i = 0; i < players.Count; i++) {
			players [i].GetComponent<PlayerMovement> ().StopCharSelection();
		}
	}
	//Previews Managment
	public void GetNextUnusedPlayer(PlayerPreview actualPreview){
		menuManager.GoNextPreview (actualPreview);
		audioManager.ChoosingSound ();
	}
	public void GetPreviousUnusedPlayer(PlayerPreview actualPreview){
		menuManager.GoPreviousPreview (actualPreview);
		audioManager.ChoosingSound ();
	}
	//Previews Managment
}