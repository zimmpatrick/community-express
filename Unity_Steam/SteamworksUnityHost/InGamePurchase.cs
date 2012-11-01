using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class InGamePurchase
	{
		public class Item
		{
			public int ID;
			public int Quantity;
			public double AmountPerItem;
			public String Description;
			public String Category;

			public Item(Int32 id, int quantity, double amountPerItem, String description, String category = "")
			{
				ID = id;
				Quantity = quantity;
				AmountPerItem = amountPerItem;
				Description = description;
				Category = category;
			}

			internal int Amount
			{
				get { return (int)((double)Quantity * AmountPerItem * 100.0); }
			}
		}

		private bool _useTestMode;
		private String _webAPIKey;
		private UInt64 _orderID;
		private List<Item> _itemList;
		private OnInGamePurchaseComplete _inGamePurchaseCompleteCallback;

		internal InGamePurchase(bool useTestMode, String webAPIKey, UInt64 orderID)
		{
			_useTestMode = useTestMode;
			_webAPIKey = webAPIKey;
			_orderID = orderID;
			_itemList = new List<Item>();
		}

		public void AddItem(Item item)
		{
			_itemList.Add(item);
		}

		public void AddItem(Int32 id, int quantity, double amountPerItem, String description, String category = "")
		{
			_itemList.Add(new Item(id, quantity, amountPerItem, description, category));
		}

		internal bool UseTestMode
		{
			get { return _useTestMode; }
		}

		internal String WebAPIKey
		{
			get { return _webAPIKey; }
		}

		public UInt64 OrderID
		{
			get { return _orderID; }
			set { _orderID = value; }
		}

		public IList<Item> ItemList
		{
			get { return _itemList; }
		}

		internal OnInGamePurchaseComplete InGamePurchaseCompleteCallback
		{
			get { return _inGamePurchaseCompleteCallback; }
			set { _inGamePurchaseCompleteCallback = value; }
		}
	}
}
