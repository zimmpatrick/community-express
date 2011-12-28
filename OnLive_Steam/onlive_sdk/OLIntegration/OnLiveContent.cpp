///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveContent.cpp $Revision: 0 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive Content SDK into
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

#include "OnLiveContent.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"

using namespace olapi;

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveContent                                //
///////////////////////////////////////////////////////////////////////////////

OnLiveContent::OnLiveContent()
	: mContentEventHandler(NULL)
	, mContentEventCallback(NULL)
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveContent::init()
{
	OLStatus status = olapi::OLGetContent(OLAPI_VERSION, &mContent);

	if (status == OL_SUCCESS)
	{
		mContentEventHandler = new ContentEventHandler();
		mContent->SetEventHandler(mContentEventHandler);

		mContentEventCallback = new ContentEventCallback();
		mContent->SetEventWaitingCallback(mContentEventCallback);

		LOG( "OnLiveContent::initialized.");

		return;
	}

	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveContent::deinit()
{
	if (mContent)
	{
		olapi::OLStopContent(mContent);
		LOG( "OnLiveContent::API stopped.");
	}

	if (mContentEventHandler)
	{
		delete mContentEventHandler;
	}

	if (mContentEventCallback)
	{
		delete mContentEventCallback;
	}

	mContentEventHandler = NULL;
	mContentEventCallback = NULL;

	LOG( "OnLiveContent::deinit.");
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveContent::GetUserPurchasedContentAddonIDs( void *pClassPtr,
															   OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mContent) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();

	//NOTE: GetUserPurchasedContentAddonIDs() returns its result immediately, so the dispatcher must be registered before it is called
	OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return mContent->GetUserPurchasedContentAddonIDs(context);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveContent::GetContentAddonInfo( OLIDList *contentIDs,
												   void *pClassPtr,
												   OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mContent) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mContent->GetContentAddonInfo(context, contentIDs);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}


///////////////////////////////////////////////////////////////////////////////

OnLiveContent::ContentEventHandler::ContentEventHandler() 
{
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveContent::ContentEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveContent::ContentEventHandler::IDListResult(olapi::OLContext context, olapi::OLIDList *idlist)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchIDListResult(context, idlist, NULL))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OnLiveContent::ContentEventCallback::ContentEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveContent::ContentEventCallback::EventWaiting()
{
	OnLiveContent::getInstance().mContent->HandleEvent(0, 0);
}

