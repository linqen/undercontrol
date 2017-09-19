using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : GenericSingletonClass<GameManager> {

	public GameObject playerPrefab;

	bool isCharSelection = false;
	bool isGameStart = false;
	MenuManager menuManager;




	new void Awake(){
		menuManager = MenuManager.Instance;
	}

	void Start () {
		menuManager.charSelectEvent.AddListener (setIsCharSelection);
		menuManager.startGameEvent.AddListener (setIsGameStart);
	}
	
	void Update () {
		if (isCharSelection) {
			Debug.Log ("IsCharSelection");
		}
		if (isGameStart) {
			Debug.Log ("IsGameStart");
			SceneManager.LoadScene ("Map1",LoadSceneMode.Additive);
			Instantiate (playerPrefab);
			isGameStart = false;
		}
	}

	public void ReportDeath(GameObject playerObject){
		Destroy (playerObject);
		SceneManager.UnloadSceneAsync ("Map1");
		menuManager.BackToMain ();
	}


	//isCharSelection Get&Set
	void setIsCharSelection(bool charSelectState){isCharSelection = charSelectState;}
	bool getIsCharSelection(){return isCharSelection;}
	//isGameStart Get&Set
	void setIsGameStart(bool isGameStartState){isGameStart= isGameStartState;}
	bool getIsGameStart(){return isGameStart;}
}
