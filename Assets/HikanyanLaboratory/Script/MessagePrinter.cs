using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace HikanyanLaboratory.Script
{
    public class MessagePrinter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textUi = default;
        TextMeshProUGUI _textMeshPro;
        
        [SerializeField] private string _message = "";
        [SerializeField] private float _speed = 1.0F;
        [SerializeField] private float _fadeDuration = 0.3F;
        private float _elapsed = 0; // 文字を表示してからの経過時間
        private float _interval; // 文字毎の待ち時間

        private int _currentIndex = -1; //// 何も指していない場合は -1 とする

        private void Start()
        {
            ShowMessage(_message).Forget();
        }

        /// <summary>
        /// 文字出力中かどうか。
        /// </summary>
        public bool IsPrinting
        {
            get
            {
                if (_textUi is null || _message is null)
                {
                    return false;
                }

                return _currentIndex + 1 < _message.Length;
            }
        }


        /// <summary>
        /// 指定のメッセージを表示する。
        /// 
        /// </summary>
        /// <param name="message">テキストとして表示するメッセージ。</param>
        public async UniTaskVoid ShowMessage(string message)
        {
            if (_textUi is null)
            {
                return;
            }

            _textUi.text = "";
            _message = message;
            _currentIndex = -1;
            _interval = _speed / Mathf.Max(_message.Length, 1);

            await PrintMessageAsync();
        }

        private async UniTask PrintMessageAsync()
        {
            while (_currentIndex + 1 < _message.Length)
            {
                _currentIndex++;
                _textUi.text += _message[_currentIndex];
                await FadeInLastCharacter(_fadeDuration);
                await UniTask.Delay((int)(_interval * 1000));
            }
        }

        /// <summary>
        /// 現在再生中の文字出力を省略する。
        /// </summary>
        public void Skip()
        {
            if (_textUi is null || IsPrinting == false)
            {
                return;
            }

            _currentIndex = _message.Length - 1;
            _textUi.text = _message;
        }

        /// <summary>
        /// 最後に表示された文字のフェードイン。
        /// </summary>
        private async UniTask FadeInLastCharacter(float duration)
        {
            if (_textUi is null || _textUi.text.Length == 0)
            {
                return;
            }

            TMP_TextInfo textInfo = _textUi.textInfo;
            _textUi.ForceMeshUpdate();
            int charIndex = _textUi.text.Length - 1;

            // 最後に追加された文字のマテリアルと頂点情報を取得
            // この文字のアルファ値を変更することでフェードインを実装
            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            var newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                byte alpha = (byte)Mathf.Lerp(0, 255, elapsed / duration);

                for (int i = 0; i < 4; i++)
                {
                    newVertexColors[vertexIndex + i].a = alpha;
                }

                _textUi.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                await UniTask.Yield();
            }

            for (int i = 0; i < 4; i++)
            {
                newVertexColors[vertexIndex + i].a = 255;
            }

            _textUi.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    
        /// <summary>
        /// 特定の文字列を強調表示する（色変更）。
        /// </summary>
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