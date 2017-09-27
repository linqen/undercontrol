
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : MonoBehaviour {
	private PlayerPreview preview;
	private PlayerInput playerInput;

	void Awake(){
	}

	public PlayerPreview Preview {
		get {
			return preview;
		}
		set {
			preview = value;
		}
	}
	public PlayerInput PlayerInput {
		get {
			//if (playerInput == null)
			//	playerInput = GetComponent<PlayerInput> ();
			return playerInput;
		}
		set {
			playerInput = value;
		}
	}

}
