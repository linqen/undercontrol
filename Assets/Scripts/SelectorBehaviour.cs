using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectorBehaviour : MonoBehaviour {
	public Sprite greySelector;
	public float timeBetweenMultipleSelectors;
	public bool selected = false;

	private List<Sprite> actualSelectors = new List<Sprite> ();
	private Image image;
	private float actualSelectorTime=0;

	void Awake(){
		image = GetComponent<Image> ();
		image.sprite = greySelector;
	}
	
	void Update () {
		if (actualSelectors.Count > 1 && !selected) {
			actualSelectorTime += Time.deltaTime;
			if (actualSelectorTime >= timeBetweenMultipleSelectors) {
				int pos = actualSelectors.IndexOf (image.sprite);
				if (pos >= (actualSelectors.Count - 1)) {
					pos = 0;
				} else {
					pos++;
				}
				image.sprite = actualSelectors [pos];
				actualSelectorTime = 0;
			}
		} else {
			actualSelectorTime = 0;
		}
	}

	public void AddSelector(Sprite newSelector){
		actualSelectors.Add (newSelector);
		image.sprite = newSelector;
	}
	public void RemoveSelector(Sprite selectorToRemove){
		if (selected) {
			return;
		}
		actualSelectors.Remove (selectorToRemove);
		if (actualSelectors.Count > 0) {
			image.sprite = actualSelectors [0];
			actualSelectorTime = 0;
		} else {
			image.sprite = greySelector;
		}
	}
	public void SelectSelector(Sprite selectorToSelect){
		selected = true;
		image.sprite = selectorToSelect;
	}
	public void ClearValues(){
		actualSelectors.Clear ();
		selected = false;
		actualSelectorTime = 0;
	}
}
