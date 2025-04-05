using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationDirector : MonoBehaviour
{
    public void VibrateOnce(float low,float high,float time)
    {
        StartVibrate(low,high);
        Invoke(nameof(StopVibrate),time);
    }
    public void StartVibrate(float low,float high)
    {
        foreach(Gamepad gamepad in Gamepad.all) gamepad.SetMotorSpeeds(low,high);
    }
    public void StopVibrate() => StartVibrate(0,0);
}
