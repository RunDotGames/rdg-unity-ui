using System.Collections.Generic;
using System.Threading.Tasks;
using RDG.UnityUtil;
using UnityEngine;
using System.Linq;


namespace RDG.UnityUI {

    [AddComponentMenu("RDG/UI/Motion")]
    public class UiMotionBeh: UIThemeableItem {

        public Vector2 offset;

        private UiTheme theme;
        private Vector2 endPos;
        private RectTransform rectTransform;
        private Coroutine motionRoutine;
        private TaskCompletionSource<bool> taskSource;

        public override IEnumerable<GameObject> InitTheme(UiTheme aTheme) {
            theme = aTheme;
            var obj = gameObject;
            rectTransform = obj.GetComponent<RectTransform>();
            EvalEndPos();
            return CollectionUtils.Once(obj);
        }

        public void EvalEndPos() {
            endPos = rectTransform.anchoredPosition;
        }

        private void HaltMotion() {
            if (motionRoutine != null) {
                taskSource.SetCanceled();
                StopCoroutine(motionRoutine);
            }
        }

        public Task<bool> PlayIn() {
            HaltMotion();
            taskSource = new TaskCompletionSource<bool>();
            motionRoutine = StartCoroutine(RunMotion(true));
            return taskSource.Task;
        }

        public Task<bool> PlayOut() {
            HaltMotion();
            taskSource = new TaskCompletionSource<bool>();
            motionRoutine = StartCoroutine(RunMotion(false));
            return taskSource.Task;
        }
        private IEnumerator<YieldInstruction> RunMotion(bool isForward) {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = theme.MotionCurve.Evaluate(deltaTime);
                if (!isForward) {
                    percent = 1.0f - percent;
                }
                rectTransform.anchoredPosition = endPos + offset * percent;
                if (deltaTime > theme.MotionCurve.keys.Last().time) {
                    motionRoutine = null;
                    taskSource.SetResult(true);
                    break;
                }
                yield return CoroutineUtils.EndOfFrame;
            }
        }
        public override object GetState() {
            return null;
        }
    }
}
