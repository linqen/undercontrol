using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
public class UIManager : GenericSingletonClass<UIManager> {

	GameManager gameManager;
	MenuManager menuManager;
	Transform scoresPanel;
	new void Awake(){
		base.Awake ();
	}

	void Start () {
		menuManager = MenuManager.Instance;
		gameManager = GameManager.Instance;
		scoresPanel = transform.Find ("Scores");
	}

	public IEnumerator ShowActualScores(List<int> scores,float time){
		Transform actualScoreText;
		for (int i = 0; i < scores.Count; i++) {
			actualScoreText = scoresPanel.Find ("" + (i + 1));
			actualScoreText.gameObject.SetActive(true);
			actualScoreText.GetComponent<Text>().text = ("Player "+(i+1)+" Have "+scores[i]+"Kills");
		}
		yield return new WaitForSeconds (3f);
		for (int i = 0; i < scores.Count; i++) {
			actualScoreText = scoresPanel.Find ("" + (i + 1));
			actualScoreText.gameObject.SetActive(false);
		}
	}

}
