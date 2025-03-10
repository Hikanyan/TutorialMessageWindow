using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _textUi = default;

   // [Header("Message Box")] [SerializeField]
   // private Timeline _timeline = default;


    [Header("StoryBackground")]
    [SerializeField] Image _storyBackgroundImage;


    // イベント表示中フラグ
    bool isEventing = false;

    // イベント完了待機時の画面タップフラグ
    bool isScreenClickedOnEventEnd = false;
}