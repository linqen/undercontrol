
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : MonoBehaviour {
	private PlayerInput playerInput;

	public PlayerInput PlayerInput {
		get {
			return playerInput;
		}
		set {
			playerInput = value;
		}
	}

}
