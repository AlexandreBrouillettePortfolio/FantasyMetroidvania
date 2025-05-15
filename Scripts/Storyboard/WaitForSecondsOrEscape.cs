using UnityEngine;

public class WaitForSecondsOrEscape : CustomYieldInstruction
{
    private readonly float _seconds;
    private readonly float _startTime;

    public WaitForSecondsOrEscape(float seconds)
    {
        _seconds = seconds;
        _startTime = Time.time;
    }

    public override bool keepWaiting
    {
        get
        {
            // Continuer d'attendre sauf si la durée est écoulée ou si l'utilisateur appuie sur Escape ou Return
            return Time.time - _startTime < _seconds && 
                   !Input.GetKeyUp(KeyCode.Escape) && 
                   !Input.GetKeyUp(KeyCode.Return);
        }
    }
}