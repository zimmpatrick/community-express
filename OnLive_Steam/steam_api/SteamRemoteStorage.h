
#pragma once

#include "stdafx.h"

namespace SteamAPI
{

class SteamRemoteStorage : public ISteamRemoteStorage
{
public:
			// file operations
		virtual bool	FileWrite( const char *pchFile, const void *pvData, int32 cubData )
		{
			TRACE( TEXT("FileWrite('%s',%x,%d)"), pchFile, pvData, cubData );
			return false;
		}

		virtual int32	FileRead( const char *pchFile, void *pvData, int32 cubDataToRead )
		{
			TRACE( TEXT("FileRead('%s',%x,%d)"), pchFile, pvData, cubDataToRead );
			return 0;
		}

		virtual bool	FileForget( const char *pchFile )
		{
			return true;
		}

		virtual bool	FileDelete( const char *pchFile )
		{
			return true;
		}
		
		virtual SteamAPICall_t FileShare( const char *pchFile )
		{
			return true;
		}

		virtual bool	SetSyncPlatforms( const char *pchFile, ERemoteStoragePlatform eRemoteStoragePlatform )
		{
			return true;
		}

		// file information
		virtual bool	FileExists( const char *pchFile )
		{
			TRACE( TEXT("FileExists('%s')"), pchFile );
			return false;
		}

		virtual bool	FilePersisted( const char *pchFile )
		{
			return false;
		}

		virtual int32	GetFileSize( const char *pchFile )
		{
			TRACE( TEXT("GetFileSize('%s')"), pchFile );
			return 0;
		}

		virtual int64	GetFileTimestamp( const char *pchFile )
		{
			return 0;
		}

		virtual ERemoteStoragePlatform GetSyncPlatforms( const char *pchFile )
		{
			return k_ERemoteStoragePlatformNone;
		}

		// iteration
		virtual int32 GetFileCount()
		{
			TRACE( TEXT("GetFileCount()") );
			return 0;
		}

		virtual const char *GetFileNameAndSize( int iFile, int32 *pnFileSizeInBytes )
		{
			TRACE( TEXT("GetFileNameAndSize()") );
			return 0;
		}

		// configuration management
		virtual bool GetQuota( int32 *pnTotalBytes, int32 *puAvailableBytes )
		{
			TRACE( TEXT("GetFileNameAndSize(%x,%x)"), pnTotalBytes, puAvailableBytes );
			*pnTotalBytes = *puAvailableBytes = 0;
			return false;
		}

		virtual bool IsCloudEnabledForAccount()
		{
			return true;
		}
		
		virtual bool IsCloudEnabledForApp()
		{
			return true;
		}
		
		virtual void SetCloudEnabledForApp( bool bEnabled )
		{
		}

		// user generated content
		virtual SteamAPICall_t UGCDownload( UGCHandle_t hContent )
		{
			return 0;
		}

		virtual bool	GetUGCDetails( UGCHandle_t hContent, AppId_t *pnAppID, char **ppchName, int32 *pnFileSizeInBytes, CSteamID *pSteamIDOwner )
		{
			return 0;
		}

		virtual int32	UGCRead( UGCHandle_t hContent, void *pvData, int32 cubDataToRead )
		{
			return 0;
		}

		// user generated content iteration
		virtual int32	GetCachedUGCCount()
		{
			return 0;
		}

		virtual	UGCHandle_t GetCachedUGCHandle( int32 iCachedContent )
		{
			return 0;
		}
};

}