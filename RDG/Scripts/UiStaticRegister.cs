
using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUI {


  public static class UiStaticRegister {
    
    private static List<UiParentBeh> _parents = new List<UiParentBeh>();
    private static Dictionary<int, string>_itemStates = new Dictionary<int, string>();
    private static readonly Dictionary<int, string> ThemeStates = new Dictionary<int, string>();
    
    public static void Update() {
      var validParents = new List<UiParentBeh>();
      foreach (var parent in _parents) {
        if (!parent) {
          continue;
        }
        
        validParents.Add(parent);
      }
      _parents = validParents;

      var updatedItemStates = new Dictionary<int, string>();

      foreach (var parent in _parents) {
        var theme = parent.themeSo;
        var hasPriorState = ThemeStates.TryGetValue(parent.themeSo.GetInstanceID(), out var priorState);
        var newState = JsonUtility.ToJson(theme.theme);
        ThemeStates[parent.themeSo.GetInstanceID()] = newState;
        var hasChange = !hasPriorState || priorState != newState;
        var items = parent.GetComponentsInChildren<UIThemeableItem>();
        foreach (var item in items) {
          var currentItemState = JsonUtility.ToJson(item.GetState());
          updatedItemStates[item.GetInstanceID()] = currentItemState;
          var hasPriorItemState = _itemStates.TryGetValue(item.GetInstanceID(), out var priorItemState);
          hasChange = hasChange || !hasPriorItemState || priorItemState != currentItemState;
        }
        if (!hasChange) {
          continue;
        }
        
        parent.Apply(_=>{}, false);
      }
      _itemStates = updatedItemStates;
      
    }

    public static void RegisterParent(UiParentBeh parent) {
      if (_parents.Contains(parent)) {
        return;
      }

      _parents.Add(parent);
    }
  }
}