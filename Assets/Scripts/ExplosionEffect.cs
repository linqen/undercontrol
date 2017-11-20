using UnityEngine;

public class ExplosionEffect : MonoBehaviour {
	Renderer rend;
	Color color;
	public float swapColorTime;
	float swappingTime;
	bool swapTime;

	void Awake(){
		rend = GetComponent<Renderer> ();
	}
	void Start () {
		color = rend.material.color;
		color.a = 0.0f;
		swappingTime = 0;
		swapTime = false;
	}
	
	void Update () {
		if (swapTime) {
			swappingTime += Time.deltaTime;
			float pongTime = Mathf.PingPong (swappingTime, 1.0f);
			color.a = pongTime;
			rend.material.color = color;
			if (swappingTime >= swapColorTime) {
				swapTime = false;
				color.a = 0;
				rend.material.color = color;
			}
		}
	}

	public void StartSwap(){
		swapTime = true;
	}
}
