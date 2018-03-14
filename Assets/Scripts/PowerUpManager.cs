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
		int randomSpawnPoint=0;
		int lastRandomSpawnPoint = -1;
		GameObject lastPowerUp = null;
		while (true) {
			currentTime += Time.deltaTime;

			if (currentTime >= spawnTime) {
				while (lastRandomPowerUp == (randomPowerUp = Random.Range (0, powerUpPool.Count))) {
				}

				bool needSearchPowerUpPosition = true;
				List<int> occupiedPositions = null;
				while (needSearchPowerUpPosition) {
					needSearchPowerUpPosition = false;
					randomSpawnPoint = GetRandomSpawnPoint (lastRandomSpawnPoint, occupiedPositions);

					RaycastHit2D[] hits = Physics2D.BoxCastAll (spawnPositions [randomSpawnPoint], new Vector2 (1, 1), 0, Vector2.right);
					for (int i = 0; i < hits.Length; i++) {
						if (hits [i].collider.CompareTag ("Player")) {
							needSearchPowerUpPosition = true;
							if (occupiedPositions == null)
								occupiedPositions = new List<int> ();
							occupiedPositions.Add (randomSpawnPoint);
							if (occupiedPositions.Count == (spawnPositions.Count - 1)) {
								occupiedPositions.Clear ();
								DisableLastPowerUp (lastPowerUp);
								yield return new WaitForSeconds (2.0f);
							}
						}
					}
				}

				GameObject powerUp = powerUpPool [randomPowerUp];
				powerUp.transform.position = spawnPositions [randomSpawnPoint];
				powerUp.SetActive (true);

				DisableLastPowerUp (lastPowerUp);

				currentTime = 0;
				lastPowerUp = powerUp;
				lastRandomPowerUp = randomPowerUp;
				lastRandomSpawnPoint = randomSpawnPoint;
			}
			yield return null;
		}
	}

	private int GetRandomSpawnPoint(int lastRandomSpawnPoint, List<int>occupiedPositions){
		int randomSpawnPoint=0;
		bool shouldContinue = true;
		while (shouldContinue) {
			shouldContinue = false;
			randomSpawnPoint = Random.Range (0, spawnPositions.Count);

			if (lastRandomSpawnPoint == randomSpawnPoint) {
				shouldContinue = true;
			}else if (occupiedPositions != null) {
				for (int i = 0; i < occupiedPositions.Count; i++) {
					if (randomSpawnPoint == occupiedPositions [i])
						shouldContinue = true;
				}
			}


		}
		return randomSpawnPoint;
	}
	
	private void DisableLastPowerUp(GameObject lastPowerUp){
		if (lastPowerUp != null && lastPowerUp.activeSelf == true) {
			lastPowerUp.SetActive (false);
		}
	}

	public void NotifyLevelFinished(){
		if (actualSpawnProcess != null) {
			StopCoroutine (actualSpawnProcess);
			for (int i = 0; i < powerUpPool.Count; i++) {
				powerUpPool [i].SetActive (false);
			}
		}
	}
}
