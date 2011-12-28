///////////////////////////////////////////////////////////////////////////////
//
// OnLiveOverlay.h $Revision: 57148 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive Overlay into your
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

#ifndef __ONLIVE_OVERLAY_H__
#define __ONLIVE_OVERLAY_H__

#include "OLAPI.h"
#include "OLUser.h"
#include "OLOverlay.h"

#include "OnLiveContextEventDispatcher.h"

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Overlay                               //
///////////////////////////////////////////////////////////////////////////////

class OnLiveOverlay
{
public:
	static OnLiveOverlay& getInstance()
	{
		static OnLiveOverlay instance;
		return instance;
	}

	OnLiveOverlay();

	void init();
	void deinit();

	// Called to display an alert in the overlay
	olapi::OLStatus DisplayAlert(char * title, char * subtitle, char * message, int duration = 3, olapi::OLOverlayIconTypes leftIconType=olapi::OL_OVERLAYICON_NONE, 
		void * leftPNGFile=NULL, int leftPNGSize=0,  olapi::OLOverlayIconTypes rightIconType=olapi::OL_OVERLAYICON_NONE, void * rightPNGFile=NULL, int rightPNGSize=0);

	olapi::OLStatus DisplayMessageBox(char * title, char * text, char * buttons, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callBack);

	// Used for times when the context is made available by OLIntegration
	olapi::OLStatus DisplayMessageBox(olapi::OLContext context, char * title, char * text, char * buttons, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callBack);


	olapi::OLStatus DisplayKeyboard(const char* title, const char* description, const char* defaultString, olapi::OLVirtualKeyboardControl controlFlag, 
						unsigned int minInputLength, unsigned int maxInputLength, const char *validCharacterString, const char* formatString, 
						void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	// Used for times when the context is made available by OLIntegration
	olapi::OLStatus DisplayKeyboard(olapi::OLContext context, const char* title, const char* description, const char* defaultString, olapi::OLVirtualKeyboardControl controlFlag, 
						unsigned int minInputLength, unsigned int maxInputLength, const char *validCharacterString, const char* formatString, 
						void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus DisableMenu(bool disableMenuAccess);
	olapi::OLStatus SetOverlayVisibility(bool visible, void *pObj, OnLiveContextEventDispatcher::ContextEventCallback callback);

private:
	olapi::OLOverlay * mOverlay;

	class OverlayEventHandler : public olapi::OLOverlayEventHandler
	{
	friend class OnLiveOverlay;

	public:
		OverlayEventHandler();

		olapi::OLStatus Status( olapi::OLOverlayStatus statusBits );
		olapi::OLStatus DataList(olapi::OLContext context, olapi::OLDataList* dataBlock);
		olapi::OLStatus MessageBoxResult( olapi::OLContext context, int button );
		olapi::OLStatus StatusResult(olapi::OLContext, olapi::OLStatus);
	};

	class OverlayEventCallback : public olapi::OLOverlayEventWaitingCallback
	{
	public:
		OverlayEventCallback();

		void EventWaiting();

	private:
		// No assignment operator
		OverlayEventCallback& operator=(OverlayEventCallback const&) {}
	};

	OverlayEventHandler * mOverlayEventHandler;
	OverlayEventCallback* mOverlayEventCallback;
};

#endif // __ONLIVE_OVERLAY_H__
