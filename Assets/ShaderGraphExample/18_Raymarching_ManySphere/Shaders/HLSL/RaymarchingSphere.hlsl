#ifndef BOOK_RAYMARCHING_SPHERE_INCLUDED
#define BOOK_RAYMARCHING_SPHERE_INCLUDED

// 距離関数: 点pからオブジェクトまでの距離を求める
#define INTERVAL interval
float distance_func(float3 p, float size, float interval) {
    p = frac(p / INTERVAL) * INTERVAL - INTERVAL / 2.0; // -INTERVAL/2.0 ~ +INTERVAL/2.0 の繰り返しを作る
    return length(p) - size;
}

// 法線の計算
float3 getNormal(float3 p, float size, float interval) {
    float2 e = float2(0.0001, 0.0);
    return normalize(float3(
        distance_func(p + e.xyy, size, interval) - distance_func(p - e.xyy, size, interval),
        distance_func(p + e.yxy, size, interval) - distance_func(p - e.yxy, size, interval),
        distance_func(p + e.yyx, size, interval) - distance_func(p - e.yyx, size, interval)
    ));
}

float RaymarchingSphere_float(
    float2 UV,
    float3 CameraPos,
    float3 CameraDir,
    float3 CameraUp,
    float ObjectSize,
    float ObjectInterval,
    float RaymarchLoop,
    float RayStartLength,
    out float Hit,
    out float Distance,
    out float3 Normal)
{
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
}
#endif
