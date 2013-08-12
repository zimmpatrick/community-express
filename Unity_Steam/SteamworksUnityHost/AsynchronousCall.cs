using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	using SteamAPICall_t = UInt64;

    internal interface IAsynchronousCall
    {
        object Sender
        {
            get;
        }

        void Complete(object e);
    }

    public class AsynchronousCall<T, E> where E : class
    {
        private class AsynchronousCallImp : IAsynchronousCall
        {
            private AsynchronousCall<T, E> _ac;

            public AsynchronousCallImp(AsynchronousCall<T, E> ac)
            {
                _ac = ac;
            }

            public void Complete(object e)
            {
                _ac.Complete(e as E);
            }

            public object Sender
            {
                get
                {
                    return _ac.Sender;
                }
            }
        }

        public delegate void AsynchronousCallCompleteHandler(T sender, E e);
        public event AsynchronousCallCompleteHandler Completed;

        internal AsynchronousCall(T sender, UInt64 call)
        {
            IAsynchronousCall = new AsynchronousCallImp(this);
            Sender = sender;
            APICall = call;
        }

        internal SteamAPICall_t APICall
        {
            get;
            private set;
        }

        public T Sender
        {
            get;
            private set;
        }

        internal IAsynchronousCall IAsynchronousCall
        {
            get;
            private set;
        }

        internal void Complete(E e)
        {
            if (Completed != null)
            {
                Completed(Sender, e);
            }
        }
    }
}
