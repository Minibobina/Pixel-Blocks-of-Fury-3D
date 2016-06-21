using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Locations
{
	public string name;
	public Transform myTransform;
}

public class SpawnManager : MonoBehaviour 
{
	public List<Locations> spawnLocations = new List<Locations>();
	public GameObject prefab;
	public GameObject effect;
	public float repeatTime = 1.5f;

	void Start () {
		//InvokeRepeating("SpawnEnemy",0.5f,repeatTime);
		if(PlayerPrefs.GetInt("hasWatchTutorial_2") == 0)
		{
			Debug.Log("Start Tutorial");
		}
		else
		{
			StartCoroutine(SpawnEnemy());
		}
	}

	public void StartLevel ()
	{
		StartCoroutine(SpawnEnemy());
	}

	IEnumerator SpawnEnemy () {
		yield return new WaitForSeconds(0.5f);

		int ran = Random.Range(0,spawnLocations.Count);
		prefab.GetComponent<Enemy>().spawnCardinal = (SpawnLocations)ran;

		// Is this really needed to make it look better??
		Vector3 temp = spawnLocations[ran].myTransform.position;
		temp.y += 0.5f;

		Instantiate(effect,temp,spawnLocations[ran].myTransform.rotation);
		Instantiate(prefab,spawnLocations[ran].myTransform.position,spawnLocations[ran].myTransform.rotation);

		yield return new WaitForSeconds(repeatTime);

		if(!Utilities.isGameOver)
			StartCoroutine(SpawnEnemy());
	}
}
