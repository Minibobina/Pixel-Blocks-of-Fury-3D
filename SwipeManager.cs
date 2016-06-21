using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
//public class KillLocations
//{
//	public string name;
//	public List<GameObject> objects = new List<GameObject>();
//}


public enum Swipe {
	None,
	Up,
	Down,
	Left,
	Right,
	UpLeft,
	UpRight,
	DownLeft,
	DownRight
};


public class SwipeManager : MonoBehaviour {
	// Min length to detect the Swipe
	public float MinSwipeLength = 5f;
	
	private Vector2 _firstPressPos;
	private Vector2 _secondPressPos;
	private Vector2 _currentSwipe;
	
	private Vector2 _firstClickPos;
	private Vector2 _secondClickPos;
	
	public static Swipe SwipeDirection;
	public float ReturnForce = 10f;

	// Newly Added
	public List<KillLocations> killLocations = new List<KillLocations>();
	public GameObject ThorHammer;
	private UIManager ui;
	public SpawnManager spawn;
	public GameObject playerController;
	public GameObject deathEffect;
	public Animator animator;

	private void Start () 
	{
		ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManager>();
		//animator = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<Animator>();
	}
	private void Update() {
		DetectSwipe();
	}
	
	public void DetectSwipe() {
		if ( Input.touches.Length > 0 ) {
			Touch t = Input.GetTouch( 0 );
			Debug.Log("Test Delete Me!");
			if ( t.phase == TouchPhase.Began ) {
				_firstPressPos = new Vector2( t.position.x, t.position.y );
			}
			
			if ( t.phase == TouchPhase.Ended ) {
				_secondPressPos = new Vector2( t.position.x, t.position.y );
				_currentSwipe = new Vector3( _secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y );
				
				// Make sure it was a legit swipe, not a tap
				if ( _currentSwipe.magnitude < MinSwipeLength ) {
					SwipeDirection = Swipe.None;
					return;
				}
				
				_currentSwipe.Normalize();
				
				// Swipe up
				if ( _currentSwipe.y > 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f ) {
					SwipeDirection = Swipe.Up;
				}
				// Swipe down
				else if ( _currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f ) {
					SwipeDirection = Swipe.Down;
				}
				// Swipe left
				else if ( _currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f ) {
					SwipeDirection = Swipe.Left;
				}
				// Swipe right
				else if ( _currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f ) {
					SwipeDirection = Swipe.Right;
				}
				// Swipe up left
				else if ( _currentSwipe.y > 0 && _currentSwipe.x < 0 ) {
					SwipeDirection = Swipe.UpLeft;
				}
				// Swipe up right
				else if ( _currentSwipe.y > 0 && _currentSwipe.x > 0 ) {
					SwipeDirection = Swipe.UpRight;
				}
				// Swipe down left
				else if ( _currentSwipe.y < 0 && _currentSwipe.x < 0 ) {
					SwipeDirection = Swipe.DownLeft;
					
					// Swipe down right
				} else if ( _currentSwipe.y < 0 && _currentSwipe.x > 0 ) {
					SwipeDirection = Swipe.DownRight;
				}
			}
		} else {
			if ( Input.GetMouseButtonDown( 0 ) ) {
				_firstClickPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
			} else {
				SwipeDirection = Swipe.None;
				//Debug.Log ("None");
			}
			if ( Input.GetMouseButtonUp( 0 ) ) {
				_secondClickPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
				_currentSwipe = new Vector3( _secondClickPos.x - _firstClickPos.x, _secondClickPos.y - _firstClickPos.y );
				
				// Make sure it was a legit swipe, not a tap
				if ( _currentSwipe.magnitude < MinSwipeLength ) {
					SwipeDirection = Swipe.None;
					return;
				}
				
				_currentSwipe.Normalize();
				
				//Swipe directional check
				// Swipe up
				if ( _currentSwipe.y > 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f ) {
					SwipeDirection = Swipe.Up;
					//Debug.Log( "Up" );
					ui.SwipeDirection("Up");
					KillAllEnemyAtDirection(0,"Up");
				}
				// Swipe down
				else if ( _currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f ) {
					SwipeDirection = Swipe.Down;
					//Debug.Log( "Down" );
					ui.SwipeDirection("Down");
					KillAllEnemyAtDirection(4,"Down");
				}
				// Swipe left
				else if ( _currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f ) {
					SwipeDirection = Swipe.Left;
					//Debug.Log( "Left" );
					ui.SwipeDirection("Left");
					KillAllEnemyAtDirection(6,"Left");
				}
				// Swipe right
				else if ( _currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f ) {
					SwipeDirection = Swipe.Right;
					//Debug.Log( "Right" );
					ui.SwipeDirection("Right");
					KillAllEnemyAtDirection(2,"Right");
				}     // Swipe up left
				else if ( _currentSwipe.y > 0 && _currentSwipe.x < 0 ) {
					SwipeDirection = Swipe.UpLeft;
					//Debug.Log( "UpLeft" );
					ui.SwipeDirection("UpLeft");
					KillAllEnemyAtDirection(7,"UpLeft");
					
				}
				// Swipe up right
				else if ( _currentSwipe.y > 0 && _currentSwipe.x > 0 ) {
					SwipeDirection = Swipe.UpRight;
					//Debug.Log( "UpRight" );
					ui.SwipeDirection("UpRight");
					KillAllEnemyAtDirection(1,"UpRight");
				}
				// Swipe down left
				else if ( _currentSwipe.y < 0 && _currentSwipe.x < 0 ) {
					SwipeDirection = Swipe.DownLeft;
					//Debug.Log( "DownLeft" );
					ui.SwipeDirection("DownLeft");
					KillAllEnemyAtDirection(5,"DownLeft");
					// Swipe down right
				} else if ( _currentSwipe.y < 0 && _currentSwipe.x > 0 ) {
					SwipeDirection = Swipe.DownRight;
					//Debug.Log( "DownRight" );
					ui.SwipeDirection("DownRight");
					KillAllEnemyAtDirection(3,"DownRight");
				}
			}
		}
	}

