//========= Copyright � 1996-2008, Valve LLC, All rights reserved. ============
//
// Purpose: Class for rendering photon beams
//
// $NoKeywords: $
//=============================================================================

#include "stdafx.h"
#include "PhotonBeam.h"
#include "SpaceWar.h"

//-----------------------------------------------------------------------------
// Purpose: Constructor
//-----------------------------------------------------------------------------
CPhotonBeam::CPhotonBeam( CGameEngine *pGameEngine, float xPos, float yPos, DWORD dwBeamColor, float flInitialRotation, float flInitialXVelocity, float flInitialYVelocity ) 
	: CSpaceWarEntity( pGameEngine, 3, true )
{
	// Beams only have a lifetime of 1 second
	m_ulTickCountToDieAt = m_pGameEngine->GetGameTickCount()+PHOTON_BEAM_LIFETIME_IN_TICKS;

	// Set a really high max velocity for photon beams
	SetMaximumVelocity( 500 );

	AddLine( -2.0f, -3.0f, -2.0f, 3.0f, dwBeamColor );
	AddLine( 2.0f, -3.0f, 2.0f, 3.0f, dwBeamColor );
	SetPosition( xPos, yPos );
	SetRotationDeltaNextFrame( flInitialRotation );
	SetVelocity( flInitialXVelocity, flInitialYVelocity );
}


//-----------------------------------------------------------------------------
// Purpose: Update with data from server
//-----------------------------------------------------------------------------
void CPhotonBeam::OnReceiveServerUpdate( ServerPhotonBeamUpdateData_t UpdateData )
{
	SetPosition( UpdateData.m_flXPosition, UpdateData.m_flYPosition );
	SetVelocity( UpdateData.m_flXVelocity, UpdateData.m_flYVelocity );
	SetAccumulatedRotation( UpdateData.m_flCurrentRotation );
}