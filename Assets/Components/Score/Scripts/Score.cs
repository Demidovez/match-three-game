using TMPro;
using UnityEngine;

namespace ScoreSpace
{
    public sealed class Score : MonoBehaviour
    {
        public static Score Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _text;
        private int _value;
        
        public int Value
        {
            get => _value;
            set
            {
                if(_value == value) return;

                _value = value;
                _text.SetText($"Score = {_value}");
            }
        }
        
        private void Awake()
        {
            Instance = this;
        }
    }
}
