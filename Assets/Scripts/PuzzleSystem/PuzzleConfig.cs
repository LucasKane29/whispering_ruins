using UnityEngine;

[CreateAssetMenu(menuName = "Puzzles/Puzzle Config")]
public class PuzzleConfig : ScriptableObject
{
    public string puzzleName;
    [TextArea] public string description;
    public float timeLimit = 0f;
    public int maxAttempts = 0;
    public AudioClip solveSound;
    public AudioClip failSound;
}
