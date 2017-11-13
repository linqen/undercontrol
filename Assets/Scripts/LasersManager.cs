﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasersManager : MonoBehaviour {
	public GameObject laserAtLeft;
	public GameObject laserAtRight;
	public float lasersMoveVelocity;

	private AudioManager audioManager;
	private Rigidbody2D laserAtLeftRigid;
	private Rigidbody2D laserAtRightRigid;
	private Vector3 leftLaserStartPosition;
	private Vector3 rightLaserStartPosition;
	private float invert=1;
	private bool suddenDeath=false;

	void Start () {
		audioManager = AudioManager.Instance;
		laserAtLeftRigid = laserAtLeft.GetComponent<Rigidbody2D> ();
		laserAtRightRigid = laserAtRight.GetComponent<Rigidbody2D> ();
		leftLaserStartPosition = laserAtLeft.transform.position;
		rightLaserStartPosition = laserAtRight.transform.position;
	}
	
	void Update () {
		if (suddenDeath) {
			laserAtLeftRigid.position += (Vector2.right * lasersMoveVelocity * Time.deltaTime*invert); 
			laserAtRightRigid.position += (Vector2.left * lasersMoveVelocity * Time.deltaTime*invert); 
		}
	}

	public void Invert(){
		invert *= -1;
		lasersMoveVelocity++;
	}

	public void StartLasers(float time){
		Invoke ("EnableLasers", time);
	}

	void EnableLasers(){
		suddenDeath=true;
		audioManager.InGameLaserMusic ();
	}

	public void RestartLasersPositions(){
		laserAtLeft.transform.position = leftLaserStartPosition;
		laserAtRight.transform.position = rightLaserStartPosition;
	}

	public void DisableLasers(){
		CancelInvoke ();
		suddenDeath=false;
	}
}
