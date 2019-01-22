using UnityEngine;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace ShaderGraphGenerator
{
    [CreateAssetMenu]
    public class GeneratorSettings : ScriptableObject
    {
        public const string FilePath = "Assets/Scripts/Editor/ShaderGraphGenerator/Data/GeneratorSettings.asset";
        [SerializeField] private SceneAsset m_TemplateScene = null;
        [SerializeField] private Shader m_TemplateShader = null;
        public SceneAsset TemplateScene => m_TemplateScene;
        public Shader TemplateShader => m_TemplateShader;
    }
}