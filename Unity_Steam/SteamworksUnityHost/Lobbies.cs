// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class Lobbies : ICollection<Lobby>
	{
		private List<Lobby> _lobbyList = new List<Lobby>();

		public int Count
		{
			get { return _lobbyList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Lobby item)
		{
			_lobbyList.Add(item);
		}

		public void Clear()
		{
			_lobbyList.Clear();
		}

		public bool Contains(Lobby item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Lobby[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Lobby item)
		{
			throw new NotSupportedException();
		}

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
