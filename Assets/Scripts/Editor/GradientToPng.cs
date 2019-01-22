using UnityEngine;
using UnityEditor;
using System.IO;

public class GradientToPng : EditorWindow
{
    [SerializeField]
    private Gradient m_Gradient;

    [MenuItem("Tools/Gradient To Png")]
    static void Open()
    {
        GetWindow<GradientToPng>();
    }

    private void OnGUI()
    {
        if (m_Gradient == null)
        {
            m_Gradient = new Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey { color = Color.black, time = 0f },
                    new GradientColorKey { color = Color.white, time = 1f },
                }
            };
        }
        m_Gradient = EditorGUILayout.GradientField(m_Gradient);

        if (GUILayout.Button("Save"))
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Gradient as png", "New Gradient", "png", "");
            SaveGradient(Path.GetFullPath(path));
        }
    }

    void SaveGradient(string path)
    {
        Texture2D tex = new Texture2D(256, 1);
        for (int x = 0; x < 256; x++)
        {
            var color = m_Gradient.Evaluate(x / 255f);
            tex.SetPixel(x, 0, color);
        }
        tex.Apply();

        var png = tex.EncodeToPNG();
        File.WriteAllBytes(path, png);

        AssetDatabase.Refresh();
    }
}