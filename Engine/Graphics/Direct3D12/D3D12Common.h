#pragma once
#include "CommonHeaders.h"
#include "Graphics\Renderer.h"
#include "Platform/Window.h"

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <dxgi1_6.h>
#include <d3d12.h>
#include <wrl.h>

#pragma comment(lib, "dxgi.lib")
#pragma comment(lib, "d3d12.lib")
//#pragma comment(lib, "dxcompiler.lib")

namespace primal::graphics::d3d12
{
	constexpr u32 frame_buffer_count{3};
	using id3d12_device = ID3D12Device10;
	using id3d12_graphics_command_list = ID3D12GraphicsCommandList7;
}

#ifdef _DEBUG
#define DXCall(x)								\
if ( FAILED(x)){								\
	char line_number[32];						\
sprintf_s(line_number, "%u", __LINE__);			\
OutputDebugStringA("Error in: ");				\
OutputDebugStringA(__FILE__);					\
OutputDebugStringA("\nLine: ");					\
OutputDebugStringA(line_number);				\
OutputDebugStringA("\n");						\
OutputDebugStringA(#x);							\
OutputDebugStringA("\n");						\
__debugbreak();									\
}
#else
#define DXCall(x) x
#endif

#ifdef _DEBUG
#define NAME_D3D12_OBJECT(obj, name) obj -> SetName(name); OutputDebugString(L"D3D12 Object Created: "); OutputDebugString(name); OutputDebugString(L"\n");

#define NAME_D3D12_OBJECT_INDEXED(obj, n, name)						\
{															\
wchar_t full_name[128];										\
if ( swprintf(full_name, L"%s[%u]", name, n) > 0){			\
	obj->SetName(full_name);								\
	OutputDebugString(L"D3D12 Object Created: ");			\
	OutputDebugString(full_name);							\
	OutputDebugString(L"\n");								\
}}											

#else
#define NAME_D3D12_OBJECT(obj, name)
#define NAME_D3D12_OBJECT_INDEXED(obj, n, name)
#endif

#include "D3D12Resources.h"
#include "D3D12Helper.h"