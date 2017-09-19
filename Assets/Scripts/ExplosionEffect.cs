using UnityEngine;

public class ExplosionEffect : MonoBehaviour {
	Renderer rend;
	Color color;
	public float swapColorTime;
	float swappingTime;
	bool swapTime;
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
		color = rend.material.color;
		color.a = 0.0f;
		swappingTime = 0;
		swapTime = false;
	}
	
	// Update is called once per frame
	void Update () {
		swappingTime += Time.deltaTime*0.5f;
		if (swapTime) {
			//color.a = Mathf.PingPong (Time.time , 1.0f);
			//rend.material.color = color;



			float pongTime = Mathf.PingPong (Time.time, 1.0f);
			color.a = pongTime;
			rend.material.color = color;
		}
	}

	public void StartSwap(){
		swapTime = true;
	}
}
