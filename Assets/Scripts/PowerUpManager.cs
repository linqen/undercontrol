using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : GenericSingletonClass<PowerUpManager> {

	public List<GameObject> powerUpPrefabs = new List<GameObject>();
	public float spawnTime;

	List<GameObject> powerUpPool = new List<GameObject> ();
	List<Vector3> spawnPositions = new List<Vector3>();
	Coroutine actualSpawnProcess;
	new void Awake(){
		base.Awake ();
	}

	void Start(){
		for (int i = 0; i < powerUpPrefabs.Count; i++) {
			GameObject go = Instantiate (powerUpPrefabs [i]);
			powerUpPool.Add (go);
		}
	}

	public void NotifyLevelStart(){
		Transform spawnPointsParent;
		spawnPointsParent = GameObject.FindGameObjectWithTag ("PowerUpSpawns").transform;
		spawnPositions.Clear ();
		foreach (Transform spawnPoint in spawnPointsParent) {
			spawnPositions.Add (spawnPoint.position);
		}

		actualSpawnProcess = StartCoroutine (SpawnProcess ());
	}

	private IEnumerator SpawnProcess(){
		float currentTime=0;
		int randomPowerUp;
		int lastRandomPowerUp = -1;
		int randomSpawnPoint;
		int lastRandomSpawnPoint = -1;
		GameObject lastPowerUp = null;
		while (true) {
			currentTime += Time.deltaTime;

			if (currentTime >= spawnTime) {
				while (lastRandomPowerUp == (randomPowerUp = Random.Range (0, powerUpPool.Count))) {
				}
				while (lastRandomSpawnPoint == (randomSpawnPoint = Random.Range (0, spawnPositions.Count))) {
				}
				GameObject powerUp = powerUpPool [randomPowerUp];
				powerUp.transform.position = spawnPositions [randomSpawnPoint];
				powerUp.SetActive (true);

				if (lastPowerUp != null && lastPowerUp.activeSelf == true) {
					lastPowerUp.SetActive (false);
				}


				currentTime = 0;
				lastPowerUp = powerUp;
				lastRandomPowerUp = randomPowerUp;
				lastRandomSpawnPoint = randomSpawnPoint;
			}
			yield return null;
		}
	}

	public void NotifyLevelFinished(){
		if (actualSpawnProcess != null) {
			StopCoroutine (actualSpawnProcess);
		}
	}
}
