#ifndef HEXAGON_NODE_HLSL_INCLUDED
#define HEXAGON_NODE_HLSL_INCLUDED

///////////////////////////////////////////////////////////////////
//
// Original shader from: https://gist.github.com/sakrist/8706749
//
// added: hex cell pos, hex cell uv, hex cell index
//
///////////////////////////////////////////////////////////////////
void Hexagon_float(
    float2 UV,
    float Scale,
    out float Hexagon, // 六角形
    out float2 HexPos, // 六角形の位置
    out float2 HexUV, // 六角形内のUVを出力
    out float2 HexIndex // 六角形の番号
)
{
    float2 p = UV * Scale;
    p.x *= 1.15470053838; // x座標を2/√3倍 (六角形の横方向の大きさが√3/2倍になる)

    float isTwo = frac(floor(p.x) / 2.0) * 2.0; // 偶数列目なら1.0
    float isOne = 1.0 - isTwo; // 奇数列目なら1.0
    p.y += isTwo * 0.5; // 偶数列目を0.5ずらす

    float2 rectUV = frac(p); // 四角形タイル
    float2 grid = floor(p); // 四角形グリッド
    p = frac(p) - 0.5;
    float2 s = sign(p); // マス目の右上:(+1, +1) 左上:(-1, +1) 右下:(+1, -1) 左下:(-1, -1)
    p = abs(p); // 上下左右対称にする

    // 六角形タイルとして出力
    Hexagon = abs(max(p.x*1.5 + p.y, p.y*2.0) - 1.0);
            
    float isInHex = step(p.x*1.5 + p.y, 1.0); // 六角形の内側なら1.0
    float isOutHex = 1.0 - isInHex; // 六角形の外側なら1.0

    // 四角形マスのうち、六角形の外側の部分を補正するために使用する値
    float2 grid2 = float2(0, 0); 

    // 偶数列目と奇数列目を同時に加工
    grid2 = lerp(
        float2(s.x, +step(0.0, s.y)), // 奇数列目 (isTwo=0.0の場合はこちらを採用)
        float2(s.x, -step(s.y, 0.0)), // 偶数列目 (isTwo=1.0の場合はこちらを採用)
        isTwo) * isOutHex; // 六角形の外側だけ取り出す

    // 六角形の番号として出力
    HexIndex = grid + grid2; 

    // 六角形の座標として出力
    HexPos = HexIndex / Scale;

    // 六角形の内側ならrectUV、外側なら4つの六角形のUVを使う
    HexUV = lerp(rectUV, rectUV - s * float2(1.0, 0.5), isOutHex); 
}

#endif