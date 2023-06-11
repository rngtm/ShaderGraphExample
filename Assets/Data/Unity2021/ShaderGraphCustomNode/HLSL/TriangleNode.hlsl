#ifndef TRIANGLE_NODE_HLSL_INCLUDED
#define TRIANGLE_NODE_HLSL_INCLUDED

// float fmod(float a, float b)
// {
//     return a - floor(a / b) * b;
// }

void TriangleNode_float(
    float2 UV,
    float Scale,
    out float Triangle, // 三角形
    out float2 TrianglePosition // 三角形
){
    #define N Scale
    float2 p = UV;
    p.x *= 0.86602; // sqrt(3)/2倍

    float isTwo = fmod(floor(p.y * N), 2.0); // 偶数列目なら1.0
    float isOne = 1.0 - isTwo; // 、奇数列目なら1.0

    // xy座標0~1の正方形をタイル状に複数個並べる
    p = p * N;
    p.x += isTwo * 0.5; // 偶数列目を0.5ズラす

    float2 p_index = floor(p); // 正方形の番号
    float2 p_rect = frac(p);
    p = p_rect; // 正方形内部の座標

    float xSign = sign(p.x - 0.5); // タイルの右側なら+1.0, 左側なら-1.0
    p.x = abs(0.5 - p.x); // x=0.5を軸として左右対称にする

    float isInTriangle = step(p.x * 2.0 + p.y, 1.0); // 三角形の内部にある場合は1.0
    float isOutTriangle = 1.0 - isInTriangle; // 三角形の外側にある場合は1.0

    // 中央の三角形
    float w1 = max( p.x * 2.0 + p.y, 1.0 - p.y * 1.5 ); 
  
    // 右上(左上)の三角形
    p = float2(0.5, 1.0) - p;
    float w2 = max(p.x * 2.0 + p.y, 1.0 - p.y * 1.5 );

    // 三角形グラデーション
    Triangle = lerp(1.0 - w2, 1.0 - w1, isInTriangle) / 0.6;
  
    // 三角形の位置
    float2 triangleIndex = p_index + float2(
       isOutTriangle * xSign / 2.0 // 左上の部分は-0.5、右上の部分は+0.5
      + isOne / 2.0, // 基数列目の三角形は横に0.5ズレているので+0.5する
      0.0
    );
  
    // 三角形の座標
    TrianglePosition = triangleIndex / N;
}

#endif