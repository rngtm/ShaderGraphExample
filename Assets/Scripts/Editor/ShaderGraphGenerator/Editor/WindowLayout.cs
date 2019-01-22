using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ShaderGraphGenerator
{
    public static class WindowLayout
    {
        public static bool LoadWindowLayout(string path, bool newProjectLayoutWasCreated)
        {
            var asm = Assembly.Load("UnityEditor");
            var classType = asm.GetType("UnityEditor.WindowLayout");
            // var method 
            Debug.Log(classType);

            var method = classType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
            Debug.Log(method);

            method.Invoke(
                null, 
                new object[] {path, newProjectLayoutWasCreated } // paremeters
                );

            return false;
        }
    }
}