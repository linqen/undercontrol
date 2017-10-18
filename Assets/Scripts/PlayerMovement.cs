using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovement : MonoBehaviour {
	public float moveVelocity;
	public float jumpForce;
	public float timeBeforeStopJumping;

	private Rigidbody2D rigid;
	private Vector3 lastDirection;
	private Vector3 lastVelocity;
	private float verticalAxis;
	private float horizontalAxis;
	private bool jump=false;
	private bool grounded=true;
	List<GameObject> lastCollisionGameObject = new List<GameObject>();

	PlayerInput input;

	void Awake () {
		input = GetComponent<PlayerInput> ();
		rigid = GetComponent<Rigidbody2D> ();
	}

	void Start(){
		rigid.freezeRotation=true;
	}

	void Update(){
		horizontalAxis = Input.GetAxisRaw (input.Horizontal);
		verticalAxis = Input.GetAxisRaw(input.Vertical);
		if (Input.GetButtonDown (input.Jump)) {jump = true;} 
		else {jump = false;}
	}

	void FixedUpdate(){
		rigid.AddForce (horizontalAxis * moveVelocity * Vector2.right, ForceMode2D.Force);
		if (horizontalAxis > 0.3f) {
			lastDirection = Vector3.right;
		} else if (horizontalAxis < -0.3f) {
			lastDirection = Vector3.left;
		}
		if (jump) {
			if (grounded) {
				rigid.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
				grounded = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground") ||
			col.gameObject.tag.Equals ("Player")) {
			lastCollisionGameObject.Add(col.gameObject);
			grounded = true;
			if (gameObject.activeSelf) {
				StopCoroutine ("ExitGroundJumpChance");
			}
		}
	}

	void OnCollisionExit2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")||col.gameObject.tag.Equals ("Player")) {
			lastCollisionGameObject.Remove (col.gameObject);
			if(lastCollisionGameObject.Count!=0){
				return;
			}
			if (gameObject.activeSelf) {
				StartCoroutine (ExitGroundJumpChance (timeBeforeStopJumping));
			}
		}
	}
	IEnumerator ExitGroundJumpChance(float time){
		yield return new WaitForSeconds (time);
		grounded = false;
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
