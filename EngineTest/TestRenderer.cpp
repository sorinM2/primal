#include "TestRenderer.h"
#include "..\Platform\Platform.h"
#include "..\Platform\PlatformTypes.h"
#include "..\Graphics\Renderer.h"
#include "ShaderCompilation.h"
#if TEST_RENDERER
using namespace primal;

graphics::render_surface _surfaces[4];
time_it timer{};

bool resized{ false };
bool is_restarting{ false };
void destroy_render_surface(graphics::render_surface& surface);
bool test_initialize();
void test_shutdown();

LRESULT win_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	bool toggle_fullscreen{ false };
	switch (msg)
	{
	case WM_DESTROY:
	{
		bool all_closed{ true };
		for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
		{
			if (_surfaces[i].window.is_valid())
			{
				if (_surfaces[i].window.is_closed())
				{
					destroy_render_surface(_surfaces[i]);
				}
				else
				{
					all_closed = false;
				}
			}
			if (all_closed && !is_restarting)
			{
				PostQuitMessage(0);
				return 0;
			}
		}
		break;
	}
	case WM_SIZE:
		if (wparam != SIZE_MINIMIZED)
			resized = true;
		break;
	case WM_SYSCHAR:
		toggle_fullscreen = (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN));
		break;

	case WM_KEYDOWN:
		if (wparam == VK_ESCAPE)
		{
			PostMessage(hwnd, WM_CLOSE, 0, 0);
			return 0;
		}
		else if (wparam == VK_F11)
		{
			is_restarting = true;
			test_shutdown();
			test_initialize();
			is_restarting = false;
		}
	}

	if ((resized && GetAsyncKeyState(VK_LBUTTON) >= 0) || toggle_fullscreen)
	{
		platform::window win{ platform::window_id{(id::id_type)GetWindowLongPtr(hwnd, GWLP_USERDATA)} };
		for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
		{
			if (win.get_id() == _surfaces[i].window.get_id())
			{
				if (toggle_fullscreen)
				{
					win.set_fullscreen(!win.is_fullscreen());

					return 0;
				}
				else
				{
					_surfaces[i].surface.resize(win.width(), win.height());
					resized = false;
				}
				break;
			}
		}
	}
	return DefWindowProc(hwnd, msg, wparam, lparam);
}

void create_render_surface(graphics::render_surface& surface, platform::window_init_info info)
{
	surface.window = platform::create_window(&info);
	surface.surface = graphics::create_surface(surface.window);
}

void destroy_render_surface(graphics::render_surface& surface)
{
	graphics::render_surface temp{surface};
	surface = {};
	if ( temp.surface.is_valid()) graphics::remove_surface(temp.surface.get_id());
	if ( temp.window.is_valid()) platform::remove_window(temp.window.get_id());
}


bool test_initialize()
{
	while (!compile_shaders())
	{
		if (MessageBox(nullptr, L"Failed to compile engine shaders.", L"Shader compilation Error", MB_RETRYCANCEL) != IDRETRY)
			return false;
	}

	if (!graphics::initialize(graphics::graphics_platform::direct3d12)) return false;
	platform::window_init_info info[]
	{
		{&win_proc, nullptr, L"Test window 1", 100, 100, 400, 800},
		{&win_proc, nullptr, L"Test window 2", 150, 150, 800, 400},
		{&win_proc, nullptr, L"Test window 3", 200, 200, 400, 400},
		{&win_proc, nullptr, L"Test window 4", 250, 250, 800, 600},
	};
	static_assert(_countof(info) == _countof(_surfaces));
	for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
		create_render_surface(_surfaces[i], info[i]);
	return true;
}

bool engine_test::initialize() 
{
	return test_initialize();
}


void engine_test::run() 
{
	timer.begin();
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
	for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
	{
		if (_surfaces[i].surface.is_valid())
		{
			_surfaces[i].surface.render();
		}
	}
	timer.end();
}

void test_shutdown()
{
	for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
	{
		destroy_render_surface(_surfaces[i]);
	}

	graphics::shutdown();
}

void engine_test::shutdown() 
{
	return test_shutdown();
}
#endif