using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour {
	public float moveVelocity;
	public float jumpForce;

	private Rigidbody2D rigid;
	private Vector3 lastDirection;
	private Vector3 lastVelocity;
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
		rigid.freezeRotation=true;
		if (Input.GetButtonDown (input.Jump)) {jump = true;} 
		else {jump = false;}
	}

	void FixedUpdate(){
		//lastVelocity = rigid.velocity;
		if (horizontalAxis > 0.3f) {
			//rigid.AddForce (Vector2.right * moveVelocity, ForceMode2D.Force);
			rigid.velocity = new Vector2 (1 * moveVelocity, rigid.velocity.y);
			lastDirection = Vector3.right;
			//StartCoroutine(Move(Vector2.right));
		} else if (horizontalAxis < -0.3f) {
			//rigid.AddForce (Vector2.left * moveVelocity, ForceMode2D.Force);
			rigid.velocity = new Vector2 (-1 * moveVelocity, rigid.velocity.y);
			lastDirection = Vector3.left;
			//StartCoroutine(Move(Vector2.left));
		} else {
			
			//rigid.velocity = lastVelocity;
		}
		if (jump) {
			if (grounded) {
				rigid.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
				grounded = false;
			} else if (canDoubleJump) {
				//rigid.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
				//canDoubleJump = false;
			}
		}
	}

//	IEnumerator Move(Vector2 direction){
//		lastVelocity = rigid.velocity;
//		rigid.velocity = new Vector2 (direction.x * moveVelocity, rigid.velocity.y*rigid.gravityScale);
//		yield return new WaitForEndOfFrame();
//		rigid.velocity = new Vector2(lastVelocity.x,lastVelocity.y);
//	}

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
