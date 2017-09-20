using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//public class CharacterSelectEvent:UnityEvent<bool>{}
//public class StartGameEvent:UnityEvent<bool>{}
public class MenuManager : GenericSingletonClass<MenuManager> {

	GameManager gameManager;
	GameObject pressStart;
	GameObject mainMenu;
	GameObject characterSelect;
	GameObject mapSelect;
	//public CharacterSelectEvent charSelectEvent;
	//public StartGameEvent startGameEvent;
	new void Awake(){
		//charSelectEvent = new CharacterSelectEvent ();
		//startGameEvent = new StartGameEvent ();
	}

	// Use this for initialization
	void Start () {
		pressStart = transform.Find ("PressStart").gameObject;
		mapSelect = transform.Find ("MapSelect").gameObject;
		mainMenu = transform.Find ("MainMenu").gameObject;
		characterSelect = transform.Find ("CharacterSelect").gameObject;
		gameManager = GameManager.Instance;

		gameManager.PressStartButton ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartPressed(){
		pressStart.SetActive (false);
		mainMenu.SetActive (true);
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		//characterSelect.SetActive (true);
		//gameManager.CharSelection ();
		//mapSelect.SetActive (true);
		//EventSystem.current.currentSelectedGameObject.name;
		gameManager.GameStart();
	}

	public void BackToMain(){
		mainMenu.SetActive (true);

	}



}
