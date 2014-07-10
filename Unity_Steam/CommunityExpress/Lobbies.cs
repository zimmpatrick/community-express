/*
 * Community Express SDK
 * http://www.communityexpresssdk.com/
 *
 * Copyright (c) 2011-2014, Zimmdot, LLC
 * All rights reserved.
 *
 * Subject to terms and condition provided in LICENSE.txt
 * Dual licensed under a Commercial Development and LGPL licenses.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// Creates list of lobbies
    /// </summary>
	public class Lobbies : ICollection<Lobby>
	{
		private List<Lobby> _lobbyList = new List<Lobby>();
        /// <summary>
        /// Number of lobbies in list
        /// </summary>
		public int Count
		{
			get { return _lobbyList.Count; }
		}
        /// <summary>
        /// If list is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds lobby to list
        /// </summary>
        /// <param name="item">Lobby to add</param>
		public void Add(Lobby item)
		{
			_lobbyList.Add(item);
		}
        /// <summary>
        /// Clears list
        /// </summary>
		public void Clear()
		{
			_lobbyList.Clear();
		}
        /// <summary>
        /// Checks if list contains certain lobby
        /// </summary>
        /// <param name="item">Lobby to check for</param>
        /// <returns>true if found</returns>
		public bool Contains(Lobby item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies list to index
        /// </summary>
        /// <param name="array">Array of lobbies</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Lobby[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes lobby from list
        /// </summary>
        /// <param name="item">Lobby to remove</param>
        /// <returns>true if removed</returns>
		public bool Remove(Lobby item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for list
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<Lobby> GetEnumerator()
		{
			return new ListEnumerator<Lobby>(_lobbyList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
