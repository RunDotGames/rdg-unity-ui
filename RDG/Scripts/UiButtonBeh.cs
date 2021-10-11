using System;
using System.Collections.Generic;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUI {

   
    
    [AddComponentMenu("RDG/UI/Button")]
    [RequireComponent(typeof(UiShapeBeh))]
    [RequireComponent(typeof(UiRipple))]
    public class UiButtonBeh : UIThemeableItem, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, UIThemeCursorInitItem {
        
        [Serializable]
        internal class State {
            public string text = "Button";
            public bool isClickDisabled = false;
            public UIThemeColorType color  = UIThemeColorType.Primary;
            public UiThemeShapeType shape = UiThemeShapeType.RoundedSquare;
            public CursorState cursor = CursorState.Interact;
        }

        [SerializeField, HideInInspector]
        private UiShapeBeh shape;

        [SerializeField, HideInInspector]
        private UiTextBeh textBeh;

        [SerializeField, HideInInspector]
        private UiRipple shapeRipple;

        [SerializeField]
        private State state = new State();
        
        
        public Action OnClick;
        private CursorSo cursors;
        private Action cursorRelease;
        private bool isHovering;

        public override IEnumerable<GameObject> InitTheme(UiTheme theme) {
            shape = ComponentUtils.GetRequiredComp<UiShapeBeh>(this);
            shape.hideFlags = theme.InspectorHideFlags;
            shape.InitTheme(theme);
            SetShapeType(state.shape);
            UiThemeUtil.AddChild(ref textBeh, "Text", transform, theme);
            textBeh.InitTheme(theme);
            textBeh.SetFontType(UIThemeFontType.Button);
            textBeh.SetAlignment(TextAnchor.MiddleCenter);
            SetText(state.text);
            shapeRipple = ComponentUtils.GetRequiredComp<UiRipple>(this);
            shapeRipple.InitTheme(theme);
            shapeRipple.hideFlags = theme.InspectorHideFlags;
            shape.hideFlags = theme.InspectorHideFlags;
            SetClickDisabled(state.isClickDisabled);
            return CollectionUtils.Once(gameObject);
        }

        public void OnDisable() {
            cursorRelease?.Invoke();
            cursorRelease = null;
        }


        public void InitCursor(CursorSo aCursors) {
            cursors = aCursors;
        }
        
        public override object GetState() {
            return state;
        }

        public void SetClickDisabled(bool disabled) {
            state.isClickDisabled = disabled;
            shape.SetIsOutline(disabled);
            shape.SetHasShadow(!disabled);
            shapeRipple.SetDisabled(disabled);
            HandleColorChange();
            if (disabled) {
                cursorRelease?.Invoke();
            }
            if(isHovering && !disabled){
                cursorRelease = cursors.Push(state.cursor);
            }
        }

        public void SetText(string text) {
            state.text = text;
            if (!textBeh) {
                return;
            }
            textBeh.SetValue(state.text);
        }
        public void SetShapeType(UiThemeShapeType shapeType) {
            state.shape = shapeType;
            if (!shape) {
                return;
            }
            shape.SetShapeType(shapeType);
        }
        public void SetColorType(UIThemeColorType colorType) {
            state.color = colorType;
            HandleColorChange();
        }
        
        
        private void HandleColorChange() {
            shape.SetColorType(state.color);
            var color = state.isClickDisabled ?
                state.color :
                UiThemeUtil.ToOnColor[state.color];
            textBeh.SetColor(color);
        }
        
        
        public void OnPointerEnter(PointerEventData eventData) {
            isHovering = true;
            if (state.isClickDisabled || cursors == null) {
                return;
            }
            
            cursorRelease = cursors.Push(state.cursor);

        }
        public void OnPointerExit(PointerEventData eventData) {
            isHovering = false;
            cursorRelease?.Invoke();
            cursorRelease = null;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            if (state.isClickDisabled) {
                return;
            }
            OnClick?.Invoke();
        }
    }
}
