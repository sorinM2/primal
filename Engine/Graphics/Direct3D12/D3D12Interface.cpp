#include "CommonHeaders.h"
#include "D3D12Interface.h"
#include "Graphics/Renderer.h"
#include "Graphics/GraphicsPlatformInterface.h"
#include "D3D12Core.h"

namespace primal::graphics::d3d12
{
	void get_platform_interface(platform_interface& pi)
	{
		pi.initialize = core::initialize;
		pi.shutdown = core::shutdown;
	}
}