using UnityEngine;

public class ButtonSequencePuzzle : PuzzleBase
{
    private SequencePuzzleConfig SequenceConfig => config as SequencePuzzleConfig;
    private int _currentIndex;
    private bool _pendingReset;

    public override void OnElementInteracted(PuzzleElement element, object data)
    {
        if (State == PuzzleState.Solved) return;

        if (_pendingReset)
        {
            _pendingReset = false;
            _currentIndex = 0;
            base.ResetPuzzle();
        }

        if (State != PuzzleState.Active) StartPuzzle();

        var sequence = SequenceConfig.correctSequence;

        if (element.ElementId == sequence[_currentIndex])
        {
            _currentIndex++;
            OnProgressChanged?.Invoke((float)_currentIndex / sequence.Length);
            if (_currentIndex >= sequence.Length)
                SolvePuzzle();
        }
        else if (SequenceConfig.resetOnWrongInput)
        {
            if (SequenceConfig.failSound != null)
                AudioSource.PlayClipAtPoint(SequenceConfig.failSound, transform.position);
            OnPuzzleFailed?.Invoke();
            _pendingReset = true;
        }
    }

    public override void ResetPuzzle()
    {
        _currentIndex = 0;
        _pendingReset = false;
        base.ResetPuzzle();
    }
}
