using System;
using Elarion.Managers;
using Elarion.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Utility {
    public static class UIHelper {
        public const string BlurShaderPath = "Materials/FrostedGlass"; 
        public const string ShadowImagePath = "Sprites/Shadow"; // needs an offset to function properly
        public const float ShadowImageOffset = -29;
        private const int DefaultRenderTextureDepth = 30;

        public static T Create<T>(string name = null, Transform parent = null) where T : Component {
            return CreateGO(name, parent).AddComponent<T>();
        }
        
        public static Image CreateBlurImage(string name = null, Transform parent = null) {
            var image = CreateOverlayImage(name, parent);
            image.material = Resources.Load<Material>(BlurShaderPath);
            return image;
        }

        public static Image CreateShadowImage(string name = null, Transform parent = null) {
            var image = CreateOverlayImage(name, parent);
            image.sprite = Resources.Load<Sprite>(ShadowImagePath);
            image.type = Image.Type.Sliced;
            image.rectTransform.offsetMax = new Vector2(-ShadowImageOffset, -ShadowImageOffset);
            image.rectTransform.offsetMin = new Vector2(ShadowImageOffset, ShadowImageOffset);
            image.fillCenter = false;
            return image;
        }
        
        public static Image CreateOverlayImage(string name = null, Transform parent = null) {
            var image = CreateGO(name, parent).AddComponent<Image>();
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
            image.raycastTarget = false;
            image.transform.SetAsLastSibling();
            return image;
        }

        public static RenderTexture CreateRendureTexture(int width, int height) {
            return new RenderTexture(width, height, DefaultRenderTextureDepth);
        }

        public static Camera CreateUICamera(string name = null, Transform parent = null) {
            var camera = CreateGO(name, parent).AddComponent<Camera>();
            
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.clearFlags = CameraClearFlags.Color;
            camera.orthographic = true;
            
            return camera;
        }

        private static GameObject CreateGO(string name = null, Transform parent = null) {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);

            // Ignore layouts; we don't want helper UI elements to break automatic layouts
            var layout = gameObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            
            #if UNITY_EDITOR
            if(!UIManager.showUIHelperObjects) {
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            #endif

            return gameObject;
        }

        public static void SetBlurIntensity(this Image blurImage, float intensity) {
            if(Math.Abs(blurImage.material.GetFloat("_Radius") - intensity) < Mathf.Epsilon) {
                return;
            }
            
            blurImage.material.SetFloat("_Radius", intensity);
        }
    }
}