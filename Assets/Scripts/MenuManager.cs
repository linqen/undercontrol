using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class MenuManager : GenericSingletonClass<MenuManager> {
	public List<GameObject> menuPlayerPreview = new List<GameObject>();

	GameManager gameManager;
	GameObject pressStart;
	GameObject mainMenu;
	GameObject characterSelect;
	GameObject mapSelect;
	GameObject roundsSelect;
	new void Awake(){
		base.Awake ();
	}
	void Start () {
		gameManager = GameManager.Instance;
		pressStart = transform.Find ("PressStart").gameObject;
		mainMenu = transform.Find ("MainMenu").gameObject;
		mapSelect = transform.Find ("MapSelect").gameObject;
		characterSelect = transform.Find ("CharacterSelect").gameObject;
		roundsSelect = transform.Find ("RoundsSelect").gameObject;
		for (int i = 0; i < menuPlayerPreview.Count; i++) {
			menuPlayerPreview[i] = characterSelect.transform.Find ("Character" + (i + 1)).gameObject;
		}
		gameManager.PressStartButton ();
	}

	public void StartPressed(){
		pressStart.SetActive (false);
		mainMenu.SetActive (true);
	}

	public void CharacterSelectionFinished(){
		characterSelect.SetActive (false);
		roundsSelect.SetActive (true);
	}
	public void RoundsSelect(){
		int rounds = int.Parse(EventSystem.current.currentSelectedGameObject.name);
		roundsSelect.SetActive (false);
		gameManager.GameStart("Map1",rounds);
	}

	public void PlayButton(){
		mainMenu.SetActive (false);
		characterSelect.SetActive (true);
		gameManager.CharSelection ();
	}

	public void BackToMain(){
		mainMenu.SetActive (true);

	}
	public void SetImagePreview(PlayerPreview playerPreview,PlayerInput playerInput){
		Image character = menuPlayerPreview [playerPreview.playerNumber - 1].GetComponent<Image> ();
		character.sprite = playerPreview.charPreview;
		Image temporalKeyImage;

		temporalKeyImage = character.transform.Find ("Left").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Horizontal+"Left");

		temporalKeyImage = character.transform.Find ("Right").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Horizontal+"Right");

		temporalKeyImage = character.transform.Find ("Up").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Vertical+"Up");

		temporalKeyImage = character.transform.Find ("Down").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Vertical+"Down");

		temporalKeyImage = character.transform.Find ("Start").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Start);

		temporalKeyImage = character.transform.Find ("Fire").GetComponent<Image> ();
		temporalKeyImage.gameObject.SetActive (true);
		temporalKeyImage.sprite = (Sprite)Resources.Load<Sprite> ("Keys/"+playerInput.Fire);
	}

	public void SetImagePreview(PlayerPreview playerPreview){
		Image character = menuPlayerPreview [playerPreview.playerNumber - 1].GetComponent<Image> ();
		character.sprite = playerPreview.charPreview;
	}

	public string GetCharacterFromPreview(int playerNumber){
		return menuPlayerPreview [playerNumber].GetComponent<Image> ().sprite.name;
	}
}
