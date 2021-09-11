using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Parent")]
    public class UiParentBeh : MonoBehaviour {
        
        public UiThemeSo themeSo;

        public void Start() {
            Apply(objs => { }, true);
        }
        
        public void Apply(Action<IEnumerable<GameObject>> handleDirty, bool runPostInits) {
            var inits = GetComponentsInChildren<UIThemeInitItem>();
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
        }

    }
}
