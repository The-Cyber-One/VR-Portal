using System.IO;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(CppGlue))]
[CanEditMultipleObjects]
public class CppGlueEditor : Editor
{
    SerializedProperty libraryName;

    void OnEnable()
    {
        // Grabbing the serialized library name to update it manualy
        libraryName = serializedObject.FindProperty("libraryName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Find all dlls that are build and show them as options
        var files = Directory.GetFiles($"Assets/Scripts/Cpp/VRPortal/x64/Release", "*.dll");
        var fileNames = files.Select(path => Path.GetFileName(path)).ToArray();
        int libraryIndex = 0;
        for (int i = 0; i < fileNames.Length; i++)
        {
            if (fileNames[i] == libraryName.stringValue)
            {
                libraryIndex = i;
                break;
            }
        }
        libraryName.stringValue = fileNames[EditorGUILayout.Popup("DLL to load", libraryIndex, fileNames)];

        serializedObject.ApplyModifiedProperties();
    }
}