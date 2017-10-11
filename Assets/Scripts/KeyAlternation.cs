using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyAlternation : MonoBehaviour {
	public List<Sprite> keys=new List<Sprite>();
	public float timeBetweenAlternation;

	private float currentTime=0;
	private Image image;
	private int actualPosition;

	void Awake(){
		image = GetComponent<Image> ();
	}

	void Start () {
		if (keys [0] != null) {
			image.sprite = keys [0];
		}
		actualPosition = 0;
	}
	
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime >= timeBetweenAlternation) {
			if (actualPosition + 1 == keys.Count || keys.Count == actualPosition) {
				actualPosition = 0;
			} else {
				actualPosition++;
			}
			image.sprite = keys [actualPosition];
			currentTime = 0;
		}
	}

	public void SetKeys(List<Sprite> availableKeys){
		keys = availableKeys;
	}
}
