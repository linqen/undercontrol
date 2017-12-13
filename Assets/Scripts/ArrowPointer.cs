using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour {
	public GameObject[] arrowPrefabs = new GameObject[4];

	GameObject[] arrowsPool = new GameObject[4];
	List<GameObject> mplayers;
	List<Vector2> bypassInternalPosition=new List<Vector2>();
	List<bool> hasBypassExternal=new List<bool>();

	void Update(){
		for (int i = 0; i < hasBypassExternal.Count; i++) {
			if (hasBypassExternal [i].Equals (true)) {
				if (mplayers [i].activeSelf) {
					var dir = mplayers [i].transform.position - arrowsPool [i].transform.position;
					var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
					arrowsPool [i].transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
				} else {
					hasBypassExternal [i] = false;
					arrowsPool [i].SetActive (false);
				}
			}
		}
	}

	public void SetPlayers(List<GameObject> players){
		mplayers = players;
		for (int i = 0; i < mplayers.Count; i++) {
			hasBypassExternal.Add (false);
			bypassInternalPosition.Add (Vector2.zero);
			arrowsPool [i] = Instantiate (arrowPrefabs [i],transform);
			arrowsPool [i].SetActive (false);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.CompareTag("Player")){
			for (int i = 0; i < mplayers.Count; i++) {
				if (mplayers [i].Equals (col.gameObject)) {
					if (hasBypassExternal [i].Equals(true)) {
						hasBypassExternal [i] = false;
						arrowsPool [i].SetActive (false);
					} else {
						bypassInternalPosition [i] = mplayers [i].transform.position;
					}
				}
			}
		}
	}

	public void ExternalOnTriggerEnter2D(Collider2D col){
		if (col.gameObject.CompareTag ("Player")) {
			for (int i = 0; i < mplayers.Count; i++) {
				if (mplayers [i].Equals (col.gameObject) && hasBypassExternal [i].Equals(false)) {
					hasBypassExternal [i] = true;
					float xSignal = 0;
					if (col.transform.position.x < 0) {
						xSignal = -1;
					} else {
						xSignal = 1;
					}
					arrowsPool [i].transform.position = new Vector2 (bypassInternalPosition [i].x - (arrowsPool [i].transform.GetChild(0).GetComponent<SpriteRenderer> ().sprite.bounds.extents.x * xSignal),
						bypassInternalPosition [i].y - (arrowsPool [i].transform.GetChild(0).GetComponent<SpriteRenderer> ().sprite.bounds.extents.y));
					arrowsPool [i].SetActive (true);
				}
			}
		}
	}
}
