shader_type canvas_item;

uniform float outline_width : hint_range(0, 100, 1) = 1.0;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform bool outline_enabled = true;

void fragment() {
    vec4 current_color = texture(TEXTURE, UV);
    vec2 size = TEXTURE_PIXEL_SIZE * outline_width;

    if (outline_enabled && current_color.a <= 0.1)
	{
        // 检查八个方向的像素
        float alpha = 0.0;
        alpha += texture(TEXTURE, UV + vec2(0.0, -size.y)).a;        // 上
        alpha += texture(TEXTURE, UV + vec2(size.x, -size.y)).a;     // 右上
        alpha += texture(TEXTURE, UV + vec2(size.x, 0.0)).a;         // 右
        alpha += texture(TEXTURE, UV + vec2(size.x, size.y)).a;      // 右下
        alpha += texture(TEXTURE, UV + vec2(0.0, size.y)).a;         // 下
        alpha += texture(TEXTURE, UV + vec2(-size.x, size.y)).a;     // 左下
        alpha += texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;        // 左
        alpha += texture(TEXTURE, UV + vec2(-size.x, -size.y)).a;    // 左上

        if (alpha > 0.0)
            COLOR = vec4(outline_color.rgb, alpha * outline_color.a);
        else
            COLOR = current_color;
    }
	else
	{
        COLOR = current_color;
    }
}