using System;
using System.Collections.Generic;
using RDG.UnityUtil;
using UnityEngine;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Parent")]
    public class UiParentBeh : MonoBehaviour {
        
        public UiThemeSo themeSo;
        public CursorSo cursors;

        public void Start() {
            Apply(objs => { }, true);
        }
        
        public void Apply(Action<IEnumerable<GameObject>> handleDirty, bool runPostInits) {
            var inits = GetComponentsInChildren<UIThemeableItem>();
            var theme = themeSo.NewTheme();
            foreach (var init in inits) {
                handleDirty(init.InitTheme(theme));
            }
            if (!runPostInits) {
                return;
            }
            var postInits = GetComponentsInChildren<UIThemePostInitItem>();
            foreach (var postInit in postInits) {
                handleDirty(postInit.PostInitTheme(theme));
            }

            var cursorInits = GetComponentsInChildren<UIThemeCursorInitItem>();
            foreach (var cursorInit in cursorInits) {
                cursorInit.InitCursor(cursors);
            }
        }

        public static void NotifyChange(GameObject from) {
            var fromParent = from.transform.parent;
            while (fromParent != null) {
                var found = fromParent.TryGetComponent(typeof(UiParentBeh), out var parentRef);
                if (!found) {
                    fromParent = fromParent.transform.parent;
                    continue;
                }
                
                (parentRef as UiParentBeh)?.Apply((obj) => {}, false);
                return;
            }

            throw new Exception("UI Component unable to locate parent root from " + from.name);
        }

    }
}
