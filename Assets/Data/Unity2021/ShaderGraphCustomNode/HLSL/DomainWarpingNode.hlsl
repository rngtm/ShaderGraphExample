#ifndef DOMAIN_WARPING_NODE_HLSL_INCLUDED
#define DOMAIN_WARPING_NODE_HLSL_INCLUDED

// http://www.iquilezles.org/www/articles/warp/warp.htm"
float random(float2 st) {
    return frac(sin(dot(st.xy,
                        float2(12.9898,78.233)))*
                43758.5453123);
}
// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float noise (float2 st) {
    float2 i = floor(st);
    float2 f = frac(st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + float2(1.0, 0.0));
    float c = random(i + float2(0.0, 1.0));
    float d = random(i + float2(1.0, 1.0));

    float2 u = f * f * (3.0 - 2.0 * f);

    return lerp(a, b, u.x) +
            (c - a)* u.y * (1.0 - u.x) +
            (d - b) * u.x * u.y;
}

#define OCTAVES 6
// based on : https://thebookofshaders.com/13/?lan=jp
float fbm (float2 st) {
    // Initial values
    float value = 0.0;
    float amplitude = .5;
    float frequency = 0.;
    // Loop of octaves
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitude * noise(st);
        st *= 2.;
        amplitude *= .5;
    }
    return value;
}

float pattern_1 (float2 p) {
    return fbm(p);
}

float pattern_2 (float2 p) {
    float2 q = float2( 
                    fbm( p + float2(0.0,0.0) ),
                    fbm( p + float2(5.2,1.3) ) 
                    );

    return fbm( p + 4.0*q );
}

float pattern_3 (float2 p) {
    // first domain warping
    float2 q = float2( 
                    fbm( p + float2(0.0,0.0) ),
                    fbm( p + float2(5.2,1.3) ) 
                    );
                            
    // second domain warping
    float2 r = float2( 
                    fbm( p + 4.0*q + float2(1.7,9.2) ),
                    fbm( p + 4.0*q + float2(8.3,2.8) ) 
                    );

    return fbm( p + 4.0*r );
}

float pattern (float2 p, float4 scale_1, float scale_2, float4 add_1, float4 add_2) {
    // first domain warping
    float2 q = float2( 
                    fbm( p + scale_1.x * add_1.xy ),
                    fbm( p + scale_1.y * add_1.zw ) 
                    );
                            
    // second domain warping
    float2 r = float2( 
                    fbm( p + scale_1.z * q + add_2.xy ),
                    fbm( p + scale_1.w * q + add_2.zw ) 
                    );

    return fbm( p + scale_2 * r );
}

void DomainWarpingNode_half(
    float2 UV,
    half4 Fbm_AddFactor_1,
    half4 Fbm_AddFactor_2,
    half4 Fbm_ScaleFactor_1,
    half4 Fbm_ScaleFactor_2,
    out half Pattern)
{
    Pattern = pattern(UV, Fbm_ScaleFactor_1, Fbm_ScaleFactor_2, Fbm_AddFactor_1, Fbm_AddFactor_2);
}

void DomainWarpingNode_float(
    float2 UV,
    float4 Fbm_AddFactor_1,
    float4 Fbm_AddFactor_2,
    float4 Fbm_ScaleFactor_1,
    float4 Fbm_ScaleFactor_2,
    out float Pattern)
{
    Pattern = pattern(UV, Fbm_ScaleFactor_1, Fbm_ScaleFactor_2, Fbm_AddFactor_1, Fbm_AddFactor_2);
}
#endif