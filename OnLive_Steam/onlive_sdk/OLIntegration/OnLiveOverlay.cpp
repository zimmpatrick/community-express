///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveOverlay.cpp $Revision: 57148 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive Overlay SDK into
// your game.
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

#include "OnLiveOverlay.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"

// Define this to simulate status result error returns from the OSP
//#define SIMULATE_STATUS_RESULT_ERRORS

using namespace olapi;

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveOverlay                                //
///////////////////////////////////////////////////////////////////////////////

OnLiveOverlay::OnLiveOverlay()
	: mOverlayEventHandler(NULL)
	, mOverlayEventCallback(NULL)
{
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveOverlay::init()
{
	OLStatus status = olapi::OLGetOverlay(OLAPI_VERSION, &mOverlay);

	if (status == OL_SUCCESS)
	{
		mOverlayEventHandler = new OverlayEventHandler();
		mOverlay->SetEventHandler(mOverlayEventHandler);

		mOverlayEventCallback = new OverlayEventCallback();
		mOverlay->SetEventWaitingCallback(mOverlayEventCallback);

		LOG( "OnLiveOverlay::initialized.");

		return;
	}

	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::DisplayAlert(char * title, char * subtitle, char * message, int duration, olapi::OLOverlayIconTypes leftIconType, 
		void * leftPNGFile, int leftPNGSize,  olapi::OLOverlayIconTypes rightIconType, void * rightPNGFile, int rightPNGSize)
{ 
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	return mOverlay->DisplayAlert(title, subtitle, message, duration, leftIconType, leftPNGFile, leftPNGSize, rightIconType, rightPNGFile, rightPNGSize); 
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::DisplayMessageBox(char * title, char * text, char * buttons, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callBack)
{ 
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mOverlay->DisplayMessageBox(context, title, text, buttons);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callBack);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::DisplayKeyboard(const char* title, const char* description, const char* defaultString, olapi::OLVirtualKeyboardControl controlFlag, 
						unsigned int minInputLength, unsigned int maxInputLength, const char *validCharacterString, const char* formatString, 
						void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status =  mOverlay->GetInputFromKeyboard(context, title, description, defaultString, controlFlag, minInputLength, maxInputLength, validCharacterString, formatString);
	
	if (status == OL_SUCCESS)
	{
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);
	}
	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::DisplayMessageBox(olapi::OLContext context, char * title, char * text, char * buttons, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callBack)
{
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	OLStatus status = mOverlay->DisplayMessageBox(context, title, text, buttons);

	if (status == OL_SUCCESS)
	OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callBack);

	return status;
}

///////////////////////////////////////////////////////////////////////////////
olapi::OLStatus OnLiveOverlay::DisplayKeyboard(olapi::OLContext context, const char* title, const char* description, const char* defaultString, olapi::OLVirtualKeyboardControl controlFlag, 
						unsigned int minInputLength, unsigned int maxInputLength, const char *validCharacterString, const char* formatString, 
						void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	OLStatus status = mOverlay->GetInputFromKeyboard(context, title, description, defaultString, controlFlag, minInputLength, maxInputLength, validCharacterString, formatString);
	
	if (status == OL_SUCCESS)
	{
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);
	}
	return status;
}

///////////////////////////////////////////////////////////////////////////////
olapi::OLStatus OnLiveOverlay::DisableMenu(bool disableMenuAccess)
{
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	return mOverlay->DisableMenu( disableMenuAccess );
}

///////////////////////////////////////////////////////////////////////////////
olapi::OLStatus OnLiveOverlay::SetOverlayVisibility(bool visible, void *pObj, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mOverlay) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = OLGetNextContext();
	OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pObj, callback);

	OLStatus status = mOverlay->DisplayMenu(context, visible ? olapi::OL_OVERLAYMENU_DEFAULT : olapi::OL_OVERLAYMENU_NONE);
	
	return status;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveOverlay::deinit()
{
	if (mOverlay)
	{
		olapi::OLStopOverlay(mOverlay);
		LOG( "OnLiveOverlay::API stopped.");
	}

	if (mOverlayEventHandler)
	{
		delete mOverlayEventHandler;
	}

	if (mOverlayEventCallback)
	{
		delete mOverlayEventCallback;
	}

	mOverlay = NULL;
	mOverlayEventHandler = NULL;
	mOverlayEventCallback = NULL;

	LOG( "OnLiveOverlay::deinit.");
}

///////////////////////////////////////////////////////////////////////////////

OnLiveOverlay::OverlayEventHandler::OverlayEventHandler() 
{
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::OverlayEventHandler::Status(olapi::OLOverlayStatus statusBits)
{
	return onlive_function::overlay_status(statusBits);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::OverlayEventHandler::DataList(olapi::OLContext context, olapi::OLDataList* dataList)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchDataListResult(context, dataList))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::OverlayEventHandler::MessageBoxResult( olapi::OLContext context, int button )
{
#ifdef SIMULATE_STATUS_RESULT_ERRORS
	// 10% of the time, do an error instead
	if( rand() % 10 < 1 )
	{
		LOG( "MessageBoxResult - Simulating a StatusResult() error return" );

		int errorToReturn = rand() % 4;
		switch(errorToReturn)
		{
		case 0:
			{
				return StatusResult( context, olapi::OL_OVERLAY_IN_USE );
			}
			break;
		case 1:
			{
				return StatusResult( context, OL_FAIL );
			}
			break;
		case 2:
			{
				return StatusResult( context, OL_INTERNAL_IO_ERROR );
			}
			break;
		case 3:
			{
				return StatusResult( context, OL_SESSION_NOT_STARTED );
			}
			break;
		}
	}
#endif

	if (OnLiveContextEventDispatcher::getInstance().DispatchMessageBoxResult(context, button))
		return olapi::OL_SUCCESS;
	else
		return  olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveOverlay::OverlayEventHandler::StatusResult( olapi::OLContext context, olapi::OLStatus status )
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OnLiveOverlay::OverlayEventCallback::OverlayEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveOverlay::OverlayEventCallback::EventWaiting()
{
	OnLiveOverlay::getInstance().mOverlay->HandleEvent(0, 0);
}

