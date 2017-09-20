using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingletonClass<GameManager> {
	public GameObject playerPrefab;
	[Range(1,8)]
	public int possiblePlayers;

	List<GameObject> players = new List<GameObject>();
	int inputNumber=1;
	bool charSelection = false;
	bool pressStart = false;
	MenuManager menuManager;

	new void Awake(){
		menuManager = MenuManager.Instance;
	}

	void Start () {}
	
	void Update () {
		if (pressStart) {
			PressStart ();
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
	}

	public void GameStart(){
		SceneManager.LoadScene ("Map1",LoadSceneMode.Additive);
		Instantiate (playerPrefab);
	}
	public void PressStartButton(){
		pressStart = true;
	}

	private void PressStart(){
		for (int i = 1; i <= possiblePlayers; i++) {
			if (Input.GetButtonDown (Inputs.Start+i)) {
				Debug.Log ("Enter pressed");
				GameObject newPlayer = new GameObject ("Player" + inputNumber);
				inputNumber++;
				PlayerInput pi = newPlayer.AddComponent <PlayerInput>();
				pi.SetInput(i);
				players.Add (newPlayer);
				menuManager.StartPressed ();
				pressStart = false;
			}
		}
	}
}
