using UnityEngine;

namespace subjectnerdagreement.psdexport
{
    public class NameBinder : IBinding
    {
        public void StartBinding(GameObject gObj, string args, string layerName)
        {
            gObj.name = args;
        }

        public void ExitBinding(GameObject g, string args, string layerName)
        {
           
        }
    }
}