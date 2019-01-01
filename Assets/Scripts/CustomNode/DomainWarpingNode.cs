using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

/// <summary>
/// ドメインワーピング(http://www.iquilezles.org/www/articles/warp/warp.htm)
/// </summary>
[Title("Custom", "Domain Warping(@iquilezles)")]
public class DomainWarpingNode : CodeFunctionNode 
{
    public DomainWarpingNode()
    {
        name = "Domain Warping(@iquilezles)";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("DomainWarpingNode_Function",
            BindingFlags.Static | BindingFlags.NonPublic);
    }

    public override string documentationURL
        => "http://www.iquilezles.org/www/articles/warp/warp.htm";

    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        registry.ProvideFunction("random", s => s.Append(@"
            float random(float2 st) {
                return frac(sin(dot(st.xy,
                                    float2(12.9898,78.233)))*
                            43758.5453123);
            }
        "));
        registry.ProvideFunction("noise", s => s.Append(@"
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
        "));
        registry.ProvideFunction("fbm", s => s.Append(@"
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
        "));

        registry.ProvideFunction("pattern_1", s => s.Append(@"
            float pattern_1 (float2 p) {
                return fbm(p);
            }
        "));

        registry.ProvideFunction("pattern_2", s => s.Append(@"
            float pattern_2 (float2 p) {
                float2 q = float2( 
                                fbm( p + float2(0.0,0.0) ),
                                fbm( p + float2(5.2,1.3) ) 
                                );

                return fbm( p + 4.0*q );
            }
        "));
        
        registry.ProvideFunction("pattern_3", s => s.Append(@"
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
        "));
        
        registry.ProvideFunction("pattern", s => s.Append(@"
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
        "));
        
        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }

    static string DomainWarpingNode_Function(
        [Slot(0, Binding.MeshUV0)] Vector2 UV,
        [Slot(1, Binding.None, 0f, 0f, 5.2f, 1.3f)] Vector4 Fbm_AddFactor_1,
        [Slot(2, Binding.None, 1.7f, 9.2f, 8.3f, 2.8f)] Vector4 Fbm_AddFactor_2,
        [Slot(3, Binding.None, 1f, 1f, 4f, 4f)] Vector4 Fbm_ScaleFactor_1,
        [Slot(4, Binding.None, 4f, 0f, 0f, 0f)] Vector1 Fbm_ScaleFactor_2,
        [Slot(5, Binding.None)] out Vector1 Pattern
    )
    {
        return @"{  
                Pattern = pattern(UV, Fbm_ScaleFactor_1, Fbm_ScaleFactor_2, Fbm_AddFactor_1, Fbm_AddFactor_2);
             }";
    }
}
