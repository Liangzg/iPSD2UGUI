using UnityEngine;

namespace subjectnerdagreement.psdexport
{
    /// <summary>
    /// 忽略节点资源
    /// </summary>
    public class IgnoreBinder : IBinding
    {
        public void StartBinding(GameObject gObj, string args, string layerName)
        {
            
        }

        public void ExitBinding(GameObject g, string args, string layerName)
        {
            GameObject.DestroyImmediate(g);
        }
    }
}