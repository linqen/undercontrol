using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
public class PlayerMovement : MonoBehaviour {
	public float moveVelocity;
	public float jumpForce;
	public float timeBeforeStopJumping;
	public float jumpingTime;
	public float movementSlowAffectedByExplocion;
	public float analogDeadZone;

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
	private bool isHoldingThrowButton = false;
	private bool isHanging=false;
	private bool touchingWallAtLeft=false;
	private bool touchingWallAtRight=false;
	private bool isCharSelection=false;
	private GameManager gameManager;
	private AudioManager audioManager;
	private int inputNumber;
	private Coroutine exitGroundJump=null;
	Vector2 oldPos;
	void OnEnable(){
		explosion = Vector2.zero;
	}

	void Awake () {
		inputNumber = GetComponent<PlayerInput> ().inputNumber;
		rigid = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start(){
		audioManager = AudioManager.Instance;
		InputManager.Devices [inputNumber].LeftStickX.LowerDeadZone = analogDeadZone;
		InputManager.Devices [inputNumber].LeftStickY.LowerDeadZone = analogDeadZone;
	}

	void Update(){

		//To avoid teleport between colliders
		RaycastHit2D hit = Physics2D.Linecast(oldPos, transform.position);
		if(hit!=null&&hit.collider != null&&((hit.collider.gameObject.CompareTag("Ground")||hit.collider.gameObject.CompareTag("Wall")))){
			Vector2 scaleVector = new Vector2 (transform.position.normalized.x * (transform.lossyScale.x / 2),
				                      transform.position.normalized.y * (transform.lossyScale.y / 2));
			transform.position = hit.point+scaleVector;
			rigid.velocity=Vector2.Reflect(rigid.velocity,hit.normal);
		}
		oldPos = transform.position;
		//
		horizontalAxis = InputManager.Devices[inputNumber].LeftStickX.GetRawValue();
		verticalAxis = InputManager.Devices[inputNumber].LeftStickY.GetRawValue();
		//horizontalAxis = Input.GetAxisRaw (input.Horizontal);
		//verticalAxis = Input.GetAxisRaw(input.Vertical);
		if(InputManager.Devices[inputNumber].Action2.IsPressed){isHoldingThrowButton = true;}
		else{isHoldingThrowButton = false;}
		if (InputManager.Devices[inputNumber].Action1.IsPressed) {jump = true;}else {jump = false;}
		if (InputManager.Devices[inputNumber].Action1.WasReleased && jumpingSince != 0.0f) {jumpingSince = jumpingTime;} 
		else if (InputManager.Devices[inputNumber].Action1.WasReleased) {canJump = true;}
	}


	void FixedUpdate(){
		if (horizontalAxis > 0.0f && !isHanging && !touchingWallAtRight && (!isHoldingThrowButton || !grounded)) {
			if (explosion == Vector2.zero) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = false;
			lastDirection = Vector3.right;
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
			}
		} else if (horizontalAxis < 0.0f && !isHanging && !touchingWallAtLeft && (!isHoldingThrowButton || !grounded)) {
			if (explosion == Vector2.zero) {
			animator.SetBool ("IsRunning", true);
			//audioManager.PlayerWalking ();
			spriteRenderer.flipX = true;
			lastDirection = Vector3.left;
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
			}
		} else if (!isHanging && !touchingWallAtLeft && !touchingWallAtRight && (!isHoldingThrowButton || !grounded)) {
			rigid.velocity = new Vector2 (horizontalAxis * moveVelocity, rigid.velocity.y);
		} else if (!isHanging && !touchingWallAtLeft && !touchingWallAtRight && isHoldingThrowButton) {
			rigid.velocity = new Vector2 (0 * moveVelocity, rigid.velocity.y);
			if (horizontalAxis > 0.0f) {
				spriteRenderer.flipX = false;
				lastDirection = Vector3.right;
			} else if (horizontalAxis < 0.0f) {
				spriteRenderer.flipX = true;
				lastDirection = Vector3.left;
			}
			animator.SetBool ("IsHoldingThrow", true);
			if (verticalAxis > 0.0f) {
				animator.SetInteger ("HoldingThrowTo", 2);
			} else if (verticalAxis < 0.0f) {
				animator.SetInteger ("HoldingThrowTo", 1);
			} else {
				animator.SetInteger ("HoldingThrowTo", 0);
			}
		}
		if (!isHoldingThrowButton) {
			animator.SetBool ("IsHoldingThrow", false);
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
			animator.SetBool ("IsRunning", false);
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
		isHanging = false;
		animator.SetBool ("IsHanging", isHanging);
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
				if (gameObject.activeSelf && exitGroundJump != null) {
					StopCoroutine (exitGroundJump);
				}
				jumpingSince=0;
			} 
			else if (contact.normal == Vector2.down) {
				ResetHangingValues ();
				jumpingSince = jumpingTime;
				audioManager.PlayerHitRoof ();
			} 
			else{
				float axisRawValue = InputManager.Devices[inputNumber].LeftStickX.GetRawValue();
				if (contact.normal == Vector2.left ) {
					hangingFromEdgeStartValue=axisRawValue;
					hangingFromEdgePreviousValue = hangingFromEdgeStartValue;
					touchingWallAtRight = true;
				} 
				else if (contact.normal == Vector2.right) {
					hangingFromEdgeStartValue=axisRawValue;
					hangingFromEdgePreviousValue = hangingFromEdgeStartValue;
					touchingWallAtLeft = true;
				}
			}
		}
	}

	void OnCollisionStay2D(Collision2D col){
		if (col.gameObject.tag.Equals ("Ground") && hangingFromEdgeStartValue != 0) {
			ContactPoint2D contact = col.contacts [0];

			float axisRawValue = InputManager.Devices[inputNumber].LeftStickX.GetRawValue();
			if (axisRawValue == hangingFromEdgeStartValue) {
				if (contact.normal == Vector2.left
				    && axisRawValue == 1.0f) {
					isHanging = true;
					touchingWallAtRight = true;
					animator.SetBool ("IsHanging", isHanging);
					rigid.position = new Vector3 (col.collider.bounds.min.x - (transform.lossyScale.x / 2),
						col.collider.bounds.min.y + (col.transform.lossyScale.y / 2), col.collider.bounds.min.z);
					rigid.velocity = new Vector2 (rigid.velocity.x, 0);
				} else if (contact.normal == Vector2.right
				           && axisRawValue == -1.0f) {
					isHanging = true;
					touchingWallAtLeft = true;
					animator.SetBool ("IsHanging", isHanging);
					rigid.position = new Vector3 (col.collider.bounds.max.x + (transform.lossyScale.x / 2),
						col.collider.bounds.max.y - (col.transform.lossyScale.y / 2), col.collider.bounds.max.z);
					rigid.velocity = new Vector2 (rigid.velocity.x, 0);
				}
				if (jump && jumpingSince == jumpingTime && !canJump) {
					if (gameObject.activeSelf && exitGroundJump != null) {
						StopCoroutine (exitGroundJump);
					}
					canJump = true;
					jumpingSince = 0;
					isHanging = false;
					animator.SetBool ("IsHanging", isHanging);
					touchingWallAtLeft = false;
					touchingWallAtRight = false;
					hangingFromEdgeStartValue = 0;
				}
			} else {
				isHanging = false;
				animator.SetBool ("IsHanging", isHanging);
				hangingFromEdgePreviousValue = hangingFromEdgeStartValue;
				hangingFromEdgeStartValue = 0;
			}
		} else if (col.gameObject.tag.Equals ("Ground")) {
			float axisRawValue = InputManager.Devices[inputNumber].LeftStickX.GetRawValue();
			if (axisRawValue != 0 && axisRawValue == hangingFromEdgePreviousValue && !jump) {
				hangingFromEdgeStartValue = axisRawValue;
			}
		} else if (col.gameObject.tag.Equals ("Wall")) {
			ContactPoint2D contact = col.contacts [0];
			if (contact.normal == Vector2.left) {
				touchingWallAtRight = true;
			} else if (contact.normal == Vector2.right) {
				touchingWallAtLeft = true;
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
				exitGroundJump = StartCoroutine (ExitGroundJumpChance (timeBeforeStopJumping));
			}
		}else if (col.gameObject.tag.Equals ("Wall")) {
			touchingWallAtRight = false;
			touchingWallAtLeft = false;
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
		//bool canMoveJoySelection = true;
		while (isCharSelection) {
			//float axisRawValue = Input.GetAxisRaw (Inputs.Horizontal + actualInput);
			if (InputManager.Devices[actualInput].DPadRight.WasPressed && !pp.selected) {
				//Move Right
				gameManager.GetNextUnusedPlayer (pp);
			} else if (InputManager.Devices[actualInput].DPadLeft.WasPressed && !pp.selected) {
				//Move Left
				gameManager.GetPreviousUnusedPlayer (pp);
			}
			//else {
			//	//Joystick case
			//	if (axisRawValue > 0.5f && !Input.GetButton (Inputs.Horizontal + actualInput) &&
			//	   !pp.selected && canMoveJoySelection == true) {
			//		//Move Right
			//		gameManager.GetNextUnusedPlayer (pp);
			//		canMoveJoySelection = false;
			//	} else if (axisRawValue < -0.5f &&
			//	          !Input.GetButton (Inputs.Horizontal + actualInput) &&
			//	          !pp.selected && canMoveJoySelection == true) {
			//		//Move Left
			//		gameManager.GetPreviousUnusedPlayer (pp);
			//		canMoveJoySelection = false;
			//	} else if (axisRawValue == 0.0f) {
			//		canMoveJoySelection = true;
			//	}
			//}
			yield return null;
		}
	}

	public void ToSpawnPoint(Vector3 spawnPointPosition){
		transform.position = spawnPointPosition;
		oldPos = transform.position;
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
