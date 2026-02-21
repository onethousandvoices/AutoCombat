using UnityEngine;

namespace AutoCombat.UI
{
    public sealed class ExitButtonView : MonoBehaviour
    {
        public void OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
