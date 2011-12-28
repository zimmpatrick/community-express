///////////////////////////////////////////////////////////////////////////////
//
// OnLiveContent.h $Revision: 0 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive Content into your
// game.  Most documentation to the classes defined herein are documented in the
// cpp file with minimal documentation in the header file.
// 
///////////////////////////////////////////////////////////////////////////////
// 
//           Copyright(c) 2009 - 2011 OnLive, Inc.  All Rights Reserved.
// 
// NOTICE TO USER:
// 
// This code contains confidential material proprietary to OnLive, Inc. Your access
// to and use of these confidential materials is subject to the terms and conditions
// of your confidentiality and SDK license agreements with OnLive. This document and
// information and ideas herein may not be disclosed, copied, reproduced or distributed
// to anyone outside your company without the prior written consent of OnLive.
// 
// You may not modify, reverse engineer, or otherwise attempt to use this code for
// purposes not specified in your SDK license agreement with OnLive. This material,
// including but not limited to documentation and related code, is protected by U.S.
// and international copyright laws and other intellectual property laws worldwide
// including, but not limited to, U.S. and international patents and patents pending.
// OnLive, MicroConsole, Brag Clips, Mova, Contour, the OnLive logo and the Mova logo
// are trademarks or registered trademarks of OnLive, Inc. The OnLive logo and the Mova
// logo are copyrights or registered copyrights of OnLive, Inc. All other trademarks
// and other intellectual property assets are the property of their respective owners
// and are used by permission. The furnishing of these materials does not give you any
// license to said intellectual property.
// 
// THIS SOFTWARE IS PROVIDED BY ONLIVE "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
// USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

#ifndef __ONLIVE_CONTENT_H__
#define __ONLIVE_CONTENT_H__

#include "OLAPI.h"
#include "OLUser.h"
#include "OLContent.h"

#include "OnLiveContextEventDispatcher.h"

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Content                               //
///////////////////////////////////////////////////////////////////////////////

class OnLiveContent
{
public:
	static OnLiveContent& getInstance()
	{
		static OnLiveContent instance;
		return instance;
	}

	OnLiveContent();

	void init();
	void deinit();

	olapi::OLStatus GetUserPurchasedContentAddonIDs( void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetContentAddonInfo( olapi::OLIDList *contentIDs,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

private:
	olapi::OLContent * mContent;

	class ContentEventHandler : public olapi::OLContentEventHandler
	{
	friend class OnLiveContent;

	public:
		ContentEventHandler();

		// Inherited functions
		virtual olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);
		virtual olapi::OLStatus IDListResult(olapi::OLContext context, olapi::OLIDList *idlist);
	};

	class ContentEventCallback : public olapi::OLContentEventWaitingCallback
	{
	public:
		ContentEventCallback();

		void EventWaiting();

	private:
		// No assignment operator
		ContentEventCallback& operator=(ContentEventCallback const&) {}
	};

	ContentEventHandler * mContentEventHandler;
	ContentEventCallback* mContentEventCallback;
};

#endif // __ONLIVE_CONTENT_H__
