using UnityEngine;

public class CameraShaderSync : MonoBehaviour
{
    [SerializeField] Material _material; // カメラと同期させる対象のマテリアル
    int _cameraUp;
    int _cameraDir;
    int _cameraPos;

    void Start()
    {
        _cameraUp = Shader.PropertyToID("_CameraUp");
        _cameraDir = Shader.PropertyToID("_CameraDir");
        _cameraPos = Shader.PropertyToID("_CameraPos");
    }

    void Update()
    {
        // 位置と向きをシェーダーに渡す
        _material.SetVector(_cameraUp, transform.up);
        _material.SetVector(_cameraDir, transform.forward);
        _material.SetVector(_cameraPos, transform.position);
    }
}
