OnLive Service Platform API
Version 1.201
=============================================================================

OnLive Confidential. Released under SDK License Agreement.
Copyright ©2009 - 2010 OnLive, Inc. All rights reserved.

This document contains confidential material proprietary to OnLive, Inc. Your
access to and use of these confidential materials is subject to the terms and
conditions of your confidentiality and SDK license agreements with OnLive. This
document and information and ideas herein may not be disclosed or distributed
to anyone outside your company without the prior written consent of OnLive. 

This material, including but not limited to documentation and related code, is
protected by U.S. and international copyright laws and other intellectual
property laws worldwide including, but not limited to, U.S. and international
patents and patents pending. OnLive, MicroConsole, Brag Clips, Mova, Contour, 
the OnLive logo and the Mova logo are trademarks or registered trademarks of 
OnLive, Inc. The OnLive logo and the Mova logo are copyrights or registered 
copyrights of OnLive, Inc. All other trademarks and other intellectual property
assets are the property of their respective owners and are used by permission. 
The furnishing of these materials does not give you any license to said
intellectual property.

THIS SOFTWARE IS PROVIDED BY ONLIVE "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF 
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

=============================================================================

The OnLive® Software Development Kit (SDK) gives you the tools and examples you
need to optimize communication and performance on the OnLive platform.  
This README file contains complete development environment requirements
and an inventory of the software, documentation, and example applications 
that make up the OnLive SDK. 


DEVELOPMENT REQUIREMENTS

    Use the development tools you prefer, on Windows Vista and XP SP2 
    or later, when adapting your title to OnLive.  There are a few 
    required tools or versions of tools you must use to successfully adapt 
    your title to be OnLive Compliant using the OnLive SDK.

    The OnLive SDK is implemented in C++, and exported as a DLL with 
    Microsoft Visual Studio 2005 and 2008 compatible libraries.    
    Both Windows Vista and XP SP2 or later are supported for development.

    Configure your development PC hardware as you prefer.  To ensure best 
    performance, test your game on a PC built to comply with the 
    OnLive target PC configuration.  OnLive target platform hardware 
    configuration is described in the document, "OnLive Compliance Guidelines".

Building the Tools and Samples

    The tools and samples included with this OnLive SDK require 
    DirectX SDK 9 Aug. 2008 or later.

ONLIVE SDK FILES AND FOLDERS

\docs

    The docs folder contains OnLive Compliance documents, in Adobe Acrobat PDF 
    format, and the OnLive SDK Reference, in Microsoft Help CHM format.  

\assets
    The assets folder contains OnLive controller images in .png format for use
    within your game.

\include\ol_api

Header files to include in your application for the OnLive SDK APIs:

    OLAPI.h
    OLApplication.h 
    OLService.h
    OLUser.h 
    OLOverlay.h
    
The main header file is OLAPI.h, and your application should include it first. 

The deprecated folder contains the deprecated version(s) of header file as a reference only.

\lib\WinXP_32

    Object files for your application to link against.

\tools\SDKtest_harness
\tools\SDKtest_harness\SDKtest_harness.exe

    The OnLive Test Harness lets you simulate the OnLive environment on your 
    own computer.  You can find additional information on the OnLive 
    Test Harness and SDKtest in the OnLive SDK Reference CHM file. 

    Usage: SDKtest_harness [ /script <script_path> | /compliance ]

    /compliance		Launch SDK Test Harness in compliance mode. 
                 	Note: Compliance mode tests your title for 
                 	compliance with the OnLive Compliance Guidelines and 
                 	outputs an OnLive Compliance Report in HTML format.

    /script  <script_path>	Launch SDK Test Harness with scripted commands
                 		to automate testing.
    
    /?			Display help commands.

    Executing SDKtest_harness with no argument or a script will execute 
    the harness in interactive mode.  You can find more information about 
    the SDK Test Harness in the "OnLive Software Development Kit Reference".
    
    Please note that SDKtest harness is designed to run and wait for a game
    to connect to it, or to launch a game.  If you use it with a debugger
    and step into your game, you will want to make sure 'Debugger Mode' is
    on so that the test harness does not delete files installed to your
    games directory.  If you also use your game exe to cook data files then
    you may find that you will need to remove the DirectX shim DLL's from
    your game director before running your game in cook mode.

\Samples

    A collection of source examples demonstrating how to implement 
    important features of the OnLive SDK.

    To build use the associated Microsoft Visual Studio project file for each sample
    or the parent project file containing all samples.

    The sample project files check for an environment variable called 
    LAUNCH_HARNESS.  If that is set to YES then the samples will automatically
    Load SDK Test Harness at the end of the build process.

    With test harness loaded you may debug into the sample directly from the
    debugger.
    
\Samples\SDKTest

    The SDKTest program demonstrates some of the basic interfaces provided
    by the OnLive SDK. Source for SDKTest is included so you can try out
    API features in a simple application before adding them to your title.

\OLIntegration

    OLIntegration may be an easier path to integration with OnLive.  It handles
    most of the session management (login flow) for you.  Most of the samples 
    have been rewritten for OLIntegration. 
    
\OLIntegration\Samples\SDKTest

    The OLIntegration SDKTest program demonstrates some of the basic interfaces 
    provided by the OnLive SDK via OLIntegration. Source for SDKTest is included
    so you can try out API features in a simple application before adding them 
    to your title.  

KNOWN ISSUES

    The OnLive Test Harness on some Windows OS installs may have an error
    where you need to run it one time to register it, and reboot your machine.
    This issue has to do with Administrator access to certain registry keys.
    Test Harness will stop and ask you to reboot if this occurs.

    Building samples with Visual Studio 2008 may cause a runtime error about 
    the VS2008 runtime library not found. This is due to conversion from the VC2005 
    project, you may have to include default libraries into your VS2008 project.
    
    Building samples with Visual Studio 2005 may appear to lock up with a 
    large amount of CPU usage.  This appears to be related to a Microsoft
    known issue, http://support.microsoft.com/kb/930259.  If you do not have
    the recommended hot fix, we have found that if you perform a Clean then 
    a Rebuild the issue may go away.

    When loading submission forms in word, Microsoft word may pop up a dialog 
    stating that "Word cannot start the converter mswrd632.wpc."  This is a
    Microsoft Word Security Update Issue, security bulletin MS09-073.

    On Windows Vista and Windows 7 please disable UAC before trying to run Test Harness.
    
    Windows Vista and Windows 7 may have administrator rights issues 
    depending upon your OS security settings. Please make sure your login account has 
    Administrator rights.  We are trying to narrow down this issue. 
