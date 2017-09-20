using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[Range(1,4)]
	public int playerNumber;
	public GameObject granadePrefab;
	public float moveVelocity;
	public float jumpForce;
	public Vector3 lastDirection;
	Rigidbody2D rigid;
	float verticalAxis;
	float horizontalAxis;
	string horizontalAxisName;
	string verticalAxisName;
	string fire1Name;
	string jumpName;
	public float granadeCooldown;
	float currentCooldownGranade;
	bool grounded;
	bool canDoubleJump;
	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody2D> ();
		if (playerNumber == 1) {
			horizontalAxisName="Horizontal";
			verticalAxisName="Vertical";
			fire1Name="Fire1";
			jumpName = "Jump";
		} else if (playerNumber == 2) {
			horizontalAxisName="Horizontal2";
			verticalAxisName="Vertical2";
			fire1Name="Fire3";
			jumpName = "Jump2";
		}else if (playerNumber == 3) {
			horizontalAxisName="Horizontal3";
			verticalAxisName="Vertical3";
			fire1Name="Fire5";
			jumpName = "Jump3";
		}else if (playerNumber == 4) {
			horizontalAxisName="Horizontal4";
			verticalAxisName="Vertical4";
			fire1Name="Fire7";
			jumpName = "Jump4";
		}
		grounded = true;
		canDoubleJump = true;
	}

	void FixedUpdate(){
		horizontalAxis = Input.GetAxisRaw (horizontalAxisName);
		verticalAxis = Input.GetAxisRaw(verticalAxisName);

		Debug.Log ("Horizontal "+horizontalAxis+" Vertical "+verticalAxis);
		currentCooldownGranade += Time.deltaTime;
		if (Input.GetButton (fire1Name)||Input.GetButtonUp (fire1Name)) {
			if (Input.GetButtonUp(fire1Name)&&currentCooldownGranade>=granadeCooldown) {
				Vector3 pos = new Vector3 ( transform.position.x+horizontalAxis,
					transform.position.y+1,
					transform.position.z);
				GameObject shoot = Instantiate (granadePrefab, pos, transform.rotation);
				shoot.GetComponent<Rigidbody2D>().AddForce (new Vector2(lastDirection.x*3,verticalAxis+2), ForceMode2D.Impulse);	
				currentCooldownGranade = 0;
			}
			return;
		}
		if ( horizontalAxis>0.3f) {
			rigid.AddForce (Vector2.right * moveVelocity, ForceMode2D.Force);
			lastDirection = Vector3.right;
		}
		if (horizontalAxis<-0.3f) {
			rigid.AddForce (Vector2.left * moveVelocity, ForceMode2D.Force);
			lastDirection = Vector3.left;
		}
		if (Input.GetButtonDown(jumpName)) {
			if (grounded) {
				rigid.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
				grounded = false;
			} else if (canDoubleJump) {
				rigid.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
				canDoubleJump = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground") ||
		   col.gameObject.tag.Equals ("Player")) {
			grounded = true;
			canDoubleJump = true;
		}
	}
}
