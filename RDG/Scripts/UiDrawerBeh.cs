using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using RDG.UnityUtil;

namespace RDG.UnityUI {
    
    // Not Fully Updated For Realtime / Auto Generation
    
    [AddComponentMenu("RDG/UI/Drawer")]
    
    public class UiDrawerBeh: UIThemeableItem {
        
        private static readonly Quaternion Upward = Quaternion.Euler(new Vector3(0, 0 , -90));
        private static readonly Quaternion Downward = Quaternion.Euler(new Vector3(0, 0 , 90));
        
        public LayoutElement toggleLayout;
        public UiButtonBeh toggle;
        public bool isCollapsed;
        
        [HideInInspector, SerializeField]
        private RectTransform drawerRect;
        
        private UiTheme theme;
        private bool isTweening;
        private float targetHeight;
        private float toggleHeight;
        
        public override IEnumerable<GameObject> InitTheme(UiTheme aTheme) {
            theme = aTheme;
            drawerRect = GetComponent<RectTransform>();
            toggleHeight = toggleLayout.preferredHeight;
            targetHeight = drawerRect.rect.height - toggleHeight;
            UpdateHeight(1.0f);
            return CollectionUtils.Once(gameObject);
        }
        
        public override object GetState() {
            return null;
        }

        public void Start() {
            toggle.OnClick += HandleToggle;
        }

        public void OnDestroy() {
            toggle.OnClick -= HandleToggle;
        }

        private void HandleToggle() {
            if (isTweening) {
                return;
            }
            isCollapsed = !isCollapsed;
            isTweening = true;
            StartCoroutine(RunToggle());
        }


        private void UpdateHeight(float percent) {
            var deltaHeight = percent * targetHeight;
            if (isCollapsed) {
                deltaHeight = targetHeight - deltaHeight;
            }
            drawerRect.sizeDelta = new Vector2(drawerRect.sizeDelta.x, deltaHeight + toggleHeight);
            toggle.transform.rotation = Quaternion.Lerp(isCollapsed ? Upward : Downward, isCollapsed ? Downward : Upward, percent);
        }

        private IEnumerator<YieldInstruction> RunToggle() {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = theme.DrawerToggleCurve.Evaluate(deltaTime);
                UpdateHeight(percent);
                if (deltaTime > theme.DrawerToggleCurve.keys.Last().time) {
                    isTweening = false;
                    break;
                }
                
                yield return CoroutineUtils.EndOfFrame;
            }
        }
    }
}
