using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Position")]
    public class UiPositionBeh: MonoBehaviour {

        public Vector2 offset;
        
        public void RePosition() {
            GetComponent<RectTransform>().anchoredPosition = offset;
        }
    }
}
