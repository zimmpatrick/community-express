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
    /// Server list information
    /// </summary>
	public class Servers : ICollection<Server>
	{
		private List<Server> _serverList = new List<Server>();
        /// <summary>
        /// Number of servers on list
        /// </summary>
		public int Count
		{
			get { return _serverList.Count; }
		}
        /// <summary>
        /// If list is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Add server to list
        /// </summary>
        /// <param name="item">Server to add</param>
		public void Add(Server item)
		{
			_serverList.Add(item);
		}
        /// <summary>
        /// Clears list
        /// </summary>
		public void Clear()
		{
			_serverList.Clear();
		}
        /// <summary>
        /// Checks if list contains specific server
        /// </summary>
        /// <param name="item">Server to check for</param>
        /// <returns>true if server found</returns>
		public bool Contains(Server item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies list to index
        /// </summary>
        /// <param name="array">Array of servers</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(Server[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes server from list
        /// </summary>
        /// <param name="item">Server to remove</param>
        /// <returns>true if server removed</returns>
		public bool Remove(Server item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for list
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<Server> GetEnumerator()
		{
			return new ListEnumerator<Server>(_serverList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
