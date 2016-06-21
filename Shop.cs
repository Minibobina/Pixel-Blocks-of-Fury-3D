using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Soomla;
using Soomla.Store;

public class Shop : MonoBehaviour
{
	public RectTransform AdPurchases;

	void Awake ()
	{
		if(PlayerPrefs.GetInt("AdRemoval") == 1)
		{
			AdPurchases.gameObject.SetActive (false);
		}
	}

	void Start ()
	{
		StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactions;
	}

	public void PurchaseCoins (int amount)
	{
		Soomla.Store.StoreInventory.BuyItem ("com.minibobinastudios.pixelblocksoffury.purchase_" + amount +"_coins");
	}

	public void PurchaseRemoveAds ()
	{
		Soomla.Store.StoreInventory.BuyItem ("com.minibobinastudios.pixelblocksoffury.purchase_remove_ads");
	}

	public void RestorePurchase ()
	{
		SoomlaStore.RestoreTransactions ();
	}

	void onRestoreTransactions (bool success)
	{
		if (success) {
			if (PlayerPrefs.GetInt ("AdRemoval") == 1) {
				AdPurchases.gameObject.SetActive (false);
				Debug.Log("Updating Remove Ad button");
			}
		}
	}
}
