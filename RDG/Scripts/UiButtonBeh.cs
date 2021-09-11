using System;
using System.Collections.Generic;
using RDG;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Button")]
    [RequireComponent(typeof(UiShapeBeh))]
    [RequireComponent(typeof(UiRipple))]
    public class UiButtonBeh : MonoBehaviour, UIThemeInitItem, IPointerClickHandler {

        public string text;
        public bool isClickDisabled;
        public UIThemeColorType colorType;
        public UiThemeShapeType shapeType;
        
        [SerializeField, HideInInspector]
        private UiShapeBeh shape;

        [SerializeField, HideInInspector]
        private UiTextBeh textBeh;

        [SerializeField, HideInInspector]
        private UiRipple shapeRipple;

        public Action OnClick;
        
        public IEnumerable<GameObject> InitTheme(UiTheme theme) {
            shape = ComponentUtils.GetRequiredComp<UiShapeBeh>(this);
            shape.hideFlags = theme.InspectorHideFlags;
            UiThemeUtil.AddChild(ref textBeh, "Text", transform, theme);
            shape.InitTheme(theme);
            textBeh.InitTheme(theme);
            textBeh.SetFontType(UIThemeFontType.Button);
            textBeh.SetAlignment(TextAnchor.MiddleCenter);
            textBeh.SetValue(text);
            shape.SetShapeType(shapeType);
            shapeRipple = ComponentUtils.GetRequiredComp<UiRipple>(this);
            shapeRipple.InitTheme(theme);
            shapeRipple.SetDisabled(isClickDisabled);
            shapeRipple.hideFlags = theme.InspectorHideFlags;
            shape.hideFlags = theme.InspectorHideFlags;
            
            SetClickDisabled(isClickDisabled);
            return CollectionUtils.Once(gameObject);
        }
     
        public void OnPointerClick(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            OnClick?.Invoke();
        }

        public void SetClickDisabled(bool disabled) {
            isClickDisabled = disabled;
            shape.SetIsOutline(disabled);
            var color = isClickDisabled ?
                colorType :
                UiThemeUtil.ToOnColor[colorType];
            textBeh.SetColor(color);
            shape.SetHasShadow(!disabled);
            shape.SetColorType(colorType);
            shapeRipple.SetDisabled(disabled);
        }
    }
}
