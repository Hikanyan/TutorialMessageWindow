using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace HikanyanLaboratory.Script
{
    public class MessagePrinter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textUi = default;
        [SerializeField] private string _message = "";
        [SerializeField] private float _speed = 1.0F;
        [SerializeField] private float _fadeDuration = 0.3F;
        
        private float _interval;
        private int _currentIndex = -1;
        private bool _isPrinting = false;

        private void Start()
        {
            ShowMessage(_message).Forget();
        }

        public bool IsPrinting => _isPrinting;

        public async UniTaskVoid ShowMessage(string message)
        {
            if (_textUi == null) return;

            _textUi.text = "";
            _message = message;
            _currentIndex = -1;
            _interval = _speed / Mathf.Max(_message.Length, 1);

            _isPrinting = true;

            await PrintMessageAsync();

            _isPrinting = false;
        }

        private async UniTask PrintMessageAsync()
        {
            while (_currentIndex + 1 < _message.Length)
            {
                _currentIndex++;
                _textUi.text += _message[_currentIndex];
                await UniTask.Delay((int)(_interval * 1000));
            }
        }

        public void Skip()
        {
            if (_textUi == null || !_isPrinting) return;

            _currentIndex = _message.Length - 1;
            _textUi.text = _message;
            _isPrinting = false;
        }

        public void ApplyEmphasis(List<EmphasisText> emphasisTexts)
        {
            foreach (var emphasis in emphasisTexts)
            {
                string colorHex = ColorUtility.ToHtmlStringRGBA(emphasis._color);
                _message = _message.Replace(emphasis._targetText, $"<color=#{colorHex}>{emphasis._targetText}</color>");
            }
        }
    }
}