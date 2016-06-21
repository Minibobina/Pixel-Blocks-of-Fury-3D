using UnityEngine;
using System.IO;
using System.Collections;

public class Player : MonoBehaviour 
{
	public GameObject Head;
	public GameObject Body;
	public GameObject[] Helment;
	public Block[] Sides;

	public Texture2D TestSkin;
	public GameObject firework;

	void Awake () 
	{
		if(PlayerPrefs.GetString("accountName") != "" || PlayerPrefs.GetInt("rememberSkin") == 1)
		{
			var bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/accountskin.png");
			var tex = new Texture2D(62, 32);
			tex.filterMode = FilterMode.Point;
			tex.LoadImage(bytes);
			UpdateModel(tex);
		}
		else
			UpdateModel(TestSkin);
	}

	public void UpdateModel (Texture2D importedTexture)
	{
		Body.GetComponent<Renderer>().material.mainTexture = importedTexture;
		Head.GetComponent<Renderer>().material.mainTexture = importedTexture;
		ApplyTextures();
	}

	public void FireworkAnimation ()
	{
		GetComponent<Animator>().SetBool("Fireworks",true);
		StartCoroutine(waitForAnimation(1.0f,"Fireworks"));
		StartCoroutine(FireworkDelay());
	}

	IEnumerator waitForAnimation (float time, string name)
	{
		yield return new WaitForSeconds(time);
		GetComponent<Animator>().SetBool(name,false);
	}

	IEnumerator FireworkDelay ()
	{
		yield return new WaitForSeconds(0.2f);
		Instantiate(firework,new Vector3(transform.position.x,transform.position.y + 3.5f,transform.position.z),Quaternion.identity);
	}

	void ApplyTextures (int offset = 0)
	{
		Texture2D tempTexture = Body.GetComponent<Renderer>().material.mainTexture as Texture2D;

		for(int i = 0; i < Helment.Length; i++)
		{
			Texture2D exportTexture = new Texture2D(8,8,TextureFormat.RGBA32, false);
			exportTexture.filterMode = FilterMode.Point;
			
			Color[] pix = tempTexture.GetPixels(Sides[i].offSetX, offset + Sides[i].offSetY,8,8);
			
			exportTexture.SetPixels(pix);
			exportTexture.Apply();
			
			Helment[i].GetComponent<Renderer>().material.mainTexture = exportTexture;
		}
	}
}
