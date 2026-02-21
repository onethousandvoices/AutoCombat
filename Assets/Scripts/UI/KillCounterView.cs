using TMPro;
using UnityEngine;

namespace AutoCombat.UI
{
    public sealed class KillCounterView : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void SetKillCount(int count)
        {
            _text.SetText("Kills: {0}", count);
        }
    }
}
