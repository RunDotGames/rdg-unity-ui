
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    namespace RDG.UnityUI {
        
        [InitializeOnLoad] 
        class UiParentStaticRegister {

            private static Coroutine updater;

            private static List<UiThemeSo> themes = new List<UiThemeSo>();
            private static List<UiParentBehEditor> parents = new List<UiParentBehEditor>();
            private static Dictionary<int, string> themeStates = new Dictionary<int, string>();
            
            static UiParentStaticRegister() {
                EditorApplication.update += Update;
            }
            
            static void Update () {
                foreach (var theme in themes) {
                    var hasPriorState = themeStates.TryGetValue(theme.GetInstanceID(), out var priorState);
                    var newState = JsonUtility.ToJson(theme.theme);
                    themeStates[theme.GetInstanceID()] = newState;
                    if (hasPriorState && priorState == newState) {
                        continue;
                    }
                    
                    foreach (var parent in parents) {
                        parent.HandleThemeChange(theme.GetInstanceID());
                    }
                }
            }

            public static void RegisterTheme(UiThemeSo theme) {
                if (themes.Contains(theme)) {
                    return;
                }
                
                themes.Add(theme);
            }

            public static void RegisterParent(UiParentBehEditor parent) {
                if (parents.Contains(parent)) {
                    return;
                }
                
                parents.Add(parent);
            }
        }
        
        [CustomEditor(typeof(UiParentBeh))]
        public class UiParentBehEditor : Editor {
            
            public void Awake() {
                Debug.Log("parent awake");
                var parent = (UiParentBeh)target;
                parent.Apply(HandleDirty, false);
                UiParentStaticRegister.RegisterParent(this);
            }
            
            public override void OnInspectorGUI() {
                DrawDefaultInspector();
                if (!GUILayout.Button("Apply", GUILayout.ExpandWidth(false))) {
                    return;
                }
                var parent = (UiParentBeh)target;
                parent.Apply(HandleDirty, false);
                EditorUtility.SetDirty(parent.gameObject);
            }

            private void HandleDirty(IEnumerable<GameObject> objs) {
                foreach (var obj in objs) {
                    EditorUtility.SetDirty(obj);
                }
            }

            public void HandleThemeChange(int themeId) {
                var parent = (UiParentBeh)target;
                if (themeId != parent.themeSo.GetInstanceID()) {
                    return;
                }
                Debug.Log("triggering from theme change");
                parent.Apply(HandleDirty, false);
                EditorUtility.SetDirty(parent.gameObject);
            }

        }
        
    }