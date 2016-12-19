using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace subjectnerdagreement.psdexport
{
	[CustomEditor(typeof(PsdSetting))]
	public class PsdSettingEditor : Editor
	{
		protected PsdSetting m_PsdSetting;

		public void OnEnable()
		{
			m_PsdSetting = (PsdSetting)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Psd Path"))
			{
                string savePath = EditorUtility.SaveFolderPanel("Default psd file path", m_PsdSetting.PsdPath, string.Empty);
			    m_PsdSetting.PsdPath = savePath.Substring(Application.dataPath.Length + 1);
			}
			m_PsdSetting.PsdPath = GUILayout.TextArea(m_PsdSetting.PsdPath);

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			SerializedProperty defaultImportPath = serializedObject.FindProperty("m_DefaultImportPath");
			if (GUILayout.Button("Default Import Path"))
			{
				var path = EditorUtility.SaveFolderPanel("Default import path", 
					Path.Combine(Application.dataPath, m_PsdSetting.DefaultImportPath), string.Empty);

				if (path.StartsWith(Application.dataPath))
				{
					var startLen = Mathf.Min(Application.dataPath.Length + 1, path.Length);
					defaultImportPath.stringValue = path.Substring(startLen);
				}
				else
				{
					Debug.LogWarning("Not support path out of Application.dataPath.");
				}
			}
			defaultImportPath.stringValue = GUILayout.TextArea(defaultImportPath.stringValue);
			EditorGUILayout.EndHorizontal();

		    EditorGUILayout.Foldout(true, "Custom Asset Type");
            List<int> rmList = new List<int>();
            SerializedProperty assetDirArr = serializedObject.FindProperty("assetDirArr");
		    for (int i = 0; i < assetDirArr.arraySize; i++)
		    {
		        SerializedProperty element = assetDirArr.GetArrayElementAtIndex(i);

                GUILayout.BeginHorizontal();
                SerializedProperty keyProperty = element.FindPropertyRelative("key");
                keyProperty.stringValue = GUILayout.TextField(keyProperty.stringValue, GUILayout.Width(100));

                SerializedProperty folderProperty = element.FindPropertyRelative("folder");
                folderProperty.stringValue = GUILayout.TextField(folderProperty.stringValue);
                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    string savePath = EditorUtility.SaveFolderPanel("Save Path",
                        string.IsNullOrEmpty(folderProperty.stringValue) ? Application.dataPath : folderProperty.stringValue,
                        string.Empty);
                    if (savePath.StartsWith(Application.dataPath))
                        folderProperty.stringValue = savePath.Substring(Application.dataPath.Length + 1);
                }

                if (i != 0 && GUILayout.Button("X", GUILayout.Width(25)))
                    rmList.Add(i);
                GUILayout.EndHorizontal();
		    }
            
            rmList.Sort();

		    for (int i = rmList.Count - 1; i >= 0; i--)
		    {
                 assetDirArr.DeleteArrayElementAtIndex(rmList[i]);
		    }

		    if (GUILayout.Button("+"))
		    {
		        assetDirArr.InsertArrayElementAtIndex(assetDirArr.arraySize);

                SerializedProperty element = assetDirArr.GetArrayElementAtIndex(assetDirArr.arraySize - 1);
                SerializedProperty keyProperty = element.FindPropertyRelative("key");
                keyProperty.stringValue = "";

                SerializedProperty folderProperty = element.FindPropertyRelative("folder");
		        folderProperty.stringValue = "";
		    }

            serializedObject.ApplyModifiedProperties();
		}
	}
}
