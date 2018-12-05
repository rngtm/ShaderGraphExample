using System.Reflection;
using UnityEditor.ShaderGraph;
using UnityEngine;

/// <summary>
/// レイマーチングで球をたくさん表示するカスタムノード
/// </summary>
[Title("Raymarching", "Raymarch Sphere")]
public class RaymarchingSphereNode : CodeFunctionNode
{
    public RaymarchingSphereNode()
    {
        name = "Raymarching(Sphere)";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("RaymarchingNode_Function",
            BindingFlags.Static | BindingFlags.NonPublic);
    }

    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        registry.ProvideFunction("distance_func", s => s.Append(@"
            // 距離関数: 点pからオブジェクトまでの距離を求める
            #define INTERVAL interval
            float distance_func(float3 p, float size, float interval) {
                p = frac(p / INTERVAL) * INTERVAL - INTERVAL / 2.0; // -INTERVAL/2.0 ~ +INTERVAL/2.0 の繰り返しを作る
                return length(p) - size;
            }
        "));

        registry.ProvideFunction("getNormal", s => s.Append(@"
            // 法線の計算
            float3 getNormal(float3 p, float size, float interval) {
                float2 e = float2(0.0001, 0.0);
                return normalize(float3(
                    distance_func(p + e.xyy, size, interval) - distance_func(p - e.xyy, size, interval),
                    distance_func(p + e.yxy, size, interval) - distance_func(p - e.yxy, size, interval),
                    distance_func(p + e.yyx, size, interval) - distance_func(p - e.yyx, size, interval)
                ));
            }
        "));


        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }

    static string RaymarchingNode_Function(
        [Slot(0, Binding.MeshUV0)] Vector2 UV,
        [Slot(1, Binding.None, 0f, 0f, 4f, 0f)] Vector3 CameraPos, // カメラ位置
        [Slot(2, Binding.None, 0f, 0f, -1f, 0f)] Vector3 CameraDir, // カメラの向きベクトル
        [Slot(3, Binding.None, 0f, 1f, 0f, 0f)] Vector3 CameraUp,  // カメラの上方向ベクトル
        [Slot(4, Binding.None, 1f, 0f, 0f, 0f)] Vector1 ObjectSize, // 球のサイズ
        [Slot(5, Binding.None, 2f, 0f, 0f, 0f)] Vector1 ObjectInterval, // 球の配置間隔
        [Slot(6, Binding.None, 32f, 0f, 0f, 0f)] Vector1 RaymarchLoop, // レイマーチングのループ回数(この数を大きくすると遠くまで描画されるようになりますが重くなります)
        [Slot(7, Binding.None, 0f, 0f, 0f, 0f)] Vector1 RayStartLength, // レイの開始位置
        [Slot(10, Binding.None)] out Vector1 Hit, // レイがオブジェクトにぶつかったら1.0, ぶつからなかったら0.0
        [Slot(11, Binding.None)] out Vector1 Distance, // レイマーチングでレイが進んだ距離
        [Slot(12, Binding.None)] out Vector3 Normal  // オブジェクト上の法線
    )
    {
        Normal = Vector3.zero;
        return @"{
                #define MAX_REPEAT 100

                float2 p = UV - 0.5;

                // カメラに関する情報(Position, Direction, Up)
                #define cPos CameraPos
                #define cDir normalize(CameraDir)
                #define cUp normalize(CameraUp)
                #define cSide normalize(cross(cUp, cDir))

                // レイマーチング
                float3 ray = normalize(p.x * cSide + p.y * cUp + 1.0 * cDir); // レイの向きベクトル
                float3 rPos = cPos; // レイ位置
                float rLength = RayStartLength;// レイが進む長さ
                float dist = 0.0; // レイとオブジェクト間の距離
                for (int i = 0; i < min(RaymarchLoop, MAX_REPEAT); i++)
                {
                    dist = distance_func(rPos, ObjectSize, ObjectInterval); // レイ位置からオブジェクトまでの距離を求める
                    rLength += dist; // 距離を足す(レイを進める)
                    rPos = cPos + ray * rLength; // レイ位置の更新
                }

                Hit = step(dist, 0.01); // レイがオブジェクトにある程度近かったら1.0を出力、それ以外は0.0を出力
                Distance = rLength; // レイが進んだ距離を出力
                Normal = saturate(getNormal(rPos, ObjectSize, ObjectInterval)); // レイの交点におけるオブジェクト上の法線を出力
            }";
    }
}