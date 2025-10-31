shader_type canvas_item;

// Settings
uniform vec4 color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 background : source_color = vec4(0.0, 0.0, 0.0, 0.7);
uniform sampler2D gradient;
uniform bool use_gradient = false;
uniform float min_value = 0.0;
uniform float max_value = 100.0;
uniform float current_value : hint_range(0.0, 100.0, 1)= 50.0;
uniform float start_angle : hint_range(-360.0, 360.0) = -90.0;
uniform bool clockwise = true;
uniform float outer_radius : hint_range(0.0, 0.5) = 0.5;
uniform float width : hint_range(0.0, 0.5) = 0.1;
uniform float antialiasing : hint_range(0.0, 0.5) = 0.01;

void fragment() {
    vec2 uv = UV - vec2(0.5);
    float radius = length(uv);
    
    float angle = atan(uv.y, uv.x);
    
    float start_rad = (start_angle / 180.0) * PI;
    
    float progress = (current_value - min_value) / (max_value - min_value);
    
    float angle_progress = angle;
    if (clockwise) {
        angle_progress = (angle - start_rad);
        if (angle_progress < 0.0) {
            angle_progress += 2.0 * PI;
        }
    } else {
        angle_progress = (start_rad - angle);
        if (angle_progress < 0.0) {
            angle_progress += 2.0 * PI;
        }
    }
    
    float percent = angle_progress / (2.0 * PI);
    
    float ring = smoothstep(outer_radius - width - antialiasing, outer_radius - width, radius) - smoothstep(outer_radius - antialiasing, outer_radius, radius);
    
    vec4 final_color = background;
    
    if (percent < progress) {
        vec4 progress_fill_color = color;
        if (use_gradient) {
            progress_fill_color = texture(gradient, vec2(percent / progress, 0.5));
        }
        final_color = progress_fill_color;
    }
    
    COLOR = mix(background, final_color, ring);
    COLOR.a *= ring;
}
