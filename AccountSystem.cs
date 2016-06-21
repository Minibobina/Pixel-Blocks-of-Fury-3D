using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

public class HatOffset
{
	public int _x;
	public int _y;

	public HatOffset (int x, int y)
	{
		_x = x;
		_y = y;
	}
}

public class AccountSystem : MonoBehaviour 
{
	public InputField userName;
	public Toggle saveName;
	public Text sendInfo;
	public Player model;

	public Text userAccountName;
	public EasyTween infoAnimation;

	public bool isInfoPanelOpen = false;
	private string lastSearch = "Minibobina";
	private string defaultText = "If you own a minecraft account enter your account name into the textfield, if found sucessfully your minecraft skin will be imported. You may choose to remember your account name, it will be used for Leaderboards and Achievements.";

	[Space(5)]
	public EasyTween[] easyTweenAnimations;
	public Toggle keepSkin;
	public MainMenuButtons mm;

	public void Login (Transform button)
	{
		if(mm.IsPanelOpen == false){
			mm.ButtonEffect(button);
			easyTweenAnimations[0].OpenCloseObjectAnimation();
			mm.IsPanelOpen = true;
		}
	}
	public void CloseAccountPanel (Transform button)
	{
		if(mm.IsPanelOpen){
		if(button!=null)mm.ButtonEffect(button);
		easyTweenAnimations[0].OpenCloseObjectAnimation();
		mm.IsPanelOpen = false;
		}
	}

	public void ConfirmLogout (Transform button)
	{
		if(!mm.IsPanelOpen){
			mm.ButtonEffect(button);
			easyTweenAnimations[1].OpenCloseObjectAnimation();
		}
	}
	public void Logout (Transform button)
	{
		mm.ButtonEffect(button);
		PlayerPrefs.SetString("accountName","");
		userAccountName.text = PlayerPrefs.GetString("accountName");
		easyTweenAnimations[1].OpenCloseObjectAnimation();
		mm.IsPanelOpen = false;

		if(!keepSkin.isOn)
		{
			model.UpdateModel(model.TestSkin);
			PlayerPrefs.SetInt("rememberSkin",0);
		}
		else
		{
			PlayerPrefs.SetInt("rememberSkin",1);
		}
	}

	public void Search () 
	{
		if(userName.text != "" && lastSearch != userName.text)
		{
			StartCoroutine(FindAccount(userName.text.ToString()));
		}
		else
		{
			if(lastSearch == userName.text)
				Debug.Log(lastSearch +  " == " + userName.text);
			else
				SendInfoToUser("Send Error, username is empty.");
		}
	}
	
	IEnumerator FindAccount (string accountName)
	{
		Debug.Log ("ERROR CHECK [ URL ]");
		string url = "https://skins.minecraft.net/MinecraftSkins/" + accountName + ".png";
		lastSearch = userName.text;

		if(isInfoPanelOpen) // Closing Info Panel Please Wait
		{
			infoAnimation.OpenCloseObjectAnimation();
		}

		Debug.Log ("ERROR CHECK [ Close Panel... ]");
		yield return new WaitForSeconds(1.25f);

		Debug.Log ("ERROR CHECK [ WWW (URL) ]");

		//url = url.Replace(" ","%20");// this is extra line
		WWW www = new WWW(url);

//		while (!www.isDone) {
//			Debug.Log ("Download Percent: " + www.progress);
//		}
		yield return www;

		Debug.Log ("ERROR CHECK [ WWW DONE ]");

		if(www.isDone && string.IsNullOrEmpty(www.error))
		{
			Debug.Log ("ERROR CHECK [ WWW PASSED ]");
			Texture2D tex = www.texture;
			tex.filterMode = FilterMode.Point;
			
			if(tex.height == 32 && tex.width == 64) 
			{
				Debug.Log("Old Style Format");

				// Check for hat texture weirdness
				tex = HatTextureFix32(tex);
				tex.filterMode = FilterMode.Point;

				var bytes = tex.EncodeToPNG();
				File.WriteAllBytes(Application.persistentDataPath + "/accountskin.png", bytes);
				model.UpdateModel(tex);
				lastSearch = "Minibobina";
			}
			else if(tex.height == 64 && tex.width == 64)
			{
				Debug.Log("New Style Format 1.8+");
				Texture2D fix = new Texture2D(64,32);
				fix.SetPixels(tex.GetPixels(0,32,64,32));
				fix.Apply();
				fix.filterMode = FilterMode.Point;

				// Check for hat texture weirdness
				fix = HatTextureFix64(fix);
				fix.filterMode = FilterMode.Point;

				var bytes = fix.EncodeToPNG();
				File.WriteAllBytes(Application.persistentDataPath + "/accountskin.png", bytes);
				model.UpdateModel(fix);
				lastSearch = "Minibobina";
			}
			else
			{
				Debug.Log ("ERROR CHECK [ WWW FAILED : TEX FORMAT ]");
				SendInfoToUser("Youre minecraft account name was found but, the texture format does not match the 64x32 or 64x64 standered format, please send us feedback about this error.");
				return false;
			}

			RememberAccount(accountName);
			CloseAccountPanel(null);
		}
		else
		{
			Debug.Log ("ERROR CHECK [ WWW FAILED ]");
			SendInfoToUser("Unable to find account with account name: " + accountName);
			CloseAccountPanel(null);
		}
	}

