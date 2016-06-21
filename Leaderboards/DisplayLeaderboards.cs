using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DisplayLeaderboards : MonoBehaviour 
{
	public GameObject sampleLabel;
	public Text searchField;
	public List<GameObject> ranks = new List<GameObject>();

	[Space(5)]
	public AudioClip soundClick;
	public Transform soundSource;

	// Use this for initialization
	void Start () 
	{
		if(LeaderboardsHandler.instance.GetHighScoresList.Count > 0)
		{
			ConfiguredLayout();
		}
		else
		{
			RefreshHighScores();
		}
	}

	public void Refresh (Transform button)
	{
		ButtonEffect(button);
		ConfiguredLayout();
	}
	public void Search (Transform button)
	{
		ButtonEffect(button);
		string t = searchField.text.ToLower();

		foreach(GameObject score in ranks)
			Destroy(score);

		ranks.Clear();

		for(int i = 0; i < LeaderboardsHandler.instance.GetHighScoresList.Count; i++)
		{
			if(LeaderboardsHandler.instance.GetHighScoresList[i].username.ToLower() == t)
			{
				GameObject newLabel = Instantiate(sampleLabel) as GameObject;
				LeaderboardComponents sc = newLabel.GetComponent<LeaderboardComponents>();
				sc.m_number.text = (i + 1).ToString();
				sc.m_name.text = LeaderboardsHandler.instance.GetHighScoresList[i].username;
				sc.m_score.text = LeaderboardsHandler.instance.GetHighScoresList[i].score.ToString("00000");
				newLabel.transform.SetParent(this.transform);
				newLabel.transform.localScale = new Vector3(1,1,1);
				// Check Censor / Admin / Controller
				ranks.Add(newLabel);
			}
		}

		if(ranks.Count == 0)
		{
			ConfiguredLayout();
		}

	}

	void RefreshHighScores ()
	{
		StartCoroutine(RefreshScores());
		Debug.Log("Refreshing Highscores, Please wait...");
	}
	
	void ConfiguredLayout()
	{
		foreach(GameObject score in ranks)
			Destroy(score);
		
		ranks.Clear();

		for(int i = 0; i < LeaderboardsHandler.instance.GetHighScoresList.Count; i++)
		{
			GameObject newLabel = Instantiate(sampleLabel) as GameObject;
			LeaderboardComponents sc = newLabel.GetComponent<LeaderboardComponents>();
			sc.m_number.text = (i + 1).ToString();
			sc.m_name.text = LeaderboardsHandler.instance.GetHighScoresList[i].username;
			sc.m_score.text = LeaderboardsHandler.instance.GetHighScoresList[i].score.ToString("00000");
			newLabel.transform.SetParent(this.transform);
			newLabel.transform.localScale = new Vector3(1,1,1);
			// Check Censor / Admin / Controller
			ranks.Add(newLabel);
		}
	}
	
	IEnumerator RefreshScores ()
	{
		yield return StartCoroutine(LeaderboardsHandler.instance.DownloadServerHighScores());
		
		if(LeaderboardsHandler.instance.GetHighScoresList.Count > 0)
		{
			ConfiguredLayout();
		}
		else
		{
			Debug.Log("Looks like we are either not connected to the internet or unable to locate highscores.");
		}
		
	}

	void ButtonEffect (Transform button, bool clickEffect = true)
	{
		//Create an effect
		if (clickEffect == true) StartCoroutine(ClickEffect(button));
		
		//Play a sound from the source
		if (soundSource) if(soundSource.GetComponent<AudioSource>()) soundSource.GetComponent<AudioSource>().PlayOneShot(soundClick);
	}
	
	IEnumerator ClickEffect(Transform button)
	{
		//Register the original size of the object
		Vector3 initScale = button.localScale;
		
		//Resize it to be larger
		button.localScale = initScale * 1.1f;
		
		//Gradually reduce its size back to the original size
		while ( button.localScale.x > initScale.x * 1.01f )
		{
			yield return new WaitForFixedUpdate();
			
			Vector3 temp = button.localScale;
			
			temp.x = button.localScale.x - (1 * Time.deltaTime);
			button.localScale = temp;
		}
		
		//Reset the size to the original
		button.localScale = initScale;
	}
}
