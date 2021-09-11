using System;
using System.Threading.Tasks;
using UnityEngine;

namespace RDG.UnityUI {

    public interface UIModal {
        void OpenModal();
        Task CloseModal(bool isGraceful);

        void AddCloseHandler(Action<bool> onClose);
        void RemoveCloseHandler(Action<bool> onClose);
        void AddOpenHandler(Action onOpen);
        void RemoveOpenHandler(Action onOpen);
    }

    [CreateAssetMenu(menuName = "RDG/UI/Modals")]
    public class UiModalsSo : ScriptableObject {
        private UIModal visibleModal;
        private bool isModalInTransition;

        public void ResetState() {
            visibleModal = null;
            isModalInTransition = false;
        }

        public bool ShowModal(UIModal modal) {
            if (isModalInTransition) {
                return false;
            }

            if (visibleModal == null) {
                OpenAsVisibleModal(modal);
                return true;
            }
            isModalInTransition = true;
            visibleModal.RemoveCloseHandler(HandleVisibleClose);
            visibleModal.CloseModal(false).ContinueWith((task) => {
                OpenAsVisibleModal(modal);
                isModalInTransition = false;
            }, TaskContinuationOptions.ExecuteSynchronously);
            return true;
        }

        private void OpenAsVisibleModal(UIModal modal) {
            visibleModal = modal;
            visibleModal.AddCloseHandler(HandleVisibleClose);
            modal.OpenModal();
        }

        private void HandleVisibleClose(bool isGraceful) {
            visibleModal.RemoveCloseHandler(HandleVisibleClose);
            visibleModal = null;
        }
    }
}

