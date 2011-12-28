///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveFunctions.cpp $Revision: 60550 $
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

#include "OnLiveFunctions.h"

///////////////////////////////////////////////////////////////////////////////
//                    OnLive Game Specific Functionality                     //
///////////////////////////////////////////////////////////////////////////////

void onlive_function::exit_game(bool exit_immediately)
{
	UNUSED_PARAM(exit_immediately);

	if (exit_immediately)
	{
		exit(-1);
	}

	// do your exit code
	PostQuitMessage(0);
}

bool onlive_function::is_resolution_supported(int width, int height, int rate, bool windowed, bool vsync)
{
	UNUSED_PARAM(width);
	UNUSED_PARAM(height);
	UNUSED_PARAM(rate);
	UNUSED_PARAM(windowed);
	UNUSED_PARAM(vsync);

	// TODO: Check if resolution is supported
	return false;
}

void onlive_function::change_resolution(int width, int height, int rate, bool windowed, bool vsync)
{
	UNUSED_PARAM(width);
	UNUSED_PARAM(height);
	UNUSED_PARAM(rate);
	UNUSED_PARAM(windowed);
	UNUSED_PARAM(vsync);

	// TODO: Change resolutions.
}

void onlive_function::pause(OnLiveUser::PauseState state)
{
	UNUSED_PARAM(state);

	// TODO: Pause the game
}

#ifdef USE_OVERLAY
olapi::OLStatus onlive_function::overlay_status( olapi::OLOverlayStatus statusBits )
{
	UNUSED_PARAM(statusBits);
	return olapi::OL_NOT_IMPLEMENTED;
}
#endif

olapi::OLStatus onlive_function::log_received(char *buf)
{
	// This allows access to the debugging SDK log receiver

	UNUSED_PARAM(buf);
	return olapi::OL_NOT_IMPLEMENTED;
}

void onlive_function::bind_user()
{
	//TODO: If you need a username or other bind information, implement this.
}

void onlive_function::unbind_user()
{
	//TODO: If you need to unbind, implement this.
}

bool onlive_function::suspend_game()
{
	// TODO: If you support suspend, implement this

	return false;
}

void onlive_function::resume_game()
{
	// TODO: If you support suspend, implement this
}

void onlive_function::wait_aborted(WaitAbortReason reason)
{
	// TODO: If you get an abort while performing an operation, the operation failed.

	UNUSED_PARAM(reason);
}

#ifdef USE_VOICE
olapi::OLStatus onlive_function::voice_chat_user_status( olapi::OLVoiceUserStatusFlags flags )
{
	UNUSED_PARAM(flags);
	return olapi::OL_NOT_IMPLEMENTED;
}
#endif

