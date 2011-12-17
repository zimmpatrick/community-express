using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommunityExpressNS
{
	class ListEnumerator<T> : IEnumerator<T>
	{
			private int _index;
			private IList<T> _list;

			public ListEnumerator(IList<T> list)
			{
				_list = list;
				_index = -1;
			}

			public T Current
			{
				get
				{
					return _list[_index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				_index++;
				return _index < _list.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
	}
}
