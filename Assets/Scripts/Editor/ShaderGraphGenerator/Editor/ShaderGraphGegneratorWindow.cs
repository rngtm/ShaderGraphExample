using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;
using System.IO;

namespace ShaderGraphGenerator
{
    public class ShaderGraphGegneratorWindow : EditorWindow
    {
        readonly GUIContent GenerateButtonContent = new GUIContent("Generate");
        readonly GUIContent GenerateFolderButtonContent = new GUIContent("Generate Folder");

        [System.NonSerialized] private GUIStyle m_ButtonStyle;
        [System.NonSerialized] private GeneratorSettings m_GeneratorSettings;

        [MenuItem("Tools/ShaderGraph Generator")]
        static void Open()
        {
            GetWindow<ShaderGraphGegneratorWindow>("ShaderGraph Generator");
        }

        void OnEnable()
        {
            m_GeneratorSettings = m_GeneratorSettings ?? AssetDatabase.LoadAssetAtPath(GeneratorSettings.FilePath, typeof(GeneratorSettings)) as GeneratorSettings;
        }

        void OnGUI()
        {
            DrawHeader();

            if (m_ButtonStyle == null)
            {
                m_ButtonStyle = new GUIStyle(GUI.skin.button);
                m_ButtonStyle.wordWrap = true;
            }

            if (m_GeneratorSettings == null)
            {
                EditorGUILayout.LabelField("GeneratorSettings.asset not found");
                m_GeneratorSettings = EditorGUILayout.ObjectField(m_GeneratorSettings, typeof(GeneratorSettings), false) as GeneratorSettings;
            }

            EditorGUI.BeginDisabledGroup(m_GeneratorSettings == null);
            {
                // // generate folder
                // if (GUILayout.Button(GenerateFolderButtonContent, m_ButtonStyle))
                // {
                //     EditorApplication.ExecuteMenuItem("Assets/Create/Folder");
                // }
                // generate data
                if (GUILayout.Button(GenerateButtonContent, m_ButtonStyle))
                {
                    string directoryPath = "";
                    foreach (var obj in Selection.objects)
                    {
                        if (obj == null) { continue; }
                        if (!AssetDatabase.IsMainAsset(obj)) continue;

                        var path = AssetDatabase.GetAssetPath(obj);
                        bool isDirectory = Directory.Exists(path);
                        directoryPath = (isDirectory) ? path : Directory.GetParent(path).FullName;
                        break;
                    }

                    var savePath = EditorUtility.SaveFilePanel(
                        "Generate ShaderGraph Data",
                        directoryPath,
                        "New Data",
                        ""
                    );
                    Debug.Log($"savePath: {savePath}");

                    if (!string.IsNullOrEmpty(savePath))
                    {
                        GenerateData(savePath);
                    }
                }
                

            }

            EditorGUI.EndDisabledGroup();
        }

        void GenerateData(string path)
        {
            var parent = Directory.GetParent(path).FullName;
            parent = parent.Substring(Application.dataPath.Length - "Assets".Length); // full path -> unity path
            var fileName = Path.GetFileName(path);

            var rootFolderPath = AssetDatabase.GUIDToAssetPath(
                AssetDatabase.CreateFolder(parent, fileName)
                );
            var materialFolderPath = AssetDatabase.GUIDToAssetPath(
                AssetDatabase.CreateFolder(rootFolderPath, "Materials")
                );
            var shaderFolderPath = AssetDatabase.GUIDToAssetPath(
                AssetDatabase.CreateFolder(rootFolderPath, "Shaders")
                );

            // shader
            var templateShaderPath = AssetDatabase.GetAssetPath(m_GeneratorSettings.TemplateShader);
            var templateShaderExt = Path.GetExtension(templateShaderPath);
            var newShaderPath = Path.Combine(shaderFolderPath, fileName) + templateShaderExt;
            AssetDatabase.CopyAsset(templateShaderPath, newShaderPath);
            var newShader = AssetDatabase.LoadAssetAtPath(newShaderPath, typeof(Shader)) as Shader;

            // material
            var newMaterialPath = Path.Combine(materialFolderPath, fileName) + ".mat";
            var newMaterial = new Material(newShader);
            AssetDatabase.CreateAsset(newMaterial, newMaterialPath);

            // scene
            var templateScenePath = AssetDatabase.GetAssetPath(m_GeneratorSettings.TemplateScene);
            var newScenePath = Path.Combine(rootFolderPath, fileName) + ".unity";
            AssetDatabase.CopyAsset(templateScenePath, newScenePath);

            // bind Material to MeshRenderer
            var newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Additive);
            var meshRenderer = newScene.GetRootGameObjects()
                .Select(go => go.GetComponent<MeshRenderer>())
                .FirstOrDefault(mr => mr != null);
            if (meshRenderer != null) { meshRenderer.material = newMaterial; }
            EditorSceneManager.SaveScene(newScene);
            EditorSceneManager.CloseScene(newScene, true);

            // refresh 
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(newShader);

            // open scene
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(newScenePath);
            }
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Load Layout", EditorStyles.toolbarButton))
                {
                    WindowLayout.LoadWindowLayout("Assets/Layouts/ShaderGraph.wlt", false);
                }
                if (GUILayout.Button("Settings", EditorStyles.toolbarButton))
                {
                    EditorGUIUtility.PingObject(m_GeneratorSettings);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}