using UnityEngine;

public class PlayerInput : MonoBehaviour {

	string start;
	string horizontal;
	string vertical;
	string jump;
	string fire;
	public int inputNumber;

	public void SetInputNumber(int rinputNumber){
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

	public string Start {
		get {
			return this.start;
		}
	}

	public string Horizontal {
		get {
			return this.horizontal;
		}
	}

	public string Vertical {
		get {
			return this.vertical;
		}
	}

	public string Jump {
		get {
			return this.jump;
		}
	}

	public string Fire {
		get {
			return this.fire;
		}
	}
}
