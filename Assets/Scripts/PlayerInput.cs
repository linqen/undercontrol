using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	public string start;
	public string horizontal;
	public string vertical;
	public string jump;
	public string fire;

	public void SetInput(int inputNumber){
		start = Inputs.Start + inputNumber;
		horizontal = Inputs.Horizontal + inputNumber;
		vertical = Inputs.Vertical + inputNumber;
		jump = Inputs.Jump + inputNumber;
		fire = Inputs.Fire + inputNumber;
	}
}
