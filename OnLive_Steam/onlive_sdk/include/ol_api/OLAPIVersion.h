/// This file is autogenerated, do not modify.
/// OnLive SDK Version 1.201

#ifndef __OLAPI_VERSION_H__
#define __OLAPI_VERSION_H__

/*! \brief OnLive SDK Version Number String.  See \ref olapi::OLGetService. 

   The version number is composed of the following version parts: "API.INCREMENTAL.MESSAGE.MIN_MESSAGE".\n
       The API version represents the public api interface between application headers and the OnLive DLLs.\n
       The INCREMENTAL version is used for bugfixes that don't bump the other version numbers.
       The MESSAGE version marks the communication between the DLLs and the OSP.\n
       The MIN_MESSAGE is the mininum compatible message version.\n

   From the sdk user's perspective, future onlive DLLs are backwards-compatible with the version you 
   compiled your title against as long as the API portion of the version matches.  NOTE: we do not support
   running a title against an earlier version of the api under any circumstances...

   For example, a title compiled against 1.047.4.4 DLLs can run with version 1.063.4.4 DLLs, since the API versions
    match, but compatibility is not guranteed for any DLLs earlier than 1.047.4.4...
*/
#define OLAPI_VERSION "1.201.32.31"

/*! OnLive SDK Build Number */
#define OLAPI_VERSION_SVN "91505"

/*! OnLive SDK Version Numbers for DLL Resource info */
#define OLAPI_VERSION_NUM 1,201,32,31			// API Win32 Version Number.
#define OLAPI_VERSION_PRD 0,0,9,1505		// API Win32 Build Number.

#endif // __OLAPI_VERSION_H__

