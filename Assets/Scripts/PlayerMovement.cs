using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovement : MonoBehaviour {
	public float moveVelocity;
	public float jumpForce;
	public float timeBeforeStopJumping;
	public float jumpingTime;
	public float movementSlowAffectedByExplocion;

	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rigid;
	private Vector3 lastDirection;
	private Vector3 lastVelocity;
	private float verticalAxis;
	private float horizontalAxis;
	private Vector2 explosionForce=Vector2.zero;
	private bool jump=false;
	private bool grounded=true;
	private bool canJump = true;
	private float jumpingSince=0;
	private GameManager gameManager;
	private PlayerInput input;
	private List<GameObject> lastCollisionGameObject = new List<GameObject>();


	void Awake () {
		input = GetComponent<PlayerInput> ();
		rigid = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start(){
	}

	void Update(){
		horizontalAxis = Input.GetAxisRaw (input.Horizontal);
		verticalAxis = Input.GetAxisRaw(input.Vertical);
		if (Input.GetButton (input.Jump)) {jump = true;}else {jump = false;}
		if (Input.GetButtonUp (input.Jump) && jumpingSince != 0.0f) {jumpingSince = jumpingTime;} 
		else if (Input.GetButtonUp (input.Jump)) {canJump = true;}
	}


	void FixedUpdate(){
		rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
		if (horizontalAxis > 0.1f) {
			animator.SetBool ("IsRunning", true);
			spriteRenderer.flipX = false;
			lastDirection = Vector3.right;
		} else if (horizontalAxis < -0.1f) {
			animator.SetBool ("IsRunning", true);
			spriteRenderer.flipX = true;
			lastDirection = Vector3.left;
		} else {
			animator.SetBool ("IsRunning", false);
		}
		if (jump) {
			if (jumpingTime > jumpingSince&&canJump==true) {
				jumpingSince += Time.deltaTime;
				rigid.velocity = new Vector2 (rigid.velocity.x, jumpForce * (jumpingTime - jumpingSince));
				animator.SetBool ("IsJumping", true);
			} else if (jumpingTime <= jumpingSince) {
				canJump = false;
			}
		}

		if (!grounded) {
			rigid.velocity = new Vector2 (rigid.velocity.x, rigid.velocity.y + (-rigid.gravityScale*rigid.mass));
		}
		if (explosionForce != Vector2.zero) {
			rigid.velocity = rigid.velocity / movementSlowAffectedByExplocion + explosionForce;
		}
	}

	public IEnumerator AddExplosionForce(Vector2 direction, float timeExploding){
		float currentTime = 0;
		while (currentTime <= timeExploding) {

			explosionForce = direction;
			currentTime += Time.fixedDeltaTime;
			yield return null;
		}
		explosionForce = Vector2.zero;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")&& jump && !grounded) {
			jumpingSince = jumpingTime;
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			grounded = true;
			if (!jump) {
				canJump = true;
			}
			animator.SetBool ("IsJumping", false);
			lastCollisionGameObject.Add(col.gameObject);
			if (gameObject.activeSelf) {
				StopCoroutine ("ExitGroundJumpChance");
				jumpingSince=0;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			lastCollisionGameObject.Remove (col.gameObject);
			if(lastCollisionGameObject.Count!=0){
				return;
			}
			grounded = false;
			if (gameObject.activeSelf && jumpingSince==0.0f) {
				StartCoroutine (ExitGroundJumpChance (timeBeforeStopJumping));
			}
		}
	}
	IEnumerator ExitGroundJumpChance(float time){
		yield return new WaitForSeconds (time);
		if (gameObject.activeSelf && jumpingSince == 0.0f) {
			//Animation isFalling true on a future
			animator.SetBool ("IsJumping", true);
			jumpingSince = jumpingTime;
		}
	}

	public IEnumerator CharSelection(){
		PlayerPreview pp = GetComponent<PlayerPreview>();
		int actualInput = GetComponent<PlayerInput>().GetInputNumber ();
		gameManager = GameManager.Instance;
		while (true) {
			if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				!pp.selected) {
				//Move Right
				gameManager.GetNextUnusedPlayer(pp);
			} 
			else if (Input.GetButtonDown(Inputs.Horizontal+actualInput) &&
				!pp.selected) {
				//Move Left
				gameManager.GetPreviousUnusedPlayer(pp);
			}
			//Joystick case
			if (Input.GetAxisRaw (Inputs.Horizontal + actualInput) > 0.5f &&
				!Input.GetButton(Inputs.Horizontal+actualInput)&&
				!pp.selected) {
				//Move Right
				gameManager.GetNextUnusedPlayer(pp);
				yield return new WaitForSecondsRealtime (0.5f);
			} 
			else if (Input.GetAxisRaw (Inputs.Horizontal + actualInput) < -0.5f &&
				!Input.GetButton(Inputs.Horizontal+actualInput)&&
				!pp.selected) {
				//Move Left
				gameManager.GetPreviousUnusedPlayer(pp);
				yield return new WaitForSecondsRealtime (0.5f);
			}

			yield return null;
		}
	}

	public void ResetMovement(){
		ResetAnimationStates ();
		canJump = true;
		grounded=true;
	}
	void ResetAnimationStates(){
		animator.SetBool ("IsRunning", false);
		animator.SetBool ("IsJumping", false);
		spriteRenderer.flipX = false;
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
