using UnityEngine;

[CreateAssetMenu(menuName = "Puzzles/Sequence Config")]
public class SequencePuzzleConfig : PuzzleConfig
{
    public string[] correctSequence;
    public bool resetOnWrongInput = true;
}
