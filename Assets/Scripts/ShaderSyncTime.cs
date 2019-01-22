using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShaderSyncTime : MonoBehaviour
{
    [SerializeField] string m_TargetPropertyName;
    [SerializeField] private float m_MinValue = 0f;
    [SerializeField] private float m_MaxValue = 1f;
    Material m_Material;
    int m_PropertyId;

    void Start()
    {
        m_PropertyId = Shader.PropertyToID(m_TargetPropertyName);
        m_Material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float delta = m_MaxValue - m_MinValue;
        float value = (Time.time - m_MinValue) % delta + m_MinValue;
        m_Material.SetFloat(m_PropertyId, value);
    }
}
