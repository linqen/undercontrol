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
	private Vector2 explosion = Vector2.zero;
	private bool jump=false;
	private bool grounded=true;
	private bool canJump = true;
	private float jumpingSince=0;
	private GameManager gameManager;
	private AudioManager audioManager;
	private PlayerInput input;
	private List<GameObject> lastCollisionGameObject = new List<GameObject>();
	private float hangingFromEdgeStartValue;
	private float hangingFromEdgePreviousValue;
	private bool isHanging=false;
	void OnEnable(){
		explosion = Vector2.zero;
	}

	void Awake () {
		input = GetComponent<PlayerInput> ();
		rigid = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start(){
		audioManager = AudioManager.Instance;
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
		if (horizontalAxis > 0.0f&&!isHanging) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = false;
			lastDirection = Vector3.right;
		} else if (horizontalAxis < 0.0f&&!isHanging) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = true;
			lastDirection = Vector3.left;
		} else {
			animator.SetBool ("IsRunning", false);
		}
		if (jump) {
			if (jumpingTime > jumpingSince&&canJump==true) {
				if (jumpingSince == 0) {
					audioManager.PlayerJumping ();
				}
				jumpingSince += Time.deltaTime;
				rigid.velocity = new Vector2 (rigid.velocity.x, jumpForce * (jumpingTime - jumpingSince));
				animator.SetBool ("IsJumping", true);
			} else if (jumpingTime <= jumpingSince) {
				canJump = false;
			}
		}

		if (!grounded||!isHanging) {
			rigid.velocity = new Vector2 (rigid.velocity.x, rigid.velocity.y + (-rigid.gravityScale*rigid.mass));
		}
		if (explosion != Vector2.zero) {
			rigid.velocity = rigid.velocity / movementSlowAffectedByExplocion + explosion;
		}
	}


	public void AddExplosionForce(Vector2 direction, float timeExploding, float explosionForce){
		if (gameObject.activeSelf) {
			StartCoroutine (LocalAddExplosionForce (direction, timeExploding, explosionForce));
		}
	}

	private IEnumerator LocalAddExplosionForce(Vector2 direction, float timeExploding, float explosionForce){
		float currentTime = 0;
		while (currentTime < timeExploding) {
			currentTime += Time.fixedDeltaTime;
			explosion = direction * (timeExploding/currentTime) * explosionForce*Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		explosion = Vector2.zero;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			ContactPoint2D contact = col.contacts[0];

			if (contact.normal == Vector2.up) {
				grounded = true;
				audioManager.PlayerTouchFloor ();
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
			else if (contact.normal == Vector2.down) {
				jumpingSince = jumpingTime;
				audioManager.PlayerHitRoof ();
			} 
			else{
				float axisRawValue = Input.GetAxisRaw (input.Horizontal);
				if (contact.normal == Vector2.left && axisRawValue!=0) {
					hangingFromEdgeStartValue=axisRawValue;
				} 
				else if (contact.normal == Vector2.right&& axisRawValue!=0) {
					hangingFromEdgeStartValue=axisRawValue;
				}
			}
		}
	}

	void OnCollisionStay2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground") && hangingFromEdgeStartValue != 0) {
			ContactPoint2D contact = col.contacts [0];

			float axisRawValue = Input.GetAxisRaw (input.Horizontal);
			if (axisRawValue == hangingFromEdgeStartValue) {
				if (contact.normal == Vector2.left
					&& axisRawValue==1.0f) {
					isHanging = true;
					//Animation isHanging true on a future
					transform.position = new Vector3 (col.collider.bounds.min.x - (transform.lossyScale.x / 2),
						col.collider.bounds.min.y+(col.transform.lossyScale.y/2), col.collider.bounds.min.z);
				} else if (contact.normal == Vector2.right
					&& axisRawValue==-1.0f) {
					isHanging = true;
					//Animation isHanging true on a future
					transform.position = new Vector3 (col.collider.bounds.max.x + (transform.lossyScale.x / 2),
						col.collider.bounds.max.y-(col.transform.lossyScale.y/2), col.collider.bounds.max.z);
				}
			} else {
				isHanging = false;
				hangingFromEdgePreviousValue = hangingFromEdgeStartValue;
				hangingFromEdgeStartValue = 0;
			}
		} else {
			float axisRawValue = Input.GetAxisRaw (input.Horizontal);
			if (col.gameObject.tag.Equals ("Ground") &&
				axisRawValue != 0 && axisRawValue==hangingFromEdgePreviousValue) {
				hangingFromEdgeStartValue = Input.GetAxisRaw (input.Horizontal);
			}
		}
	}

	void OnCollisionExit2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			lastCollisionGameObject.Remove (col.gameObject);
			if(lastCollisionGameObject.Count!=0){
				return;
			}
			hangingFromEdgePreviousValue = 0;
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
