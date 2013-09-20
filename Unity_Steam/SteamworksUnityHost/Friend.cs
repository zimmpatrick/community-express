// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about a friend
    /// </summary>
	public class Friend
	{
		private Friends _friends;
		private SteamID _id;

		internal Friend(Friends friends, SteamID id)
		{
			_friends = friends;
			_id = id;
		}

        /// <summary>
        /// Gets large-size avatar for user
        /// </summary>
        /// <returns>large avatar</returns>
		public Image GetLargeAvatar()
		{
			return _friends.GetLargeFriendAvatar(_id);
		}

        /// <summary>
        /// Name of the friend's account
        /// </summary>
		public string PersonaName
		{
			get { return _friends.GetFriendPersonaName(_id); }
		}
        /// <summary>
        /// What state the friend is in
        /// </summary>
		public EPersonaState PersonaState
		{
			get { return _friends.GetFriendPersonaState(_id); }
		}
        /// <summary>
        /// Friend's Steam ID
        /// </summary>
		public SteamID SteamID
		{
			get { return _id; }
		}
        /// <summary>
        /// Small version of the friend's avatar
        /// </summary>
		public Image SmallAvatar
		{
			get { return _friends.GetSmallFriendAvatar(_id); }
		}
        /// <summary>
        /// Medium version of the friend's avatar
        /// </summary>
		public Image MediumAvatar
		{
			get { return _friends.GetMediumFriendAvatar(_id); }
		}
        /// <summary>
        /// Large version of the friend's avatar
        /// </summary>
		public Image LargeAvatar
		{
			get { return _friends.GetLargeFriendAvatar(_id); }
		}

        public FriendGameInfo GamePlayed
        {
            get { return _friends.GetFriendGamePlayed(_id); }
        }
	}
}
