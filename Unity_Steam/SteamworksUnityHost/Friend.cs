// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class Friend
	{
		private Friends _friends;
		private SteamID _id;

		internal Friend(Friends friends, SteamID id)
		{
			_friends = friends;
			_id = id;
		}

		public string PersonaName
		{
			get { return _friends.GetFriendPersonaName(_id); }
		}

		public EPersonaState PersonaState
		{
			get { return _friends.GetFriendPersonaState(_id); }
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

        public Image SmallAvatar
        {
            get { return _friends.GetSmallFriendAvatar(_id); }
        }

        public Image MediumAvatar
        {
            get { return _friends.GetMediumFriendAvatar(_id); }
        }
	}
}
