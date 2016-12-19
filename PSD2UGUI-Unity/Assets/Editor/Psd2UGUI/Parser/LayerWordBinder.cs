////////////////////////////////////////////////
/// auth: liangzg
/// mail: game.liangzg@foxmail.com
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace subjectnerdagreement.psdexport
{
    /// <summary>
    /// 词法绑定 
    /// </summary>
    public class LayerWordBinder
    {
        private static Dictionary<string , Action<string , GameObject>> importParser = new Dictionary<string, Action<string, GameObject>>();

        static LayerWordBinder()
        {
            registWord();
        }

        public static void registWord()
        {
            importParser["anchor"] = importAnchor;
            importParser["name"] = importName;
            importParser["img"] = importImageAsset;

            registComponent();
        }

        private static void importImageAsset(string s, GameObject gameObject)
        {
            
        }

        private static void importName(string s, GameObject gameObject)
        {
            gameObject.name = s;
        }

        /// <summary>
        /// 定义结点的对齐方式
        /// </summary>
        /// <param name="arg"></param>
        private static void importAnchor(string arg, GameObject mainObj)
        {
//            throw new NotImplementedException();
        }

        public static void registComponent()
        {
            importParser["button"] = importButtonComponent;
            importParser["toggle"] = importToggleButton;
            importParser["slider"] = importSlider;
            importParser["scrollview"] = importScrollView;
            importParser["scrollbar"] = importScrollBar;
            importParser["inputfield"] = importInputField;
        }

        private static void importInputField(string s, GameObject mainObj)
        {
            Transform bgTrans = findChildComponent<Transform>(mainObj, "background");
            copyRectTransform(bgTrans.gameObject, mainObj, true);

            Text holderText = findChildComponent<Text>(mainObj, "holder");
            if (holderText) {
                RectTransform holderTrans = holderText.GetComponent<RectTransform>();
                holderTrans.anchorMin = Vector2.zero;
                holderTrans.anchorMax = Vector2.one;
                holderTrans.sizeDelta = Vector2.zero;
                holderTrans.offsetMin = new Vector2(10, 6);
                holderTrans.offsetMax = new Vector2(-10, -7);
            }

            GameObject textObj = CreateUIObject("text" , mainObj);
            Text text = textObj.AddComponent<Text>();
            text.text = "";
            text.supportRichText = false;
            if (holderText)
            {
                text.alignment = holderText.alignment;
                text.fontSize = holderText.fontSize - 1;
            }

            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            InputField inputField = swapComponent<InputField>(mainObj);
            inputField.textComponent = text;
            inputField.placeholder = holderText;
        }

        private static void importScrollBar(string s, GameObject mainObj)
        {
            Image oldImg = findChildComponent<Image>(mainObj, "background");
            copyRectTransform(oldImg.gameObject, mainObj , true);

            GameObject sliderArea = CreateUIObject("Sliding Area" , mainObj);
            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = findChildComponent<RectTransform>(mainObj, "handle");
            handleRect.transform.SetParent(sliderArea.transform);
            handleRect.sizeDelta = new Vector2(20, 20);
            handleRect.anchoredPosition = Vector2.zero;

            Scrollbar scrollbar = swapComponent<Scrollbar>(mainObj);
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleRect.GetComponent<Image>();
        }

        private static void importSlider(string s, GameObject mainObj)
        {
            Slider slider = swapComponent<Slider>(mainObj);

            Image imgBg = findChildComponent<Image>(mainObj, "background");
            imgBg.type = Image.Type.Sliced;
            slider.targetGraphic = imgBg;

            RectTransform sliderBgTrans = slider.targetGraphic.GetComponent<RectTransform>();
            RectTransform mainRectTrans = mainObj.GetComponent<RectTransform>();
            mainRectTrans.sizeDelta = sliderBgTrans.sizeDelta;
            mainRectTrans.anchoredPosition3D = sliderBgTrans.anchoredPosition3D;

            sliderBgTrans.anchorMin = new Vector2(0, 0.25f);
            sliderBgTrans.anchorMax = new Vector2(1, 0.75f);
            sliderBgTrans.sizeDelta = Vector2.zero;
            sliderBgTrans.anchoredPosition3D = Vector3.zero;

            slider.fillRect = findChildComponent<RectTransform>(mainObj, "fill");
            if (slider.fillRect)
            {
                GameObject fillRoot = CreateUIObject("fillArea" , mainObj);
                fillRoot.transform.localScale = Vector3.one;
                fillRoot.transform.localPosition = Vector3.zero;
                RectTransform rectTrans = fillRoot.GetComponent<RectTransform>();
                rectTrans.anchorMin = new Vector2(0 , 0.25f);
                rectTrans.anchorMax = new Vector2(1 , 0.75f);
                rectTrans.pivot = Vector2.one * 0.5f;
                rectTrans.sizeDelta = new Vector2(-20, 0);

                slider.fillRect.transform.SetParent(fillRoot.transform);
                slider.fillRect.anchoredPosition = Vector3.zero;
                Image fillImage = slider.fillRect.GetComponent<Image>();
                fillImage.type = Image.Type.Sliced;

                slider.fillRect.anchorMax = new Vector2(0 , 1);
                slider.fillRect.sizeDelta = new Vector2(10, 0);
            }

            slider.handleRect = findChildComponent<RectTransform>(mainObj, "handle");
            if (slider.handleRect)
            {
                GameObject handleArea = CreateUIObject("HandleSlideArea" , mainObj);
                handleArea.transform.localScale = Vector3.one;
                handleArea.transform.localPosition = Vector3.zero;

                RectTransform rectTrans = handleArea.GetComponent<RectTransform>();
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.pivot = Vector2.one * 0.5f;
                rectTrans.sizeDelta = new Vector2(-20, 0);

                slider.handleRect.transform.SetParent(handleArea.transform);
                slider.handleRect.anchoredPosition = Vector3.zero;
                slider.handleRect.anchorMax = new Vector2(0, 1);
                slider.handleRect.sizeDelta = new Vector2(20 , 0);
            }
        }

        private static void importScrollView(string s, GameObject mainObj)
        {
            ScrollRect scrollview = swapComponent<ScrollRect>(mainObj);
            scrollview.viewport = findChildComponent<RectTransform>(mainObj, "viewport");
            scrollview.content = findChildComponent<RectTransform>(mainObj, "viewport/content");

            Scrollbar hbar = findChildComponent<Scrollbar>(mainObj, "hbar");
            if (hbar != null)
            {
                scrollview.horizontalScrollbar = hbar;
                scrollview.horizontal = true;
            }

            Scrollbar vbar = findChildComponent<Scrollbar>(mainObj, "vbar");
            if (vbar != null)
            {
                scrollview.verticalScrollbar = hbar;
                scrollview.vertical = true;
            }
        }

        private static void importToggleButton(string s, GameObject mainObj)
        {
            Toggle toggle = swapComponent<Toggle>(mainObj);
            
            Image imgBtn = findChildComponent<Image>(mainObj, "background");
            toggle.image = imgBtn;

            Image imgMark = findChildComponent<Image>(mainObj, "checkmark");
            toggle.graphic = imgMark;
            toggle.isOn = true;
        }

        private static void importButtonComponent(string s, GameObject mainObj)
        {
            Button button = swapComponent<Button>(mainObj);
            Image imgBtn = findChildComponent<Image>(mainObj, "imgBtn");
            button.targetGraphic = imgBtn;
        }


        public static Action<string, GameObject> GetParser(string key)
        {
            if (importParser.ContainsKey(key)) return importParser[key];
            return null;
        }


        public static T swapComponent<T>(GameObject gObj) where T : Component
        {
            T instance = gObj.GetComponent<T>();
            if (instance == null)
            {
                instance = gObj.AddComponent<T>();
            }
            return instance;
        }

        public static T findChildComponent<T>(GameObject gObj, string hierarchy)
            where T : Component
        {
            Transform destTrans = null;
            foreach (Transform childTrans in gObj.transform)
            {
                if (childTrans.name.StartsWith(hierarchy))
                {
                    destTrans = childTrans;
                    break;
                }
            }
            if (destTrans == null) return null;

            return destTrans.GetComponent<T>();
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }
        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static void copyRectTransform(GameObject src, GameObject dest , bool copyImage = false)
        {
            RectTransform srt = src.GetComponent<RectTransform>();
            RectTransform drt = dest.GetComponent<RectTransform>();

            drt.anchoredPosition = srt.anchoredPosition;
            drt.anchorMin = srt.anchorMin;
            drt.anchorMax = srt.anchorMax;
            drt.pivot = srt.pivot;

            drt.localScale = Vector3.one;

            if (copyImage)
            {
                Image imgRoot = src.GetComponent<Image>();
                Image destImg = dest.AddComponent<Image>();
                destImg.sprite = imgRoot.sprite;
                destImg.SetNativeSize();

                GameObject.DestroyImmediate(src);                
            }
        }

    }



}
