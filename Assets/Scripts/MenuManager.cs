using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class CharacterSelectEvent:UnityEvent<bool>{}
public class StartGameEvent:UnityEvent<bool>{}
public class MenuManager : GenericSingletonClass<MenuManager> {

	GameObject mapSelect;
	GameObject characterSelect;
	GameObject mainMenu;
	public CharacterSelectEvent charSelectEvent;
	public StartGameEvent startGameEvent;
	new void Awake(){
		charSelectEvent = new CharacterSelectEvent ();
		startGameEvent = new StartGameEvent ();
	}

	// Use this for initialization
	void Start () {
		mapSelect = transform.Find ("MapSelect").gameObject;
		mainMenu = transform.Find ("MainMenu").gameObject;
		characterSelect = transform.Find ("CharacterSelect").gameObject;



	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		//characterSelect.SetActive (true);
		//charSelectEvent.Invoke (true);
		//mapSelect.SetActive (true);
		//EventSystem.current.currentSelectedGameObject.name;
		startGameEvent.Invoke(true);
	}

	public void BackToMain(){
		mainMenu.SetActive (true);

	}

}
