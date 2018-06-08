// GameModeHook32.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

bool altPressed;

LRESULT CALLBACK llKeyboardProc32(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode < 0 || nCode != HC_ACTION)
		return CallNextHookEx(NULL, nCode, wParam, lParam);

	KBDLLHOOKSTRUCT* p = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);
	switch (wParam)
	{
	case WM_KEYDOWN:
	case WM_SYSKEYDOWN:
		if (p->vkCode == VK_LMENU || p->vkCode == VK_RMENU)
		{
			altPressed = true;
		}
	case WM_KEYUP:
	case WM_SYSKEYUP:
		if (p->vkCode == VK_LMENU || p->vkCode == VK_RMENU)
		{
			altPressed = false;
		}
	}

	if (p->vkCode == VK_F4)
	{
		bool alt = altPressed || (GetKeyState(VK_MENU) & 0x8000);
		if (alt)
		{
			return 1;
		}
	}
	return CallNextHookEx(NULL, nCode, wParam, lParam);
}