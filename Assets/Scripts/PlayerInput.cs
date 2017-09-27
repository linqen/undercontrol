using UnityEngine;

public class PlayerInput : MonoBehaviour {

	string start;
	string horizontal;
	string vertical;
	string jump;
	string fire;
	public int inputNumber;

	public PlayerInput (int rinputNumber){
		inputNumber = rinputNumber;
		start = Inputs.Start + inputNumber;
		horizontal = Inputs.Horizontal + inputNumber;
		vertical = Inputs.Vertical + inputNumber;
		jump = Inputs.Jump + inputNumber;
		fire = Inputs.Fire + inputNumber;
	}
	public int GetInputNumber(){
		return inputNumber;
	}
}
