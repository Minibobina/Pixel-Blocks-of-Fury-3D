using System;
using UnityEngine;
using System.Collections;

public class DailyBonus : MonoBehaviour 
{
	void Awake () 
	{
		if(!PlayerPrefs.HasKey("oldDate"))
			PlayerPrefs.SetString("oldDate", System.DateTime.Now.ToBinary().ToString());

		CheckDailyReward();
	}

	void CheckDailyReward() 
	{
		//Store the current time when it starts
		DateTime currentDate = System.DateTime.Now;
		Debug.Log("currentDate: " + currentDate);

		//Grab the old time from the player prefs as a long
		long temp = Convert.ToInt64(PlayerPrefs.GetString("oldDate"));

		//Convert the old time from binary to a DataTime variable
		DateTime oldDate = DateTime.FromBinary(temp);
		Debug.Log("oldDate: " + oldDate);
		
		//Use the Subtract method and store the result as a timespan variable
		TimeSpan difference = currentDate.Subtract(oldDate);
		print("Difference: " + difference);
		print("Total Days difference " + (int)difference.TotalDays);
		// Send message to daily reward cointainer, only change time when reward is clicked

		if((int)difference.TotalDays >= 1)
			this.PostNotification("Daily Reward", new MessageEventArgs("A day has passed, send user daily reward"));
		else
			this.gameObject.SetActive(false);
	}

	void OnEnable ()
	{
		this.AddObserver(OnNotification, "Daily Reward Accepted");
	}
	
	void OnDisable ()
	{
		this.RemoveObserver(OnNotification, "Daily Reward Accepted");
	}
	
	void OnNotification (object sender, EventArgs e)
	{
		Debug.Log("Recieved Message: " + ((MessageEventArgs)e).message);
		PlayerPrefs.SetString("oldDate", System.DateTime.Now.ToBinary().ToString());
	}
}
