/*
//
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//        Copyright (c) 2000-2005 Intel Corporation. All Rights Reserved.
//
*/

// IppiImageDC.cpp: implementation of the CUImageDC class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "uImageDC.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CUImageDC::CUImageDC()
{
    m_pBits = NULL;
    m_hBmp  = NULL;
	m_hDC	= NULL;
}

CUImageDC::~CUImageDC()
{
   Delete();
}

void CUImageDC::Create(int nWidth, int nHeight)
{
	BITMAPINFOHEADER	bih;
	memset(&bih, 0, sizeof(BITMAPINFOHEADER));
	bih.biSize		=	sizeof(BITMAPINFOHEADER);
	bih.biWidth		=	((((int)nWidth * 8) + 31) & ~31) >> 3;
	bih.biHeight	=	nHeight;
	bih.biPlanes	=	1;
	bih.biBitCount	=	32;
	bih.biCompression	=	BI_RGB;
	m_hBmp	=	NULL;
	m_hDC	=	CreateCompatibleDC(NULL);
	m_hBmp	= ::CreateDIBSection(
		GetSafeHdc(), (BITMAPINFO*)&bih,
		DIB_RGB_COLORS, (void**)(&m_pBits), NULL, 0);
	SelectObject(m_hDC, m_hBmp);
}

void CUImageDC::Delete()
{
	if (m_hBmp)
		DeleteObject(m_hBmp);
	if (m_hDC)
		DeleteDC(m_hDC);
}
