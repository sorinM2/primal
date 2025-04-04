#pragma once
#include "ToolsCommon.h"

namespace primal::tools
{

	struct  geometry_impoert_settings
	{
		f32 smoothing_angle;
		u8 calculate_noramals;
		u8 calculate_tangents;
		u8 reverse_handedness;
		u8 import_embeded_textures;
		u8 import_animations;
	};

	struct scene_data
	{
		u8* buffer;
		u32 buffer_size;
		geometry_impoert_settings settings;
	};
}
