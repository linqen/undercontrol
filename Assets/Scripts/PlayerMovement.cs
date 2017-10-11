using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public float moveVelocity;
	public float jumpForce;

	private Rigidbody2D rigid;
	private Vector3 lastDirection;
	private float verticalAxis;
	private float horizontalAxis;
	private bool jump=false;
	private bool grounded=true;
	private bool canDoubleJump=true;

	PlayerInput input;

	void Awake () {
		input = GetComponent<PlayerInput> ();
		rigid = GetComponent<Rigidbody2D> ();
	}

	void Update(){
		horizontalAxis = Input.GetAxisRaw (input.Horizontal);
		verticalAxis = Input.GetAxisRaw(input.Vertical);
		if (Input.GetButtonDown (input.Jump)) {jump = true;} 
		else {jump = false;}
	}

	void FixedUpdate(){
		if ( horizontalAxis>0.3f) {
			rigid.AddForce (Vector2.right * moveVelocity, ForceMode2D.Force);
			lastDirection = Vector3.right;
		}
		if (horizontalAxis<-0.3f) {
			rigid.AddForce (Vector2.left * moveVelocity, ForceMode2D.Force);
			lastDirection = Vector3.left;
		}
		if (jump) {
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

	//Getters
	public Vector3 LastDirection {
		get {
			return this.lastDirection;
		}
	}
	public float VerticalAxis {
		get {
			return this.verticalAxis;
		}
	}
	
	public float HorizontalAxis {
		get {
			return this.horizontalAxis;
		}
	}
}
