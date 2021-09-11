using System.Collections.Generic;
using System.Linq;
using RDG.UnityUtil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDG.UnityUI {
    
    [AddComponentMenu("RDG/UI/Ripple")]
    [RequireComponent(typeof(UiShapeBeh))]
    public class UiRipple : MonoBehaviour, UIThemeInitItem, IPointerDownHandler, IPointerUpHandler {
        
        public bool isClickDisabled;
        
        private UiShapeBeh shape;
        private Image ripple;
        private UiTheme uiTheme;
        private bool fadeOut;
        private Coroutine rippleRoutine;

        public IEnumerable<GameObject> InitTheme(UiTheme theme) {
            uiTheme = theme;
            shape = ComponentUtils.GetRequiredComp<UiShapeBeh>(this);
            SetDisabled(isClickDisabled);
            return CollectionUtils.Once(gameObject);
        }
        
        public void OnEnable() {
            if (ripple == null) {
                return;
            }
            ripple.gameObject.SetActive(false);
        }

        private IEnumerator<YieldInstruction> OnRipple(float targetSize) {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = uiTheme.ButtonRippleSizeCurve.Evaluate(deltaTime);
                ripple.rectTransform.sizeDelta = Vector2.one * (targetSize * percent);
                if (deltaTime > uiTheme.ButtonRippleSizeCurve.keys.Last().time) {
                    break;
                }
                
                yield return CoroutineUtils.EndOfFrame;
            }
            while (!fadeOut) {
                yield return CoroutineUtils.EndOfFrame;
            }
            deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = uiTheme.ButtonRippleFadeCurve.Evaluate(deltaTime);
                
                var color = ripple.color;
                color.a = percent;
                ripple.color = color;
                if (deltaTime > uiTheme.ButtonRippleFadeCurve.keys.Last().time) {
                    break;
                }
                yield return CoroutineUtils.EndOfFrame;
            }
            ripple.gameObject.SetActive(false);
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            fadeOut = false;
            var shapeImage = shape.ShapeImage;
            UiThemeUtil.AddChild(ref ripple, "Ripple", shapeImage.transform, uiTheme);
            ripple.transform.SetSiblingIndex(0);
            ripple.gameObject.SetActive(true);
            var rippleRect = ripple.rectTransform;
            var shapeRect = shapeImage.rectTransform;
            ripple.sprite = uiTheme.GetShape(UiThemeShapeType.Circle).sprite;
            ripple.preserveAspect = true;
            ripple.type = Image.Type.Simple;
            ripple.color = uiTheme.GetColor(UiThemeUtil.ToLightColor[shape.colorType]).color;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                shapeImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint
            )) {
                return;
            }
            rippleRect.anchorMax = Vector2.one  * .5f;
            rippleRect.anchorMin = Vector2.one  * .5f;
            rippleRect.anchoredPosition = localPoint;
            var shapeSize = shapeRect.rect.size;
            var shapeMid = shapeSize * 0.5f;
            var size = Mathf.Max(
                shapeMid.x + Mathf.Abs(localPoint.x),
                shapeMid.y + Mathf.Abs(localPoint.y)
            ) * 2.5f;
            
            if (rippleRoutine != null) {
                StopCoroutine(rippleRoutine);
            }
            rippleRoutine = StartCoroutine(OnRipple(size));
        }
        public void OnPointerUp(PointerEventData eventData) {
            fadeOut = true;
        }

        public void SetDisabled(bool disabled) {
            isClickDisabled = disabled;
        }
    }
}
