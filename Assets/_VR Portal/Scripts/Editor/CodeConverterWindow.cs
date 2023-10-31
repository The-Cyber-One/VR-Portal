using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CodeConverterWindow : EditorWindow
{
    [SerializeField] private string[] classes, parameters, structs;

    string _textContent;
    Texture2D _whiteTex, _backgroundColorTex, textColorTex, classColorTex, parameterColorTex, structColorTex;
    SerializedObject _serializedObject;
    SerializedProperty _classesProperty, _parametersProperty, _structsProperty;

    // Define the CSS styles for each color
    const string TEXT_COLOR = "#DCDCDC";
    const string BACKGROUND_COLOR = "#1E1E1E";
    const string COMMENT_COLOR = "#57A64A";
    const string STRING_COLOR = "#D69D85";
    const string NUMBER_COLOR = "#B5CEA8";
    const string KEYWORD_COLOR = "#569CD6";
    const string KEWORD_SPECIAL_COLOR = "#D8A0DF";
    const string CLASS_COLOR = "#4EC9B0";
    const string PARAMETER_COLOR = "#9CDCF8";
    const string STRUCT_COLOR = "#86C691";
    const string FUNCTION_COLOR = "#DCDC96";

    [MenuItem("Tools/CodeConverter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CodeConverterWindow));
    }

    private void OnEnable()
    {
        _whiteTex = new Texture2D(1, 1);
        _whiteTex.SetPixel(0, 0, new Color(1, 1, 1)); //#FFFFFF
        _whiteTex.Apply();

        _backgroundColorTex = new Texture2D(1, 1);
        _backgroundColorTex.SetPixel(0, 0, new Color(30 / 255f, 30 / 255f, 30 / 255f)); //#1E1E1E
        _backgroundColorTex.Apply();

        textColorTex = new Texture2D(1, 1);
        textColorTex.SetPixel(0, 0, new Color(220 / 255f, 220 / 255f, 220 / 255f)); //#DCDCDC
        textColorTex.Apply();

        classColorTex = new Texture2D(1, 1);
        classColorTex.SetPixel(0, 0, new Color(78 / 255f, 201 / 255f, 176 / 255f)); //#4EC9B0
        classColorTex.Apply();

        parameterColorTex = new Texture2D(1, 1);
        parameterColorTex.SetPixel(0, 0, new Color(156 / 255f, 220 / 255f, 248 / 255f)); //#9CDCF8
        parameterColorTex.Apply();

        structColorTex = new Texture2D(1, 1);
        structColorTex.SetPixel(0, 0, new Color(134 / 255f, 198 / 255f, 145 / 255f)); //#86C691
        structColorTex.Apply();

        ScriptableObject target = this;
        _serializedObject = new(target);
        _classesProperty = _serializedObject.FindProperty(nameof(classes));
        _parametersProperty = _serializedObject.FindProperty(nameof(parameters));
        _structsProperty = _serializedObject.FindProperty(nameof(structs));
    }


    private void OnGUI()
    {
        var backgroundStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            normal = new GUIStyleState() { background = _backgroundColorTex, textColor = Color.white },
            active = new GUIStyleState() { background = _whiteTex },
            stretchWidth = true,
        };
        var textStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            normal = new GUIStyleState() { background = textColorTex },
            active = new GUIStyleState() { background = _whiteTex },
            stretchWidth = true,
        };
        var classStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            normal = new GUIStyleState() { background = classColorTex },
            active = new GUIStyleState() { background = _whiteTex },
            stretchWidth = true,
        };
        var parameterStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            normal = new GUIStyleState() { background = parameterColorTex },
            active = new GUIStyleState() { background = _whiteTex },
            stretchWidth = true,
        };
        var structStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            normal = new GUIStyleState() { background = structColorTex },
            active = new GUIStyleState() { background = _whiteTex },
            stretchWidth = true,
        };

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Background", backgroundStyle)) GUIUtility.systemCopyBuffer = BACKGROUND_COLOR;
        if (GUILayout.Button("Text", textStyle)) GUIUtility.systemCopyBuffer = TEXT_COLOR;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Class", classStyle)) GUIUtility.systemCopyBuffer = CLASS_COLOR;
        EditorGUILayout.PropertyField(_classesProperty, true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Parameter", parameterStyle)) GUIUtility.systemCopyBuffer = PARAMETER_COLOR;
        EditorGUILayout.PropertyField(_parametersProperty, true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Struct", structStyle)) GUIUtility.systemCopyBuffer = STRUCT_COLOR;
        EditorGUILayout.PropertyField(_structsProperty, true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        
        _serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Convert code"))
        {
            GUIUtility.systemCopyBuffer = StyleCode(_textContent);
            EditorUtility.DisplayDialog("Code Converter", "The code was converted and copied to your clipboard", "Nice!");
        }
        _textContent = EditorGUILayout.TextArea(_textContent, GUILayout.ExpandHeight(true));
    }

    public string StyleCode(string code)
    {
        // Define regular expressions to match various code elements
        string commentRegex = @"(?=\/\/)\s*(.*)";
        string stringRegex = @"""(.*?)\""";
        string numberRegex = @"\b(\d+)\b";
        string keywordRegex = @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|var)\b";
        string specialKeywordRegex = @"\b(if|for|while|switch|foreach|return|yield)\b";
        string classRegex = $@"\b({string.Join("|", classes)})\b";
        string parameterRegex = $@"\b({string.Join("|", parameters)})\b";
        string structRegex = $@"\b({string.Join("|", structs)})\b";
        string functionRegex = @"\b(\w+)(?=\()";

        List<string> regexList = new List<string>()
        {
            commentRegex,
            stringRegex,
            numberRegex,
            keywordRegex,
            specialKeywordRegex,
            functionRegex,
        };
        if (classes.Length > 0) regexList.Add(classRegex);
        if (parameters.Length > 0) regexList.Add(parameterRegex);
        if (structs.Length > 0) regexList.Add(structRegex);

        Regex regex = new(string.Join("|", regexList));

        string styledCode = regex.Replace(code, match =>
        {
            if (Regex.IsMatch(match.Value, commentRegex)) return $"<mark style=\"background-color:#0000;color:{COMMENT_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (Regex.IsMatch(match.Value, stringRegex)) return $"<mark style=\"background-color:#0000;color:{STRING_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (Regex.IsMatch(match.Value, numberRegex)) return $"<mark style=\"background-color:#0000;color:{NUMBER_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (Regex.IsMatch(match.Value, keywordRegex)) return $"<mark style=\"background-color:#0000;color:{KEYWORD_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (Regex.IsMatch(match.Value, specialKeywordRegex)) return $"<mark style=\"background-color:#0000;color:{KEWORD_SPECIAL_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (classes.Length > 0 && Regex.IsMatch(match.Value, classRegex)) return $"<mark style=\"background-color:#0000;color:{CLASS_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (parameters.Length > 0 && Regex.IsMatch(match.Value, parameterRegex)) return $"<mark style=\"background-color:#0000;color:{PARAMETER_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            if (structs.Length > 0 && Regex.IsMatch(match.Value, structRegex)) return $"<mark style=\"background-color:#0000;color:{STRUCT_COLOR}\" class=\"has-inline-color\">{match}</mark>";
            
            return $"<mark style=\"background-color:#0000;color:{FUNCTION_COLOR}\" class=\"has-inline-color\">{match}</mark>"; // Function is the last option
        });

        return styledCode;
    }
}
