﻿using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour {
	public float explodeTime;
	public float explosionForcePlayers;
	public float explosionForceGrenades;
	public float dieTime;
	public float timeExploding;
	public float radius;
	//A bigger value means distance affects less to the final force
	[Range(1.0f,10.0f)]
	public float reduceForceByDistanceEffect;
	public Sprite[] grenades;

	Animator animator;
	float timeLived;
	int throwedByPlayerNumber;
	int grenadeOfCharacterNumber;
	bool explode=false;
	Vector2 oldPos;
	Rigidbody2D myRigid;
	SpriteRenderer spriteRender;

	void Awake(){
		spriteRender = GetComponent<SpriteRenderer> ();
		myRigid = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	void Start () {
		timeLived = 0.0f;
		oldPos = transform.position;
		AkSoundEngine.SetSwitch ("Granade", "PreBomb", gameObject);
		AkSoundEngine.PostEvent ("Granade", gameObject);
		//spriteRender.sprite = grenades [grenadeOfCharacterNumber-1];
		animator.SetInteger("CharacterNumber", grenadeOfCharacterNumber);
	}

	void FixedUpdate(){
		//To avoid teleport between colliders
		RaycastHit2D hit = Physics2D.Linecast(oldPos, transform.position);
		if(hit!=null&&((hit.collider.CompareTag("Ground")||hit.collider.CompareTag("Wall")))){
			transform.position = hit.point;
			myRigid.velocity=Vector2.Reflect(myRigid.velocity,hit.normal);
		}
		oldPos = transform.position;
		//
	}

	void Update () {
		timeLived += Time.deltaTime;
		if (timeLived >= explodeTime) {
			if (explode == false) {
				Explode ();
				GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, 0);
			}
			explode = true;
		}
		if (timeLived >= explodeTime + dieTime) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag.Equals ("Laser")&&explode==false) {
			timeLived = explodeTime;
		}
	}

	void Explode(){
		AkSoundEngine.SetSwitch ("Granade", "Bomb", gameObject);
		AkSoundEngine.PostEvent ("Granade", gameObject);
		animator.SetBool("Twinkle", false);
		myRigid.constraints = RigidbodyConstraints2D.FreezeAll;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, radius);
		for (int i = 0; i < cols.Length; i++) {
			Rigidbody2D rigid = cols[i].GetComponent<Rigidbody2D> ();
			if (rigid != null) {
				Vector3 explodeDirection = rigid.transform.position - transform.position;
				float hitPowerForce = radius-(explodeDirection.magnitude/reduceForceByDistanceEffect)+cols[i].bounds.extents.magnitude;
				if (cols[i].tag.Equals ("Player")) {
					cols[i].GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					cols[i].GetComponent<PlayerMovement> ().AddExplosionForce (explodeDirection.normalized * hitPowerForce, timeExploding, explosionForcePlayers);
				} else if(cols[i].tag.Equals("Grenade")) {
					rigid.AddForce (explodeDirection.normalized * hitPowerForce * explosionForceGrenades, ForceMode2D.Impulse);
				}
			}
		}
	}

	public int GrenadeOfCharacterNumber {
		get {
			return this.grenadeOfCharacterNumber;
		}
		set {
			grenadeOfCharacterNumber = value;
		}
	}
		
	public int ThrowedByPlayerNumber {
		get {
			return this.throwedByPlayerNumber;
		}
		set {
			throwedByPlayerNumber = value;
		}
	}
}
