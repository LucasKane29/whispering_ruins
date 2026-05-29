using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI _dialogueUI;
    public static DialogueManager Instance { get; private set; }
    private DialogueSO _currentDialogue;
    private int _currentLineIndex;

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public bool IsActive {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        if (IsActive)
            return;
        if(dialogue == null || dialogue.Lines == null || dialogue.Lines.Count == 0) return;
        _currentDialogue = dialogue;
        _currentLineIndex = 0;
        IsActive = true;
        _dialogueUI.Show();
        ShowCurrentLine();
        OnDialogueStarted?.Invoke();
    }

    public void NextLine()
    {         
        if (!IsActive)
            return;
        _currentLineIndex++;
        MakeDialogueDesision();
    }

    public void Skip()
    {
        if (!IsActive)
            return;
        if(_dialogueUI.IsTyping)
        {
            _dialogueUI.SkipTyping();
            return;
        }

        _currentLineIndex++;
        MakeDialogueDesision();
    }

    private void ShowCurrentLine()
    {
        _dialogueUI.DisplayLine(_currentDialogue.Lines[_currentLineIndex]);
    }

    private void EndDialogue()
    {
        IsActive = false;
        _dialogueUI.Hide();
        OnDialogueEnded?.Invoke();
        _currentDialogue = null;
    }

    private void MakeDialogueDesision()
    {
        if (_currentLineIndex >= _currentDialogue.Lines.Count)
            EndDialogue();
        else
            ShowCurrentLine();
    }
}
