using UnityEngine;
using System.Collections;

public enum SpawnLocations {
	North,
	North_East,
	East,
	South_East,
	South,
	South_West,
	West,
	North_West
};

public class Enemy : MonoBehaviour 
{
	private NavMeshAgent agent;
	private Animator animator;
	private CapsuleCollider capCol;
	//private Rigidbody rig;
	private Transform myTransform;
	private Transform targetTransform;
	private UIManager ui;

	//private SwipeManager swipeManager;
	private Swipe2Test swipeManager;
	private SpawnManager spawnManager;
	
	public SpawnLocations spawnCardinal;
	public GameObject deathEffect;

	private bool isKillable = false;
	private bool isDead = false;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		capCol = GetComponent<CapsuleCollider>();
		//rig = GetComponent<Rigidbody>();
		ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManager>();
		targetTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

		// Editor
		//swipeManager = Camera.main.GetComponent<SwipeManager>();
		// Moblie
		swipeManager = Camera.main.GetComponent<Swipe2Test>();

		spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();

		myTransform = transform;
		swipeManager.killLocations[(int)spawnCardinal].objects.Add(this.gameObject);
	}
	public bool getIsKillable
	{
		get {return isKillable;}
	}
	
	void FixedUpdate () 
	{
		if(!isDead)
			MoveToTarget();
		if(Utilities.isGameOver)
			agent.Stop();
	}

	void MoveToTarget ()
	{
		if(targetTransform != null)
		{
			SetNavDestination(targetTransform);
		}
	}

	void SetNavDestination(Transform dest)
	{
		agent.SetDestination(dest.position);
	}

	public void Death ()
	{
		if(PlayerPrefs.GetInt("hasWatchTutorial_2") == 0)
		{
			if(!spawnManager.GetComponent<GameTutorial>().dontAllowToBeKilled)
				spawnManager.GetComponent<GameTutorial>().NextSection();
		}
		ui.IBeenKilled();
		animator.SetBool("isKilled",true);
		isDead = true;
		agent.Stop();
		capCol.enabled = false;
		agent.radius = 0;
		agent.height = 0;
		StartCoroutine(TriggerDeath());
	}

	IEnumerator TriggerDeath ()
	{
		yield return new WaitForSeconds(2.5f);

		Instantiate(deathEffect,transform.position,transform.rotation);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if(other.name == "Tile_1" && isKillable == false)
		{
			//Debug.Log(other.name);

			if(PlayerPrefs.GetInt("hasWatchTutorial_2") == 0)
			{
				// We are in tutorial mode, dont allow this enemy to move anymore.
				agent.Stop();

				if(spawnManager.GetComponent<GameTutorial>().dontAllowToBeKilled == false)
					isKillable = true;
				else
					Debug.Log("Tutorial non swipeable minion, have to use spell!");
			}
			else
			{
				isKillable = true;
			}

		}
		if(other.name == "GameOver" && Utilities.isGameOver == false)
		{
			Debug.Log("Enemy Hit the base!");
			ui.GameOver();
		}
	}
}
