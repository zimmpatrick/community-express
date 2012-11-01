// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class Servers : ICollection<Server>
	{
		private List<Server> _serverList = new List<Server>();

		public int Count
		{
			get { return _serverList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(Server item)
		{
			_serverList.Add(item);
		}

		public void Clear()
		{
			_serverList.Clear();
		}

		public bool Contains(Server item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Server[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(Server item)
		{
			throw new NotSupportedException();
		}

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
