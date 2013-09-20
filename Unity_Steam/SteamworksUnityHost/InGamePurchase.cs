using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about a purchase
    /// </summary>
	public class InGamePurchase
	{
        /// <summary>
        /// Description of the items purchased
        /// </summary>
		public class Item
		{
            /// <summary>
            /// Item ID
            /// </summary>
			public int ID;
            /// <summary>
            /// Item quantity
            /// </summary>
			public int Quantity;
            /// <summary>
            /// Cost per item
            /// </summary>
			public double AmountPerItem;
            /// <summary>
            /// Item description
            /// </summary>
			public String Description;
            /// <summary>
            /// Item category
            /// </summary>
			public String Category;
            /// <summary>
            /// Item information
            /// </summary>
            /// <param name="id">Item ID</param>
            /// <param name="quantity">Item quantity</param>
            /// <param name="amountPerItem">Cost per item</param>
            /// <param name="description">Item description</param>
            /// <param name="category">Item category</param>
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
        /// <summary>
        /// Add item to purchase
        /// </summary>
        /// <param name="item">Item to be added</param>
		public void AddItem(Item item)
		{
			_itemList.Add(item);
		}
        /// <summary>
        /// Information on item to be added
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="amountPerItem">Cost per item</param>
        /// <param name="description">Item description</param>
        /// <param name="category">Item category</param>
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
        /// <summary>
        /// ID of the purchase order
        /// </summary>
		public UInt64 OrderID
		{
			get { return _orderID; }
			set { _orderID = value; }
		}
        /// <summary>
        /// List of items in purchase
        /// </summary>
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
