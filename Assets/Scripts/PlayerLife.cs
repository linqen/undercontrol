using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public int totalLife;
	[Range(1,4)]
	public int playerNumber;
	int actualLife;
	public GameManager gameManager;
	Color color;
	Renderer rend;
	bool canDead;
	// Use this for initialization
	void Start () {

		gameManager = GameManager.Instance;

		canDead = true;
		actualLife = totalLife;
		rend = GetComponent<Renderer> ();
		if (playerNumber == 1) {
			color.b = 1.0f;
			color.g = 0;
			color.r = 0;
		} else if (playerNumber == 2) {
			color.g = 1.0f;
			color.b = 0;
			color.r = 0;
		}else if (playerNumber == 3) {
			color.g = 0.4f;
			color.b = 0.6f;
			color.r = 0;
		}else if (playerNumber == 4) {
			color.g = 0.9f;
			color.b = 0.5f;
			color.r = 0;
		}

		rend.material.color = color;
	}
	public void NotifyHit(int damage){
		actualLife -= damage;
		Debug.Log (actualLife);
		if (actualLife == 2) {
			color.r = 0.5f;
			rend.material.color = color;
		} else if (actualLife == 1) {
			color.r = 1.0f;
			rend.material.color = color;
		}

		if (actualLife <= 0&&canDead) {
			canDead = false;
			gameManager.ReportDeath (gameObject);
			//Death ();
		}

	}

	private void Death(){
		color.r = 1.0f;
		if (playerNumber == 1) {
			color.b = 0;
		} else if (playerNumber == 2) {
			color.g = 0;
		}else if (playerNumber == 3) {
			color.g = 0;
			color.b = 0;
		}else if (playerNumber == 4) {
			color.g = 0;
			color.b = 0;
		}
		rend.material.color = color;
		//gameManager.PlayerDeath (gameObject);
		gameObject.SetActive (false);
	}

	public void ResetLife(){
		canDead = true;
		actualLife = totalLife;
		if (playerNumber == 1) {
			color.b = 1.0f;
		} else if (playerNumber == 2) {
			color.g = 1.0f;
		}else if (playerNumber == 3) {
			color.g = 0.4f;
			color.b = 0.6f;
		}else if (playerNumber == 4) {
			color.g = 0.9f;
			color.b = 0.5f;
		}
		color.r = 0;
		rend.material.color = color;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("LevelLimit")) {
			gameManager.ReportDeath (gameObject);
		}
	}
}
