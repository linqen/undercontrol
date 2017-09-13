using UnityEngine;
using UnityEngine.UI;

public class MenuManager : GenericSingletonClass<MenuManager> {

	GameObject mapSelect;
	GameObject characterSelect;
	GameObject mainMenu;


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
		mapSelect.SetActive (true);
	}

}
