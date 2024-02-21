using RavingBots.MultiInput;

using UnityEngine;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

// this class holds prefab values for a single pawn, and a
// convenience method for turning prefab clone into properly
// setup player pawn
public class MIDemoPlayerPawn : MonoBehaviour {
    public TextMesh Label;
    public SpriteRenderer Sprite;

    public Sprite KeyboardSprite;
    public Sprite MouseSprite;
    public Sprite GamepadSprite;
    public Color KeyboardColor;
    public Color MouseColor;
    public Color GamepadLeftColor;
    public Color GamepadRightColor;

    public MIDemoPawnController Controller { get; private set; }

    private void Awake() {
        Controller = GetComponent<MIDemoPawnController>();
    }

    public void SetPlayer(int player, IDevice device, bool isLeft, MIDemoPawnType type) {
        name = string.Format("Player {0} {1} pawn", player, isLeft ? "left" : "right");

        var color = GetDeviceColor(type, isLeft);
        Label.text = player.ToString();
        Label.color = color;
        Sprite.sprite = GetDeviceSprite(type);
        Sprite.color = color;
        Controller.IsLeftPawn = isLeft;
        Controller.Device = device;
    }

    private Color GetDeviceColor(MIDemoPawnType type, bool isLeft) {
        switch (type) {
            case MIDemoPawnType.Keyboard:
                return KeyboardColor;
            case MIDemoPawnType.Gamepad:
                return isLeft ? GamepadLeftColor : GamepadRightColor;
            case MIDemoPawnType.Mouse:
                return MouseColor;
        }

        return Color.white;
    }

    private Sprite GetDeviceSprite(MIDemoPawnType type) {
        switch (type) {
            case MIDemoPawnType.Keyboard:
                return KeyboardSprite;
            case MIDemoPawnType.Gamepad:
                return GamepadSprite;
            case MIDemoPawnType.Mouse:
                return MouseSprite;
        }

        return null;
    }
}
