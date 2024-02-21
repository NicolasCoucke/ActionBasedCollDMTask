using RavingBots.MultiInput;

using UnityEngine;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

// this class is responsible for querying input from specific
// device and moving the player pawn object around the screen
public class MIDemoPawnController : MonoBehaviour {
    private const float Speed = 3f;

    // this is set to the device object that we get from InputState
    public IDevice Device;

    // this is here because for gamepads we use two sticks on one
    // device instead of two devices, so we have to query different
    // InputCode for the left and right one
    public bool IsLeftPawn;

    // ReSharper disable once UnusedMember.Local
    private void Update() {
        if (Device == null) {
            return;
        }

        // query device axes to see whether we should move
        // the pawn
        //
        // you can see how you can construct new axes based
        // on existing ones: we use that to treat A and D keys
        // as two ends of an X axis, and W and S keys as two ends
        // of an Y axis
        //
        // it also demonstrates how alternate bindings can be checked

        // simply check all axes we want to correspond to particular direction,
        // NB all of those axes are in range [0, 1]
        var xNegative = GetValue(
            InputCode.KeyA,
            InputCode.KeyLeftArrow,
            InputCode.MouseXLeft,
            IsLeftPawn ? InputCode.PadLeftStickLeft : InputCode.PadRightStickLeft);
        var xPositive = GetValue(
            InputCode.KeyD,
            InputCode.KeyRightArrow,
            InputCode.MouseXRight,
            IsLeftPawn ? InputCode.PadLeftStickRight : InputCode.PadRightStickRight);
        var yNegative = GetValue(
            InputCode.KeyS,
            InputCode.KeyDownArrow,
            InputCode.MouseYDown,
            IsLeftPawn ? InputCode.PadLeftStickDown : InputCode.PadRightStickDown);
        var yPositive = GetValue(
            InputCode.KeyW,
            InputCode.KeyUpArrow,
            InputCode.MouseYUp,
            IsLeftPawn ? InputCode.PadLeftStickUp : InputCode.PadRightStickUp);

        var x = new VirtualAxis();
        var y = new VirtualAxis();

        x.Set(xNegative > 0 ? -xNegative : xPositive);
        x.Commit();

        y.Set(yNegative > 0 ? -yNegative : yPositive);
        y.Commit();

        // after grabbing the axes all that's left is to move the pawn
        // accordingly
        var translation = new Vector3(x.Value, y.Value, 0) * Speed * Time.deltaTime;
        transform.position = transform.position + translation;

        // this demonstrates the vibration API
        var vibrate = Device[InputCode.PadA];
        if (vibrate != null && vibrate.IsDown) {
            // vibrate for 1s at 75% strength on both motors
            Device.Vibrate(1000, 0.75f, 0.75f);
        }
    }

    private float GetValue(params InputCode[] codes) {
        foreach (var code in codes) {
            var axis = Device[code];
            if (axis != null && axis.IsHeld) {
                return axis.Value;
            }
        }

        return 0;
    }
}
