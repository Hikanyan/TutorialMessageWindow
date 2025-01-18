using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace HikanyanLaboratory.Script
{


    public class MessageSequencer : MonoBehaviour
    {
        [SerializeField] private MessagePrinter _messagePrinter = default;
        [SerializeField] private string[] _messages = default;
        [SerializeField] private List<EmphasisText> _emphasisTexts = default;
        private int _currentIndex = -1;


        private void Start()
        {
            MoveNext();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_messagePrinter != null && _messagePrinter.IsPrinting)
                {
                    _messagePrinter.Skip();
                }
                else
                {
                    MoveNext();
                }
            }
        }

        /// <summary>
        /// 次のページに進む。
        /// 次のページが存在しない場合は無視する。
        /// </summary>
        private void MoveNext()
        {
            if (_messages is null or { Length: 0 })
            {
                return;
            }

            if (_currentIndex + 1 < _messages.Length)
            {
                _currentIndex++;
               
                _messagePrinter?.ShowMessage(_messages[_currentIndex]);
                _messagePrinter.ApplyEmphasis(_emphasisTexts);
            }
            else
            {
                OnSequenceEnd();
            }
        }

        private void OnSequenceEnd()
        {
            Debug.Log("シーケンス終了");
        }

        public void ResetMessage()
        {
            _currentIndex = -1;
            MoveNext();
        }
    }
}