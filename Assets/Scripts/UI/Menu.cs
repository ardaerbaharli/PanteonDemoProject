using UnityEngine;

namespace UI
{
    public abstract class Menu : MonoBehaviour
    {
        private static readonly int ShowHash = Animator.StringToHash("Show");
        private static readonly int HideHash = Animator.StringToHash("Hide");
        public bool IsShown { get; set; }
        public Animator Animator { get; set; }

        public void Toggle()
        {
            Toggle(!IsShown);
        }

        public void Toggle(bool value)
        {
            if (value == IsShown) return;

            if (value)
                Show();
            else
                Hide();

            IsShown = value;
        }

        public virtual void Show()
        {
            if (IsShown) return;
            IsShown = true;
            Animator.SetTrigger(ShowHash);
        }

        public virtual void Hide()
        {
            if (!IsShown) return;
            IsShown = false;
            Animator.SetTrigger(HideHash);
        }
    }
}