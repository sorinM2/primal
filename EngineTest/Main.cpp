#pragma comment(lib, "engine.lib")
#include "Windows.h"

#define TEST_ENTITY_COMPONENTS 0
#define TEST_WINDOW 1

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponents.h"
#elif TEST_WINDOW
#include "WindowTest.h"
#endif


#ifdef _WIN64
int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	engine_test test{};
	if (test.initialize())
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
			test.run();
		}
	}
	test.shutdown();
	return 0;
}
#else
int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
    engine_test test{};

    if (test.initialize())
    {
        test.run();
    }

    test.shutdown();
}
#endif