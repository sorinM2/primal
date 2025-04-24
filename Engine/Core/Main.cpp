#include <filesystem>
#include "CommonHeaders.h"
#ifdef _WIN64

#ifndef WIN32_LEAN_AND_MEAN
#define WIND32_LEAN_AND_MEAN
#endif
#include <Windows.h>
#include <crtdbg.h>

namespace
{
	std::filesystem::path set_current_directory_to_executable_path()
	{
		wchar_t path[MAX_PATH];
		const uint32_t length{ GetModuleFileName(0, &path[0], MAX_PATH) };
		if (!length || GetLastError() == ERROR_INSUFFICIENT_BUFFER) return {};
		std::filesystem::path p{ path };
		std::filesystem::current_path(p.parent_path());
		return std::filesystem::current_path();
	}
}
#ifndef USE_WITH_EDITOR

extern bool engine_intialize();
extern void engine_update();
extern void engine_shutdown();

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	set_current_directory_to_executable_path();
	if (engine_intialize())
	{
		MSG msg{};
		bool is_running{ true };
		while (is_running)
		{
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
				is_running = (msg.message != WM_QUIT);
			}
			engine_update();
		}
	}
	engine_shutdown();
	return 0;
}
#endif

#endif