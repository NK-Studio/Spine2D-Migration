using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NKStudio.Spine
{
    public class SpineLegacyMigration : AssetPostprocessor
    {
        // PostprocessOrder를 0으로 설정하여 다른 Postprocessor보다 먼저 실행되도록 합니다.
        public override int GetPostprocessOrder() => 0;
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0)
                return;

            //스크립트 편집에 의한 Import인 경우 Event.current는 null이므로 null체크를 하기
            if (Event.current == null ||
                Event.current.type != EventType.DragPerform)
                return;

            foreach (string asset in importedAssets)
            {
                // asset의 확장자가 .json인 경우
                if (asset.EndsWith(".json"))
                {
                    // JSON 파일을 TextAsset으로 변환
                    TextAsset json = AssetDatabase.LoadAssetAtPath<TextAsset>(asset);

                    // "skeleton" 객체에서 "spine" 값을 변경
                    if (MiniJSON.Json.Deserialize(json.text) is Dictionary<string, object> parsedData)
                    {
                        Dictionary<string, object> skeleton = parsedData["skeleton"] as Dictionary<string, object>;

                        // 레거시 스파인인 경우
                        if (!IsLegacySpine(skeleton))
                            return;

                        if (skeleton != null) 
                            skeleton["spine"] = Spine2DMigrationSettings.Instance.TargetVersion;

                        // 변경된 데이터를 다시 JSON 문자열로 직렬화
                        string newJsonData = MiniJSON.Json.Serialize(parsedData);

                        // 파일에 쓰기
                        SaveJson(asset, newJsonData);

                        // 새로고침
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
        
        /// <summary>
        /// Spine2D 데이터가 레거시 스파인인지 확인합니다.
        /// </summary>
        /// <param name="data">Spine2D Data</param>
        /// <returns>JObject가 레거시 Spine을 나타내는 경우 true이고, 그렇지 않으면 false입니다. 그렇지 않으면 거짓입니다.</returns>
        private static bool IsLegacySpine(IReadOnlyDictionary<string, object> data)
        {
            // spine 값이 "3"으로 시작하면 레거시 스파인
            if (data["spine"] is string spine)
            {
                bool isFirstNumber = spine.StartsWith("3");
                return isFirstNumber;
            }

            return false;
        }
        
        /// <summary>
        /// 주어진 JSON 데이터를 지정된 자산 경로의 파일에 저장합니다.
        /// </summary>
        /// <param name="assetPath">파일이 저장될 자산 경로입니다.</param>
        /// <param name="jsonData">저장할 JSON 데이터입니다.</param>
        private static void SaveJson(string assetPath, string jsonData)
        {
            string jsonPathWithoutAssets = assetPath.Replace("Assets", "");
            string absolutePath = Application.dataPath + jsonPathWithoutAssets;
            File.WriteAllText(absolutePath, jsonData);
        }
    }
}
