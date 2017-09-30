using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[Range(1,4)]
	public int playerNumber;
	public GameObject granadePrefab;
	public float moveVelocity;
	public float jumpForce;
	public Vector3 lastDirection;
	public float granadeCooldown;

	Rigidbody2D rigid;
	float verticalAxis;
	float horizontalAxis;
	float currentCooldownGranade;
	bool grounded;
	bool canDoubleJump;

	PlayerInput input;

	void Start () {
		input = GetComponent<PlayerInput> ();
		rigid = GetComponent<Rigidbody2D> ();
		grounded = true;
		canDoubleJump = true;
	}

	void FixedUpdate(){
		horizontalAxis = Input.GetAxisRaw (input.Horizontal);
		verticalAxis = Input.GetAxisRaw(input.Vertical);

		Debug.Log ("Horizontal "+horizontalAxis+" Vertical "+verticalAxis);
		currentCooldownGranade += Time.deltaTime;
		if (Input.GetButton (input.Fire)||Input.GetButtonUp (input.Fire)) {
			if (Input.GetButtonUp(input.Fire)&&currentCooldownGranade>=granadeCooldown) {
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
		if (Input.GetButtonDown(input.Jump)) {
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
