#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace NKStudio.Spine
{
    public class Spine2DMigrationSettings : ScriptableObject
    {
        public string TargetVersion = "4.1.24";

        private const string FileName = "Spine2D Migration Settings";
        private const string SettingFileDirectory = "Assets/Editor";
        private const string SettingFilePath = "Assets/Editor/" + FileName + ".asset";

        private static Spine2DMigrationSettings _instance;
        public static Spine2DMigrationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<Spine2DMigrationSettings>(SettingFilePath);
                    if (_instance == null)
                    {
#if UNITY_EDITOR
                        if (!AssetDatabase.IsValidFolder(SettingFileDirectory))
                            AssetDatabase.CreateFolder("Assets", "Resources");
#endif
                        _instance = AssetDatabase.LoadAssetAtPath<Spine2DMigrationSettings>(SettingFilePath);
#if UNITY_EDITOR
                        if (_instance == null)
                        {
                            _instance = CreateInstance<Spine2DMigrationSettings>();
                            _instance.name = FileName;

                            AssetDatabase.CreateAsset(_instance, SettingFilePath);
                        }
#endif
                    }
                }

                return _instance;
            }
        }
    }
}