	Texture2D HatTextureFix32 (Texture2D tex)
	{
		List<HatOffset> temp = new List<HatOffset> ();
		temp.Add (new HatOffset (40, 24));
		temp.Add (new HatOffset (48, 24));
		temp.Add (new HatOffset (32, 16));
		temp.Add (new HatOffset (40, 16));
		temp.Add (new HatOffset (48, 16));
		temp.Add (new HatOffset (56, 16));

		bool isAllBlack = true;
		bool isAllWhite = true;

		for (int i = 0; i < temp.Count; i++) 
		{
			Color[] pixels = tex.GetPixels (temp[i]._x, temp[i]._y, 8, 8);

			foreach(Color p in pixels)
			{
				if (p.b == 1 && p.g == 1 && p.r == 1)
				{
					Debug.Log ("This pixel is white");
				}
				else
				{
					Debug.Log ("This pixel is something else");
					isAllWhite = false;
				}

				if (p.b == 0 && p.g == 0 && p.r == 0)
				{
					Debug.Log ("This pixel is black");
				}
				else
				{
					Debug.Log ("This pixel is something else");
					isAllBlack = false;
				}
			}
		}

		if (isAllWhite) {
			Debug.Log ("This has to be an error with the texture as all items are white.");

			for (int j = 0; j < temp.Count; j++) 
			{
				Color [] textureColors = tex.GetPixels (temp[j]._x, temp[j]._y, 8, 8);

				//set all pixel's alpha channel to 0
				for(int index = 0; index< textureColors.Length; index++)
				{
					textureColors[index].a = 0;    
				}

				tex.SetPixels(temp[j]._x, temp[j]._y, 8, 8, textureColors);
			}

			tex.Apply();
		}

		if (isAllBlack) {
			Debug.Log ("This has to be an error with the texture as all items are black.");
			
			for (int j = 0; j < temp.Count; j++) 
			{
				Color [] textureColors = tex.GetPixels (temp[j]._x, temp[j]._y, 8, 8);
				
				//set all pixel's alpha channel to 0
				for(int index = 0; index< textureColors.Length; index++)
				{
					textureColors[index].a = 0;    
				}
				
				tex.SetPixels(temp[j]._x, temp[j]._y, 8, 8, textureColors);
			}
			
			tex.Apply();
		}

		return tex;
	}
	Texture2D HatTextureFix64 (Texture2D tex)
	{
		List<HatOffset> temp = new List<HatOffset> ();
		temp.Add (new HatOffset (40, 56));
		temp.Add (new HatOffset (48, 56));
		temp.Add (new HatOffset (32, 48));
		temp.Add (new HatOffset (40, 48));
		temp.Add (new HatOffset (48, 48));
		temp.Add (new HatOffset (56, 48));
		
		bool isAllBlack = true;
		bool isAllWhite = true;
		
		for (int i = 0; i < temp.Count; i++) 
		{
			Color[] pixels = tex.GetPixels (temp[i]._x, temp[i]._y, 8, 8);
			
			foreach(Color p in pixels)
			{
				if (p.b == 1 && p.g == 1 && p.r == 1)
				{
					Debug.Log ("This pixel is white");
				}
				else
				{
					Debug.Log ("This pixel is something else");
					isAllWhite = false;
				}
				
				if (p.b == 0 && p.g == 0 && p.r == 0)
				{
					Debug.Log ("This pixel is black");
				}
				else
				{
					Debug.Log ("This pixel is something else");
					isAllBlack = false;
				}
			}
		}
		
		if (isAllWhite) {
			Debug.Log ("This has to be an error with the texture as all items are white.");
			
			for (int j = 0; j < temp.Count; j++) 
			{
				Color [] textureColors = tex.GetPixels (temp[j]._x, temp[j]._y, 8, 8);
				
				//set all pixel's alpha channel to 0
				for(int index = 0; index< textureColors.Length; index++)
				{
					textureColors[index].a = 0;    
				}
				
				tex.SetPixels(temp[j]._x, temp[j]._y, 8, 8, textureColors);
			}
			
			tex.Apply();
		}
		
		if (isAllBlack) {
			Debug.Log ("This has to be an error with the texture as all items are black.");
			
			for (int j = 0; j < temp.Count; j++) 
			{
				Color [] textureColors = tex.GetPixels (temp[j]._x, temp[j]._y, 8, 8);
				
				//set all pixel's alpha channel to 0
				for(int index = 0; index< textureColors.Length; index++)
				{
					textureColors[index].a = 0;    
				}
				
				tex.SetPixels(temp[j]._x, temp[j]._y, 8, 8, textureColors);
			}
			
			tex.Apply();
		}
		
		return tex;
	}

	void SendInfoToUser (string infoText)
	{
		isInfoPanelOpen = true;
		sendInfo.text = infoText;
		infoAnimation.OpenCloseObjectAnimation();
	}

	IEnumerator SetDefaultText ()
	{
		yield return new WaitForSeconds(1);
		sendInfo.text = defaultText;
		isInfoPanelOpen = false;
	}

	public void GetInfo ()
	{
		if(lastSearch != userName.text)
		{
			if(isInfoPanelOpen)
				StartCoroutine(SetDefaultText());
			else
				isInfoPanelOpen = true;

			infoAnimation.OpenCloseObjectAnimation();
		}
		else
		{
			infoAnimation.OpenCloseObjectAnimation();
			isInfoPanelOpen = false;
		}

	}

	void RememberAccount (string accountName)
	{
		if(saveName.isOn)
			PlayerPrefs.SetString("accountName",accountName);
		else
		
			PlayerPrefs.SetString("accountName","");

		userAccountName.text = PlayerPrefs.GetString("accountName");
	}
}
