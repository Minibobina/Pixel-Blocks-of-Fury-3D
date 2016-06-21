using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardComponents : MonoBehaviour 
{
	public Text m_number = null;
	public Text m_name = null;
	public Text m_score = null;
	[Space(5)]
	public Color AdminColor = new Color(0.9f, 0f, 0.1f, 0.9f);
	public Color ModeratorColor = new Color(0f, 0.1f, 0.9f, 0.9f);

	public void isAdmin()
	{
		name += " <color=red>[Admin]</color>";
		m_number.color = AdminColor;
		m_name.color = AdminColor;
		m_score.color = AdminColor;
	}

	public void isBanned()
	{
		name += " <color=red>[Banned]</color>";
		Debug.Log("Detect a Ban");
	}
	
	public void isModerator()
	{
		name += " <color=#4E8DE6>[Moderator]</color>";
		m_number.color = ModeratorColor;
		m_name.color = ModeratorColor;
		m_score.color = ModeratorColor;
	}
}
