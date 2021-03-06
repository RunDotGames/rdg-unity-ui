using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.UI;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Modal")]
    [RequireComponent(typeof(UiShapeBeh))]
    [RequireComponent(typeof(UiMotionBeh))]
    public class UiModalBeh: UIThemeableItem, UIThemePostInitItem, UIModal {

        [Serializable]
        public class State {
            public Vector2 size;
            public Vector2 position;
            public bool isVisible;    
        } 
        
        public event Action OnOpen;
        public event Action<bool> OnClose;
        
        public UiModalsSo modals;
        
        
        private UiTheme theme;
        
        [HideInInspector, SerializeField] private UiMotionBeh openMotion;
        [HideInInspector, SerializeField] private UiButtonBeh closeButton;
        [HideInInspector, SerializeField] private UiShapeBeh windowShape;
        [HideInInspector, SerializeField] private VerticalLayoutGroup layoutGroup;
        [HideInInspector, SerializeField] private LayoutElement closeButtonRowLayout;
        [HideInInspector, SerializeField] private LayoutElement contentRowLayout;
        [SerializeField]
        private State state = new State();
        
        
        public override IEnumerable<GameObject> InitTheme(UiTheme aTheme) {
            theme = aTheme;
            if (modals != null) {
                modals.ResetState();
            }
            windowShape = ComponentUtils.GetRequiredComp<UiShapeBeh>(this);
            windowShape.hideFlags = theme.InspectorHideFlags;
            openMotion = ComponentUtils.GetRequiredComp<UiMotionBeh>(this);
            openMotion.hideFlags = theme.InspectorHideFlags;
            openMotion.offset = Vector2.right * (state.size.x + state.position.x);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;;
            rectTransform.pivot = Vector2.one * 0.5f;;
            rectTransform.sizeDelta = state.size;
                
            
            if (UiThemeUtil.AddChild(ref layoutGroup, "Layout", transform, theme)) {
                layoutGroup.padding = new RectOffset(12, 12, 12, 12);
                layoutGroup.spacing = 6;    
            }
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.gameObject.hideFlags = HideFlags.None;
            
            UiThemeUtil.AddChild(ref closeButtonRowLayout, "Modal Row", layoutGroup.transform, theme);
            closeButtonRowLayout.preferredWidth = 25;
            closeButtonRowLayout.preferredHeight = 25;
            closeButtonRowLayout.minHeight = 25;
            closeButtonRowLayout.minWidth = 25;
            closeButtonRowLayout.flexibleWidth = 1.0f;
            closeButtonRowLayout.flexibleHeight = 0;
            closeButtonRowLayout.gameObject.hideFlags = HideFlags.None;
            
            UiThemeUtil.AddChild(ref contentRowLayout, "Content Row", layoutGroup.transform, theme);
            contentRowLayout.flexibleWidth = 1.0f;
            contentRowLayout.flexibleHeight = 1.0f;
            contentRowLayout.gameObject.hideFlags = HideFlags.None;
            
            closeButtonRowLayout.transform.SetSiblingIndex(0);
            contentRowLayout.transform.SetSiblingIndex(1);
             
            UiThemeUtil.AddChild(ref closeButton, "Close Button", closeButtonRowLayout.transform, theme);
            closeButton.InitTheme(theme);
            closeButton.SetShapeType(UiThemeShapeType.Circle);
            closeButton.SetColorType(UIThemeColorType.Secondary);
            closeButton.SetText("x");
            
            closeButton.gameObject.hideFlags = theme.HierarchyHideFlags;
            var closeTransform = closeButton.GetComponent<RectTransform>();
            closeTransform.anchorMax = Vector2.one;
            closeTransform.anchorMin = Vector2.right;
            closeTransform.pivot = Vector2.right + Vector2.up * .5f;
            closeTransform.sizeDelta = Vector2.right * 25;

            rectTransform.anchoredPosition = state.position;

            return CollectionUtils.Once(gameObject);
        }
        public IEnumerable<GameObject> PostInitTheme(UiTheme _) {
            openMotion.EvalEndPos();
            gameObject.SetActive(state.isVisible);

            closeButton.OnClick = HandleCloseClick;
            
            return CollectionUtils.Once(gameObject);
        }
        private void HandleCloseClick() {
            CloseModal(false);
        }

        public bool OpenModalAsSingle() {
            return modals.ShowModal(this);
        }
        
        public void OpenModal() {
            gameObject.SetActive(true);
            openMotion.PlayOut();
            state.isVisible = true;
            OnOpen?.Invoke();
        }
        public Task CloseModal(bool isGraceful) {
            if (state.isVisible) {
                return Task.CompletedTask;
            }
            
            state.isVisible = false;
            OnClose?.Invoke(isGraceful);
            return openMotion.PlayIn().ContinueWith(result => {
                gameObject.SetActive(false);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
        
        public void AddCloseHandler(Action<bool> onClose) {
            OnClose += onClose;
        }
        public void RemoveCloseHandler(Action<bool> onClose) {
            OnClose -= onClose;
        }
        public void AddOpenHandler(Action onOpen) {
            OnOpen += onOpen;
        }
        public void RemoveOpenHandler(Action onOpen) {
            OnOpen -= onOpen;
        }

        public override object GetState() {
            return state;
        }
        
    }
}
