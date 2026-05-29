using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private GameObject _speakerNamePanel;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _continueIndicator;

    [Header("Settings")]
    [SerializeField] private float _charactersPerSecond = 30f;

    private Coroutine _typingCoroutine;
    private Coroutine _showingCoroutine;
    private string _currentDialogueText;

    public bool IsTyping {  get; private set; }

    public void Show()
    {
        if(_showingCoroutine != null)
        {
            StopCoroutine(_showingCoroutine);
        }
        _panel.SetActive(true);
    }

    public void Hide()
    {
        _panel.SetActive(false);
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        IsTyping = false;
    }

    public void DisplayLine(DialogueLine dialogueLine)
    {
        if(string.IsNullOrEmpty(dialogueLine.SpeakerName))
        {
            _speakerNamePanel.SetActive(false);
        }
        else
        {
            _speakerNamePanel.SetActive(true);
            _speakerNameText.text = dialogueLine.SpeakerName;
        }
        
        _currentDialogueText = dialogueLine.DialogueText;
        _dialogueText.text = string.Empty;
        _continueIndicator.SetActive(false);
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeText(_currentDialogueText));
    }

    private IEnumerator TypeText(string text)
    {
        IsTyping = true;
        _continueIndicator.SetActive(false);
        _dialogueText.text = string.Empty;

        StringBuilder displayedText = new StringBuilder();
        float delay = 1f / _charactersPerSecond;
        foreach (char c in text)
        {
            displayedText.Append(c);
            _dialogueText.text = displayedText.ToString();
            if(c == ',' || c == ';')
            {
                yield return new WaitForSeconds(delay * 4);
            }else if (c == '.' || c == '!' || c == '?')
            {
                yield return new WaitForSeconds(delay * 8);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

        }
        IsTyping = false;
        _continueIndicator.SetActive(true);
        _typingCoroutine = null;
    }

    public void SkipTyping()
    {
        if (!IsTyping)
            return;
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _dialogueText.text = _currentDialogueText;
        IsTyping = false;
        _continueIndicator.SetActive(true);
        _typingCoroutine = null;
    }

    private IEnumerator ShowPanel(float duration, float targetAlpha)
    {
        _panel.GetComponent<CanvasGroup>().alpha = targetAlpha;
        float elapsed = 0f;
        if (targetAlpha > 0f)
        {
            elapsed = Time.deltaTime;
        }
        else
        {
            elapsed = Time.deltaTime;
        }
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _panel.GetComponent<CanvasGroup>().alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
    }


}
