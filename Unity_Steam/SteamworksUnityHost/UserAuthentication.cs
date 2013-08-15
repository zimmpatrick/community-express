using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{    
    using HSteamUser = Int32;
	using AppId_t = UInt32;
	using SteamAPICall_t = UInt64;
    using HAuthTicket = UInt32;

    public class SessionTicket
    {
        private User _user;

        internal Byte[] authTicket = new Byte[UserAuthentication.AuthTicketSizeMax];
	    internal UInt32 authTicketSize;
        internal HAuthTicket handleAuthTicket;
        internal EResult result;

        internal SessionTicket(User user)
        {
            _user = user;
        }

        public Byte[] AutheticationTicket
        {
            get { return authTicket; }
        }

        public EResult Valid
        {
            get { return result; }
        }

        public void Cancel()
        {
            _user.Authentication.CancelAuthSessionTicket(this);
        }
    }

    delegate void OnUserGetEncryptedAppTicketFromSteam();
    public delegate void OnUserEncryptedAppTicketCreated(Byte[] ticket);

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    struct GetAuthSessionTicketResponse_t
    {
        public HAuthTicket m_hAuthTicket;
        public EResult m_eResult;
    }

    delegate void OnAuthSessionTicketResponseReceivedFromSteam(ref GetAuthSessionTicketResponse_t CallbackData);
    public delegate void OnAuthSessionTicketResponseReceived(User user, SessionTicket st);

    public class UserAuthentication
    {
        [DllImport("CommunityExpressSW")]
        private static extern SteamAPICall_t SteamUnityAPI_SteamUser_RequestEncryptedAppTicket(IntPtr user, IntPtr dataToInclude, Int32 dataLength);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamUser_GetEncryptedAppTicket(IntPtr user,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] ticket, int maxTicket, out UInt32 ticketSize);
        [DllImport("CommunityExpressSW")]
        private static extern HAuthTicket SteamUnityAPI_SteamUser_GetAuthSessionTicket(IntPtr user,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] ticket, int maxTicket, out UInt32 ticketSize);
        [DllImport("CommunityExpressSW")]
        private static extern void SteamUnityAPI_SteamUser_CancelAuthTicket(IntPtr user, HAuthTicket hAuthTicket);

        internal const Int32 AuthTicketSizeMax = 2048;
        
        private User _user;
        private OnUserEncryptedAppTicketCreated _onUserEncryptedAppTicketCreated;
        private OnAuthSessionTicketResponseReceivedFromSteam _internalOnAuthSessionTicketResponseReceived = null;
        private OnAuthSessionTicketResponseReceived _onAuthSessionTicketResponseReceived;
        private List<SessionTicket> _sts;

        internal UserAuthentication(User user)
        {
            _user = user;
            _sts = new List<SessionTicket>();
        }


        public void RequestEncryptedAppTicket(Byte[] dataToInclude, OnUserEncryptedAppTicketCreated onUserEncryptedAppTicketCreated)
        {
            _onUserEncryptedAppTicketCreated = onUserEncryptedAppTicketCreated;

            IntPtr dataPtr = Marshal.AllocHGlobal(dataToInclude.Length);
            Marshal.Copy(dataToInclude, 0, dataPtr, dataToInclude.Length);

            SteamAPICall_t callbackId = SteamUnityAPI_SteamUser_RequestEncryptedAppTicket(_user.UserPointer, dataPtr, dataToInclude.Length);

            CommunityExpress.Instance.AddUserGetEncryptedAppTicketCallback(callbackId, OnGetEncryptedAppTicketFromSteam);

            Marshal.FreeHGlobal(dataPtr);
        }

        private void OnGetEncryptedAppTicketFromSteam()
        {
            Byte[] ticket = null, internalTicket = new Byte[AuthTicketSizeMax];
            UInt32 ticketSize;

            if (SteamUnityAPI_SteamUser_GetEncryptedAppTicket(_user.UserPointer, internalTicket, AuthTicketSizeMax, out ticketSize))
            {
                ticket = new Byte[ticketSize];

                for (Int32 i = 0; i < ticketSize; i++)
                {
                    ticket[i] = internalTicket[i];
                }
            }

            _onUserEncryptedAppTicketCreated(ticket);
        }


        private void OnAuthSessionTicketResponseReceivedCallback(ref GetAuthSessionTicketResponse_t CallbackData)
        {
            HAuthTicket authTicket = CallbackData.m_hAuthTicket;
            SessionTicket st = _sts.Find(sst => sst.handleAuthTicket == authTicket);
            if (st != null)
            {
                _sts.Remove(st);

                Byte[] newData = new Byte[st.authTicketSize];
                for (Int32 i = 0; i < st.authTicketSize; i++)
                {
                    newData[i] = st.authTicket[i];
                }
                st.authTicket = newData;
                st.result = CallbackData.m_eResult;
 
                if (_onAuthSessionTicketResponseReceived != null)
                {
                    _onAuthSessionTicketResponseReceived(_user, st);
                }
            }
        }

        // Retrieve ticket to be sent to the entity who wishes to authenticate you ( using BeginAuthSession API ). 
        // pcbTicket retrieves the length of the actual ticket.
        public SessionTicket GetAuthSessionTicket(OnAuthSessionTicketResponseReceived onAuthSessionTicketResponseReceived)
        {
            _onAuthSessionTicketResponseReceived = onAuthSessionTicketResponseReceived;

            SessionTicket st = new SessionTicket(_user);

            if (_internalOnAuthSessionTicketResponseReceived == null)
            {
                _internalOnAuthSessionTicketResponseReceived = new OnAuthSessionTicketResponseReceivedFromSteam(OnAuthSessionTicketResponseReceivedCallback);
            }

            _sts.Add(st);
            st.handleAuthTicket = SteamUnityAPI_SteamUser_GetAuthSessionTicket(_user.UserPointer, st.authTicket, AuthTicketSizeMax, out st.authTicketSize);

            if (st.handleAuthTicket == 0)
            {
                _sts.Remove(st);
                return null;
            }

            return st;
        }

        internal void CancelAuthSessionTicket(SessionTicket st)
        {
            SteamUnityAPI_SteamUser_CancelAuthTicket(_user.UserPointer, st.handleAuthTicket);

            _sts.Remove(st);
        }
    }
}
