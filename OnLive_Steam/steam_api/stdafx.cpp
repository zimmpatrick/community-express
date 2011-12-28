// stdafx.cpp : source file that includes just the standard includes
// steam_api.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

// TODO: reference any additional headers you need in STDAFX.H
// and not in this file

#ifdef _DEBUG
bool _trace(TCHAR *format, ...)
{
   TCHAR buffer[1000];

   va_list argptr;
   va_start(argptr, format);
   int end = wvsprintf(buffer, format, argptr);
   va_end(argptr);

   buffer[end] = '\n';
   buffer[end + 1] = '\0';

   OutputDebugString(buffer);

   return true;
}
#endif
