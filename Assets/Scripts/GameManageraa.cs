using UnityEngine;
using UnityEngine.UI;
public class GameManageraa : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public Text player1Text;
	public Text player2Text;
	public GameObject player3;
	public GameObject player4;
	public Text player3Text;
	public Text player4Text;
	public Text countdownText;
	float deadIdleTime=4.5f;
	float onIdle;
	int player1Wins;
	int player2Wins;
	int player3Wins;
	int player4Wins;
	bool restartLevel;
	bool player1Die;
	bool player2Die;
	bool player3Die;
	bool player4Die;
	int actualDeadsPlayers;
	Vector3 player1OriginalPos;
	Vector3 player2OriginalPos;
	Vector3 player3OriginalPos;
	Vector3 player4OriginalPos;

	// Use this for initialization
	void Start () {
		countdownText.text = "";
		player1OriginalPos = new Vector3(player1.transform.position.x,
			player1.transform.position.y,
			player1.transform.position.z);
		player2OriginalPos = new Vector3 (player2.transform.position.x,
			player2.transform.position.y,
			player2.transform.position.z);
		player3OriginalPos = new Vector3(player3.transform.position.x,
			player3.transform.position.y,
			player3.transform.position.z);
		player4OriginalPos = new Vector3 (player4.transform.position.x,
			player4.transform.position.y,
			player4.transform.position.z);
			
		onIdle = 0;
		restartLevel = false;
		player1Wins = 0;
		player2Wins = 0;
		player3Wins = 0;
		player4Wins = 0;
		//DontDestroyOnLoad (gameObject);
		player1Text.text="Player 1 Wins: "+player1Wins;
		player2Text.text="Player 2 Wins: "+player2Wins;
		player3Text.text="Player 3 Wins: "+player3Wins;
		player4Text.text="Player 4 Wins: "+player4Wins;
		actualDeadsPlayers = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (restartLevel) {
			if(onIdle>0.0f && onIdle<1.0f){
				countdownText.text="3";
			}else if(onIdle>1.0f && onIdle<2.0f){
				countdownText.text="2";
			}else if(onIdle>2.0f && onIdle<3.0f){
				countdownText.text="1";
			}else if(onIdle>3.0f){
				countdownText.text="GO!";
			}
			onIdle += Time.unscaledDeltaTime;
			if (onIdle >= deadIdleTime) {
				player1.gameObject.SetActive (true);
				player2.gameObject.SetActive (true);
				player3.gameObject.SetActive (true);
				player4.gameObject.SetActive (true);
				player1.transform.position =player1OriginalPos;
				player2.transform.position =player2OriginalPos;
				player1.GetComponent<PlayerLife> ().ResetLife ();
				player2.GetComponent<PlayerLife> ().ResetLife ();
				player3.transform.position =player3OriginalPos;
				player4.transform.position =player4OriginalPos;
				player3.GetComponent<PlayerLife> ().ResetLife ();
				player4.GetComponent<PlayerLife> ().ResetLife ();
				Time.timeScale = 1.0f;
				onIdle = 0.0f;
				actualDeadsPlayers = 0;
				restartLevel = false;
				countdownText.text="";
			}
		}
		
	}

	public void PlayerDeath(GameObject player){
		actualDeadsPlayers++;
		string playerName = player.name;
		switch (playerName) {
			case "Player1":
				player1Die = true;
				break;
			case "Player2":
				player2Die = true;
				break;
			case "Player3":
				player3Die = true;
				break;
			case "Player4":
				player4Die = true;
				break;
		}
		if (actualDeadsPlayers >= 3) {
			if (!player1Die) {
				player1.gameObject.SetActive (false);
				player1Wins++;
				Time.timeScale = 0.7f;
				restartLevel = true;
			} else if (!player2Die) {
				player2.gameObject.SetActive (false);
				player2Wins++;
				Time.timeScale = 0.7f;
				restartLevel = true;
			} else if (!player3Die) {
				player3.gameObject.SetActive (false);
				player3Wins++;
				Time.timeScale = 0.7f;
				restartLevel = true;
			} else if (!player4Die) {
				player4.gameObject.SetActive (false);
				player4Wins++;
				Time.timeScale = 0.7f;
				restartLevel = true;
			} else {
				Time.timeScale = 0.7f;
				restartLevel = true;
			}

			UpdateScores ();
		}
	}

	void UpdateScores(){

		player1Text.text="Player 1 Wins: "+player1Wins;
		player2Text.text="Player 2 Wins: "+player2Wins;
		player3Text.text="Player 3 Wins: "+player3Wins;
		player4Text.text="Player 4 Wins: "+player4Wins;
	}
}
