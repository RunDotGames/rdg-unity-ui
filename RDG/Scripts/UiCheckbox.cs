using System.Collections.Generic;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Checkbox")]
    public class UiCheckbox : MonoBehaviour, UIThemeInitItem, IPointerClickHandler {

        public bool isChecked;
        public string label;
        public bool isClickDisabled;
        public UIThemeColorType labelColor = UIThemeColorType.OnSurface;
        public UIThemeColorType boxColor = UIThemeColorType.Primary;

        public bool IsChecked => isChecked;
        
        [SerializeField, HideInInspector] private UiTextBeh textBeh;
        [SerializeField, HideInInspector] private UiShapeBeh boxShapeBeh;
        [SerializeField, HideInInspector] private UiRipple ripple;
        [SerializeField, HideInInspector] private UiShapeBeh checkShapeBeh;
        private static void PositionBox(UiTheme theme, RectTransform rect) {
            var halfUp = Vector2.up * 0.5f;
            rect.anchorMax = Vector2.right + halfUp;
            rect.anchorMin = Vector2.right + halfUp;
            rect.pivot = Vector2.right + halfUp;
            rect.sizeDelta = Vector2.one * theme.CheckboxSize;
        }
        public IEnumerable<GameObject> InitTheme(UiTheme theme) {
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
            textBeh.SetColor(labelColor);
            textBeh.SetValue(label);
            textBeh.SetFontType(UIThemeFontType.Body);
            PositionBox(theme, boxShapeBeh.GetComponent<RectTransform>());
            boxShapeBeh.SetHasShadow(true);
            boxShapeBeh.SetColorType(boxColor);
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
            boxShapeBeh.SetColorType(boxColor);
            boxShapeBeh.SetShapeType(UiThemeShapeType.RoundedSquare);
            
            SetChecked(isChecked);
            SetClickDisabled(isClickDisabled);
            
            return CollectionUtils.Once(gameObject);
        }

        public void SetChecked(bool isNowChecked) {
            isChecked = isNowChecked;
            if (checkShapeBeh == null) {
                return;
            }
            checkShapeBeh.ShapeImage.gameObject.SetActive(isNowChecked);
        }

        public void SetClickDisabled(bool isNowClickDisabled) {
            isClickDisabled = isNowClickDisabled;
            ripple.SetDisabled(isClickDisabled);
            boxShapeBeh.SetHasShadow(!isClickDisabled);
            boxShapeBeh.SetIsOutline(isClickDisabled);
            checkShapeBeh.SetColorType(isNowClickDisabled ? boxColor : UiThemeUtil.ToOnColor[boxColor]);
        }
        public void OnPointerClick(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            
            SetChecked(!isChecked);
        }
    }
}
