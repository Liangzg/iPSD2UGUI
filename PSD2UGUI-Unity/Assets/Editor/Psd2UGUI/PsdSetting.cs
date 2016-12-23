
using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if !SLUA_STANDALONE
using UnityEngine;
#endif

namespace subjectnerdagreement.psdexport
{
    [Serializable]
    public class AssetDir
    {
        [SerializeField]
        public string key = string.Empty;
        [SerializeField]
        public string folder = string.Empty;
    }

	public class PsdSetting : ScriptableObject
	{
		private const string DEFAULT_IMPORT_PATH = "UI";
	    private const string SETTING_PATH = "Assets/PSD/Resources/psdsetting.asset";

		public string PsdPath = "";


		public string DefaultImportPath
		{
			get
			{
				if (string.IsNullOrEmpty(m_DefaultImportPath))
					return DEFAULT_IMPORT_PATH;
				return m_DefaultImportPath;
			}
		}


	    public string DefaultFontPath
	    {
	        get { return "Assets/" + defaultFont; }
	    }

		[SerializeField]
		protected string m_DefaultImportPath;
        
	    public AssetDir[] assetDirArr;

        #region ------------默认的文本字体--------------------
        /// <summary>
        /// 默认文本的字符大小
        /// </summary>
        [SerializeField]
	    public int DefaultTextSize = 20;
        /// <summary>
        /// 默认字体
        /// </summary>
        [SerializeField]
	    public string defaultFont;

        #endregion

        private static PsdSetting _instance = null;
		public static PsdSetting Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<PsdSetting>("psdsetting");

#if UNITY_EDITOR
					if (_instance == null)
					{
						_instance = PsdSetting.CreateInstance<PsdSetting>();
					    string dirPath = Path.GetDirectoryName(SETTING_PATH);
					    if (!Directory.Exists(dirPath))
					        Directory.CreateDirectory(dirPath);
						AssetDatabase.CreateAsset(_instance, SETTING_PATH);
					}
#endif
				}

				return _instance;
			}
		}

#if UNITY_EDITOR && !SLUA_STANDALONE
		[MenuItem("PSD/Setting")]
		public static void Open()
		{
			Selection.activeObject = Instance;
		}
#endif


	    public string GetAssetFolder(string assetType)
	    {
	        foreach (AssetDir assetDir in assetDirArr)
	        {
	            if (assetDir.key == assetType)
	            {
	                return assetDir.folder;
	            }
	        }
	        return DefaultImportPath;
	    }


	    public void insertAssetMapFirst(string key, string path)
	    {
            AssetDir dir = new AssetDir()
            {
                key = key,
                folder = path
            };

            AssetDir[] newAssetDirs = new AssetDir[assetDirArr.Length + 1];
            Array.Copy(assetDirArr, 0 ,  newAssetDirs, 1 , assetDirArr.Length);
            newAssetDirs[0] = dir;
            assetDirArr = newAssetDirs;
        }

	}
}
