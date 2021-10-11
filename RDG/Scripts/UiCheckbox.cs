using System;
using System.Collections.Generic;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUI {

    
    [AddComponentMenu("RDG/UI/Checkbox")]
    public class UiCheckbox: UIThemeableItem, IPointerClickHandler {
        
        [Serializable]
        public class State {
            public bool isChecked;
            public string label;
            public bool isClickDisabled;
            public UIThemeColorType labelColor = UIThemeColorType.OnSurface;
            public UIThemeColorType boxColor = UIThemeColorType.Primary;
        }
        
        public bool IsChecked => state.isChecked;
        
        [SerializeField, HideInInspector] private UiTextBeh textBeh;
        [SerializeField, HideInInspector] private UiShapeBeh boxShapeBeh;
        [SerializeField, HideInInspector] private UiRipple ripple;
        [SerializeField, HideInInspector] private UiShapeBeh checkShapeBeh;
        [SerializeField] private State state;
        private static void PositionBox(UiTheme theme, RectTransform rect) {
            var halfUp = Vector2.up * 0.5f;
            rect.anchorMax = Vector2.right + halfUp;
            rect.anchorMin = Vector2.right + halfUp;
            rect.pivot = Vector2.right + halfUp;
            rect.sizeDelta = Vector2.one * theme.CheckboxSize;
        }
        public override IEnumerable<GameObject> InitTheme(UiTheme theme) {
            UiThemeUtil.AddChild(ref textBeh, "Label", transform, theme);
            if (UiThemeUtil.AddChild(ref boxShapeBeh, "Box", transform, theme)) {
                boxShapeBeh.gameObject.AddComponent<UiRipple>();
            }
            UiThemeUtil.AddChild(ref checkShapeBeh, "Check", boxShapeBeh.transform, theme);
            ripple = boxShapeBeh.gameObject.GetComponent<UiRipple>();
            textBeh.InitTheme(theme);
            boxShapeBeh.InitTheme(theme);
            checkShapeBeh.InitTheme(theme);
            ripple.InitTheme(theme);
            
            textBeh.SetAlignment(TextAnchor.MiddleRight);
            textBeh.SetColor(state.labelColor);
            textBeh.SetValue(state.label);
            textBeh.SetFontType(UIThemeFontType.Body);
            PositionBox(theme, boxShapeBeh.GetComponent<RectTransform>());
            boxShapeBeh.SetHasShadow(true);
            boxShapeBeh.SetColorType(state.boxColor);
            boxShapeBeh.SetShapeType(UiThemeShapeType.RoundedSquare);
            boxShapeBeh.SetIsOutline(false);
            checkShapeBeh.SetHasShadow(false);
            var checkRect = checkShapeBeh.GetComponent<RectTransform>();
            checkRect.offsetMax = Vector2.one * -2.0f;
            checkRect.offsetMin = Vector2.one * 2.0f;
            checkShapeBeh.SetShapeType(UiThemeShapeType.Check);
            checkShapeBeh.SetIsOutline(false);
            var textRect = textBeh.GetComponent<RectTransform>();
            textRect.offsetMax = Vector2.right * (-theme.CheckboxSize - 5);
            boxShapeBeh.SetColorType(state.boxColor);
            boxShapeBeh.SetShapeType(UiThemeShapeType.RoundedSquare);
            
            SetChecked(state.isChecked);
            SetClickDisabled(state.isClickDisabled);
            
            return CollectionUtils.Once(gameObject);
        }
        public override object GetState() {
            return state;
        }

        public void SetChecked(bool isNowChecked) {
            state.isChecked = isNowChecked;
            if (checkShapeBeh == null) {
                return;
            }
            checkShapeBeh.ShapeImage.gameObject.SetActive(isNowChecked);
        }

        public void SetClickDisabled(bool isNowClickDisabled) {
            state.isClickDisabled = isNowClickDisabled;
            ripple.SetDisabled(isNowClickDisabled);
            boxShapeBeh.SetHasShadow(!isNowClickDisabled);
            boxShapeBeh.SetIsOutline(isNowClickDisabled);
            checkShapeBeh.SetColorType(isNowClickDisabled ? state.boxColor : UiThemeUtil.ToOnColor[state.boxColor]);
        }
        public void OnPointerClick(PointerEventData eventData) {
            if (state.isClickDisabled) {
                return;
            }
            
            SetChecked(!state.isChecked);
        }
    }
}
