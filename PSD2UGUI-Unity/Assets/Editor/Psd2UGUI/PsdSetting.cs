
using System;
using System.Collections.Generic;
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
		[SerializeField]
		protected string m_DefaultImportPath;
        
	    public AssetDir[] assetDirArr;
          
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
						AssetDatabase.CreateAsset(_instance, "Assets/PSD/Resources/psdsetting.asset");
					}
#endif
				    if (_instance.assetDirArr.Length == 0 || _instance.assetDirArr[0].key != "default")
				    {
                        _instance.insertAssetMapFirst( "default",String.Empty);
                    }
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
	        return assetDirArr[0].folder;
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
