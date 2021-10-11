using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RDG.UnityUI {

    [AddComponentMenu("RDG/UI/Shape")]
    public class UiShapeBeh: UIThemeableItem {

        [Serializable]
        public class State {
            public UiThemeShapeType shapeType = UiThemeShapeType.Square;
            public UIThemeColorType colorType = UIThemeColorType.Surface;
            public bool hasShadow;
            public bool isOutline;
        }
        
        [HideInInspector, SerializeField]
        private Image shapeImage;
        [HideInInspector, SerializeField]
        private Image shadowImage;
        [SerializeField]
        private State state = new State();
        
        public Image ShapeImage => shapeImage;
        public UIThemeColorType ColorType => state.colorType;
        private UiTheme theme;
        
        public override IEnumerable<GameObject> InitTheme(UiTheme aTheme) {
            theme = aTheme;
            var initObjs = new List<GameObject>{
                gameObject
            };
            SetHasShadow(state.hasShadow);
            if (UiThemeUtil.AddChild(ref shapeImage, "Shape", transform, theme)) {
                shapeImage.gameObject.AddComponent<Mask>();
            }
            SetShapeType(state.shapeType);
            SetColorType(state.colorType);
            initObjs.Add(shapeImage.gameObject);
            var mask = shapeImage.gameObject.GetComponent<Mask>();
            mask.showMaskGraphic = true;
            return initObjs;
        }

        public void SetIsOutline(bool isNowOutline) {
            state.isOutline = isNowOutline;
            var shape = theme.GetShape(state.shapeType);
            shapeImage.sprite = state.isOutline ? shape.outline : shape.sprite;
            
        }
        
        public void SetHasShadow(bool nowHasShadow) {
            state.hasShadow = nowHasShadow;
            var shape = theme.GetShape(state.shapeType);
            if (!state.hasShadow && shadowImage != null) {
                DestroyImmediate(shadowImage.gameObject);
                shadowImage = null;
            }

            if (state.hasShadow) {
                UiThemeUtil.AddChild(ref shadowImage, "Shadow", transform, theme);
                var shadowTransform = shadowImage.gameObject.GetComponent<RectTransform>();
                shadowTransform.sizeDelta = Vector2.one * (shape.shadowSize * 2.0f);
                shadowTransform.anchoredPosition = Vector2.zero;
                shadowImage.sprite = shape.shadow;
                shadowImage.type = shape.imageType;
                shadowImage.preserveAspect = shape.preserveAspect;
                shadowImage.raycastTarget = false;
                shadowTransform.SetSiblingIndex(0);
            }
        }
        public void SetColorType(UIThemeColorType aColorType) {
            state.colorType = aColorType;
            shapeImage.color = theme.GetColor(state.colorType).color;
        }
        public void SetShapeType(UiThemeShapeType aShapeType) {
            state.shapeType = aShapeType;
            SetHasShadow(state.hasShadow); //update shadow shape
            var shape = theme.GetShape(state.shapeType);
            shapeImage.sprite = state.isOutline ? shape.outline : shape.sprite;
            shapeImage.preserveAspect = shape.preserveAspect;
            shapeImage.type = shape.imageType;
            shapeImage.transform.SetSiblingIndex(state.hasShadow ? 1 : 0);
        }
        
        public override object GetState() {
            return state;
        }
    }
}
