using System;
using UnityEditor;
using UnityEngine;

namespace RDG.UnityUI {
  
  [InitializeOnLoad] 
  public class UiEditorInit {
    
    static UiEditorInit() {
      EditorApplication.update += Update;
      EditorApplication.hierarchyWindowItemOnGUI += OnGui;
    }
    private static void OnGui(int instanceId, Rect _) {
      var item = EditorUtility.InstanceIDToObject(instanceId);
      if (!item) {
        return;
      }
      var obj = (item as GameObject);
      if (!obj || !obj.TryGetComponent(typeof(UiParentBeh), out var parent)) {
        return;
      }
      
      UiStaticRegister.RegisterParent(parent as UiParentBeh);
    }

    private static void Update() {
      UiStaticRegister.Update();
    }
    
  }
}
