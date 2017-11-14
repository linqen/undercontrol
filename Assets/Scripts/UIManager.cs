using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class UIManager : GenericSingletonClass<UIManager> {

	public Sprite[] charactersPreviews;
	public Sprite[] grenades;


	GameManager gameManager;
	MenuManager menuManager;
	Transform scoresPanel;
	List<GameObject> charactersKills=new List<GameObject>();
	List<PlayerPreview> playersPreviews = new List<PlayerPreview>();
	new void Awake(){
		base.Awake ();
	}

	void Start () {
		menuManager = MenuManager.Instance;
		gameManager = GameManager.Instance;
		scoresPanel = transform.Find ("Scores");
		for (int i = 0; i < gameManager.possiblePlayers; i++) {
			charactersKills.Add(scoresPanel.Find ((i + 1).ToString ()).Find("Character").gameObject);
		}
	}

	public void StartGame(List<GameObject>players){
		playersPreviews.Clear ();
		for (int i = 0; i < players.Count; i++) {
			playersPreviews.Add (players [i].GetComponent<PlayerPreview> ());
		}
		for (int i = 0; i < playersPreviews.Count; i++) {
			int characterIdentifier = playersPreviews [i].charPreviewPos-1;
			charactersKills [i].GetComponent<Image> ().sprite = charactersPreviews [characterIdentifier];
			foreach (Transform possibleKill in charactersKills[i].transform) {
				possibleKill.GetComponent<Image>().sprite = grenades [characterIdentifier];
			}
		}
	}

	public IEnumerator ShowActualScores(List<int> scores,float time){
		for (int i = 0; i < scores.Count; i++) {
			charactersKills [i].SetActive (true);
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < scores[i]; j++) {
				childrensImages [j].SetActive (true);
			}
		}
		yield return new WaitForSeconds (3f);
		for (int i = 0; i < scores.Count; i++) {			
			charactersKills [i].SetActive (false);
			List<GameObject> childrensImages=new List<GameObject>();
			foreach (Transform kill in charactersKills[i].transform) {
				childrensImages.Add (kill.gameObject);
			}
			for (int j = 0; j < scores[i]; j++) {
				childrensImages [j].SetActive (false);
			}
		}
	}

}
