shader_type canvas_item;

uniform float pixelFactor: hint_range(0.0, 100, 0.5) = 1;

void fragment() {
    vec2 pixelatedUV = round(UV * pixelFactor) / pixelFactor;
    COLOR = texture(TEXTURE, pixelatedUV);
}