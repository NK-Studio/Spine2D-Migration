using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NKStudio.Spine
{
    public static class Style
    {
        public const string InfoKorean = "변환하려고자 하는 스파인 버전을 작성합니다.";
        public const string InfoEnglish = "Write the version of Spine you want to convert.";
    }

    public class MigrationWindow : EditorWindow
    {
        private VisualTreeAsset _visualTree;
        

        [MenuItem("Tools/Spine2D/Migration Settings")]
        public static void ShowWindow()
        {
            MigrationWindow wnd = GetWindow<MigrationWindow>("Spine2D Migration");
            wnd.titleContent = new GUIContent("Spine2D Migration");
            wnd.minSize = new Vector2(350, 120);
            wnd.maxSize = new Vector2(350, 120);
        }

        private void InitRoot(VisualElement root)
        {
            string uxmlPath = AssetDatabase.GUIDToAssetPath("ce5817625fff50848b0482b2f2582b24");
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            VisualElement doc = visualTree.Instantiate();
            root.Add(doc);
        }
        
        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            InitRoot(root);
            
            TextField targetVersionField = root.Q<TextField>("TargetVersion");
            HelpBox infoBox = root.Q<HelpBox>("infoBox");

            infoBox.text = Application.systemLanguage == SystemLanguage.Korean 
                ? Style.InfoKorean 
                : Style.InfoEnglish;

            string path = Spine2DMigrationSettings.Instance.TargetVersion;

            targetVersionField.value = path;
            
            targetVersionField.RegisterValueChangedCallback(evt =>
            {
                string changeVersion = evt.newValue;
                
                Spine2DMigrationSettings.Instance.TargetVersion = changeVersion;
                EditorUtility.SetDirty(Spine2DMigrationSettings.Instance);
                
                targetVersionField.value = changeVersion;
            });
        }
    }
}