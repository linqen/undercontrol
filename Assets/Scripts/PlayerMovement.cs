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
	private Vector3 lastDirection = Vector3.right;
	private Vector3 lastVelocity;
	private Vector2 explosion = Vector2.zero;
	private List<GameObject> lastCollisionGameObject = new List<GameObject>();
	private float verticalAxis;
	private float horizontalAxis;
	private float hangingFromEdgeStartValue;
	private float hangingFromEdgePreviousValue;
	private float jumpingSince=0;
	private bool jump=false;
	private bool grounded=true;
	private bool canJump = true;
	private bool canMove = true;
	private bool isHanging=false;
	private bool touchingWallAtLeft=false;
	private bool touchingWallAtRight=false;
	private bool isCharSelection=false;
	private GameManager gameManager;
	private AudioManager audioManager;
	private PlayerInput input;
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
		if(Input.GetButton(input.Fire)){canMove = false;}
		else{canMove = true;}
		if (Input.GetButton (input.Jump)) {jump = true;}else {jump = false;}
		if (Input.GetButtonUp (input.Jump) && jumpingSince != 0.0f) {jumpingSince = jumpingTime;} 
		else if (Input.GetButtonUp (input.Jump)) {canJump = true;}
	}


	void FixedUpdate(){
		if (horizontalAxis > 0.0f && !isHanging && !touchingWallAtRight && canMove) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = false;
			lastDirection = Vector3.right;
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
		} else if (horizontalAxis < 0.0f && !isHanging && !touchingWallAtLeft && canMove) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = true;
			lastDirection = Vector3.left;
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
		} else if (!isHanging && !touchingWallAtLeft && !touchingWallAtRight && canMove) {
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
		} else if (!isHanging && !touchingWallAtLeft && !touchingWallAtRight && !canMove) {
			rigid.velocity = new Vector2 (0 * moveVelocity, rigid.velocity.y);
			if (horizontalAxis > 0.0f) {
				spriteRenderer.flipX = false;
				lastDirection = Vector3.right;
			} else if (horizontalAxis < 0.0f) {
				spriteRenderer.flipX = true;
				lastDirection = Vector3.left;
			}
		}

		if(horizontalAxis == 0.0f || isHanging || touchingWallAtLeft || touchingWallAtRight) {
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

		if (!grounded && !isHanging) {
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
		CancelHangingByExplosion ();
		while (currentTime < timeExploding) {
			currentTime += Time.fixedDeltaTime;
			explosion = direction * (timeExploding/currentTime) * explosionForce*Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		explosion = Vector2.zero;
	}

	private void CancelHangingByExplosion(){
		hangingFromEdgeStartValue = 0;
		hangingFromEdgePreviousValue = 0;
		animator.SetBool ("IsHanging", isHanging);
		isHanging = false;
		touchingWallAtLeft = false;
		touchingWallAtRight = false;
		//Animation explosion force
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			ContactPoint2D contact = col.contacts[0];
			lastCollisionGameObject.Add(col.gameObject);
			if (contact.normal == Vector2.up) {
				ResetHangingValues ();
				grounded = true;
				audioManager.PlayerTouchFloor ();
				if (!jump) {
					canJump = true;
				}
				animator.SetBool ("IsJumping", false);
				if (gameObject.activeSelf) {
					StopCoroutine ("ExitGroundJumpChance");
					jumpingSince=0;
				}
			} 
			else if (contact.normal == Vector2.down) {
				ResetHangingValues ();
				jumpingSince = jumpingTime;
				audioManager.PlayerHitRoof ();
			} 
			else{
				float axisRawValue = Input.GetAxisRaw (input.Horizontal);
				if (contact.normal == Vector2.left ) {
					hangingFromEdgeStartValue=axisRawValue;
					touchingWallAtRight = true;
				} 
				else if (contact.normal == Vector2.right) {
					hangingFromEdgeStartValue=axisRawValue;
					touchingWallAtLeft = true;
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
					touchingWallAtRight = true;
					animator.SetBool ("IsHanging", isHanging);
					rigid.position = new Vector3 (col.collider.bounds.min.x - (transform.lossyScale.x / 2),
						col.collider.bounds.min.y+(col.transform.lossyScale.y/2), col.collider.bounds.min.z);
					rigid.velocity = new Vector2 (rigid.velocity.x, 0);
				} else if (contact.normal == Vector2.right
					&& axisRawValue==-1.0f) {
					isHanging = true;
					touchingWallAtLeft = true;
					animator.SetBool ("IsHanging", isHanging);
					rigid.position = new Vector3 (col.collider.bounds.max.x + (transform.lossyScale.x / 2),
						col.collider.bounds.max.y-(col.transform.lossyScale.y/2), col.collider.bounds.max.z);
					rigid.velocity = new Vector2 (rigid.velocity.x, 0);
				}
				if (jump&&jumpingSince==jumpingTime&&!canJump) {
					if (gameObject.activeSelf) {
						StopCoroutine ("ExitGroundJumpChance");
					}
					canJump = true;
					jumpingSince = 0;
					isHanging = false;
					animator.SetBool ("IsHanging", isHanging);
					touchingWallAtLeft = false;
					touchingWallAtRight = false;
					hangingFromEdgeStartValue = 0;
					hangingFromEdgePreviousValue = 0;
				}
			} else {
				isHanging = false;
				animator.SetBool ("IsHanging", isHanging);
				hangingFromEdgePreviousValue = hangingFromEdgeStartValue;
				hangingFromEdgeStartValue = 0;
			}
		} else if (col.gameObject.tag.Equals ("Ground")) {
			float axisRawValue = Input.GetAxisRaw (input.Horizontal);
			if (axisRawValue != 0 && axisRawValue==hangingFromEdgePreviousValue) {
				hangingFromEdgeStartValue = axisRawValue;
			}
		}
	}
	private void ResetHangingValues(){
		touchingWallAtLeft = false;
		touchingWallAtRight = false;
		hangingFromEdgePreviousValue = 0;
	}
	void OnCollisionExit2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground")) {
			lastCollisionGameObject.Remove (col.gameObject);
			if(lastCollisionGameObject.Count!=0){
				return;
			}
			grounded = false;
			if (touchingWallAtLeft || touchingWallAtRight) {
				ResetHangingValues ();
				hangingFromEdgeStartValue = 0;
				isHanging = false;
				animator.SetBool ("IsHanging", isHanging);
			}
			else if (gameObject.activeSelf && jumpingSince==0.0f) {
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
	public void StopCharSelection(){
		isCharSelection = false;
	}

	public IEnumerator CharSelection(){
		PlayerPreview pp = GetComponent<PlayerPreview>();
		int actualInput = GetComponent<PlayerInput>().GetInputNumber ();
		isCharSelection = true;
		gameManager = GameManager.Instance;
		bool canMoveJoySelection = true;
		while (isCharSelection) {
			float axisRawValue = Input.GetAxisRaw (Inputs.Horizontal + actualInput);
			if (Input.GetButtonDown (Inputs.Horizontal + actualInput) &&
				!pp.selected && axisRawValue > 0.5f) {
				//Move Right
				gameManager.GetNextUnusedPlayer (pp);
			} else if (Input.GetButtonDown (Inputs.Horizontal + actualInput) &&
				!pp.selected && axisRawValue < 0.5f) {
				//Move Left
				gameManager.GetPreviousUnusedPlayer (pp);
			} else {
				//Joystick case
				if (axisRawValue > 0.5f && !Input.GetButton (Inputs.Horizontal + actualInput) &&
				   !pp.selected && canMoveJoySelection == true) {
					//Move Right
					gameManager.GetNextUnusedPlayer (pp);
					canMoveJoySelection = false;
				} else if (axisRawValue < -0.5f &&
				          !Input.GetButton (Inputs.Horizontal + actualInput) &&
				          !pp.selected && canMoveJoySelection == true) {
					//Move Left
					gameManager.GetPreviousUnusedPlayer (pp);
					canMoveJoySelection = false;
				} else if (axisRawValue == 0.0f) {
					canMoveJoySelection = true;
				}
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