	public void KillAllEnemyAtDirection(int index, string direction)
	{
		for(int i = 0; i < killLocations[index].objects.Count; i++)
		{
			Enemy enemy = killLocations[index].objects[i].GetComponent<Enemy>();

			if(enemy != null)
			{
				if(enemy.getIsKillable == true)
				{
					Debug.Log("Setting Bool to true");
					animator.Play("standing_melee_attack_downward");
					//animator.SetBool("isAttacking",true);
					//StartCoroutine(delayAnimation(0.5f));
					enemy.Death();
					Instantiate(deathEffect,enemy.transform.position,Quaternion.FromToRotation(Vector3.back, transform.forward));
					killLocations[index].objects.RemoveAt(i);
					// Move Player To that direction
					playerController.transform.LookAt(EndPosition(direction));
					//playerController.transform.rotation = Quaternion.Slerp(playerController.transform.rotation, EndPosition(direction), Time.deltaTime * 10.0f);
				}
				else
					Debug.Log ("There are no objects of direction: " + (SpawnLocations)index + " that are killable.");
			}
		}
	}

	public Transform EndPosition (string name)
	{
		switch(name)
		{
		case "Up":
			return spawn.spawnLocations[0].myTransform;
		case "Down":
			return spawn.spawnLocations[4].myTransform;
		case "Left":
			return spawn.spawnLocations[5].myTransform;
		case "Right":
			return spawn.spawnLocations[2].myTransform;
		case "UpLeft":
			return spawn.spawnLocations[6].myTransform;
		case "UpRight":
			return spawn.spawnLocations[1].myTransform;
		case "DownLeft":
			return spawn.spawnLocations[5].myTransform;
		case "DownRight":
			return spawn.spawnLocations[3].myTransform;
		default:
			Debug.Log("I shouldnt be called, look at varable 'name'");
			return spawn.spawnLocations[4].myTransform;
		}
	}

	IEnumerator delayAnimation (float time)
	{
		Debug.Log("Waiting Time");
		yield return new WaitForSeconds(time);
		animator.SetBool("isAttacking",false);
		Debug.Log("Setting to false");
	}
	public void KillAll ()
	{
		for(int i = 0; i < killLocations.Count; i++)
		{
			for(int j = 0; j < killLocations[i].objects.Count; j++)
			{
				//Instantiate(ThorHammer,killLocations[i].objects[j].gameObject.transform.position,killLocations[i].objects[j].gameObject.transform.rotation);
				killLocations[i].objects[j].GetComponent<Enemy>().Death();
				killLocations[i].objects.RemoveAt(j);
			}
		}
	}

	public void SetEnemySpeed (float amount)
	{
		for(int i = 0; i < killLocations.Count; i++)
		{
			for(int j = 0; j < killLocations[i].objects.Count; j++)
			{
				killLocations[i].objects[j].GetComponent<NavMeshAgent>().speed = amount;
			}
		}

		spawn.prefab.GetComponent<NavMeshAgent>().speed = amount;
	}
}