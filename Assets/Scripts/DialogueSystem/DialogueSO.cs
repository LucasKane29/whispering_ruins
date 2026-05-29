using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [SerializeField] private List<DialogueLine> _lines;
    public List<DialogueLine> Lines => _lines;
}

[Serializable]
public class DialogueLine
{
    [SerializeField] private string _speakerName;
    [TextArea(3, 10)]
    [SerializeField] private string _dialogueText;
    public string SpeakerName => _speakerName;
    public string DialogueText => _dialogueText;
}