///////////////////////////////////////////////////////////////////////////////
//
// OnLiveFunctions.h $Revision: 60550 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// These function definitions are scattered throughout the codebase, as they
// often need access to local variables to do their job.  They provide a means
// for the integration class to change the state of the game.  Additional
// functions may be required to support additional functionality.
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

#ifndef __ONLIVE_FUNCTIONS_H__
#define __ONLIVE_FUNCTIONS_H__

#include "OnLiveIntegration.h"

namespace onlive_function
{
	enum WaitAbortReason
	{
		WAIT_ABORT_WM_QUIT,
		WAIT_ABORT_TIMEOUT,
		WAIT_ABORT_EXIT_RECEIVED,
	};

	// This API function is used when OnLive is requesting that the game
	// switch to a new resolution.  This function returns true if the
	// requested resolution is valid.  This functions is called from
	// a separate thread so make sure it's thread safe (d3d device calls may not be).
	bool is_resolution_supported(int width, int height, int rate, bool windowed = false, bool vsync = true);

	// This API function is used when OnLive is requesting that the game
	// switch to a new resolution.  Resolution changes are possible after
	// the start() function and after the bind() function.
	void change_resolution(int width, int height, int rate, bool windowed = false, bool vsync = true);

	// This API function is used when OnLive is asking the game to exit
	void exit_game(bool exit_immediately = false);

	// This API function is used when OnLive is asking the game to pause, or resume
	void pause(OnLiveUser::PauseState state);

	// Everything below this point is optional depending upon what you
	// choose to support.

#ifdef USE_OVERLAY
	// This API function allows for getting the status of the overlay status messages
	olapi::OLStatus overlay_status( olapi::OLOverlayStatus statusBits );
#endif

#ifdef USE_VOICE
	// This API function allows for getting the user's voice chat status.
	olapi::OLStatus voice_chat_user_status( olapi::OLVoiceUserStatusFlags flags );
#endif

	// This API function allows debugging access to OnLive's logging system
	olapi::OLStatus log_received(char *buf);

	// These API functions are to let you know when users are being bound and
	// unbound.
	void bind_user();
	void unbind_user();

	// These API functions are for suspending your session
	bool suspend_game();
	void resume_game();

	// This API function is called if an OLIntegration function gets aborted while
	// waiting for a state change.
	//
	// TODO: If you get an abort while performing an operation, the operation failed.
	void wait_aborted(WaitAbortReason reason);
}


#endif // __ONLIVE_FUNCTIONS_H__
