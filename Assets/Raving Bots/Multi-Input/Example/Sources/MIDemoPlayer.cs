using System;
using System.Collections.Generic;

using RavingBots.MultiInput;

using UnityEngine;

using Random = UnityEngine.Random;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

// this class holds the state of the player: their number,
// all of their devices, and both of their pawns
public class MIDemoPlayer : MonoBehaviour {
    // ReSharper disable once UnassignedField.Global
    public GameObject PawnPrefab;

    private InputState _inputState;
    private Dictionary<long, IDevice> _devices;
    private MIDemoPlayerPawn _leftPawn;
    private MIDemoPlayerPawn _rightPawn;
    private IDevice _keyboard;
    private IDevice _mouse;
    private IDevice _gamepad;

    public bool KeyboardMouse { get; private set; }
    public bool HasKeyboard { get; private set; }

    public bool Ready {
        get {
            if (KeyboardMouse) {
                return _devices.Count == 1;
            }

            return _devices.Count == 1;
        }
    }

    [NonSerialized]
    public int Player;

    void Update()
    {
        if (_keyboard[InputCode.KeyNum8].IsHeld)
        {
            Debug.Log(9);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake() {
        _inputState = FindObjectOfType<InputState>();
        _devices = new Dictionary<long, IDevice>();

        _inputState.DeviceStateChanged.AddListener(OnDeviceStateChanged);
    }

    public bool HasDevice(IDevice device) {
        return _devices.ContainsKey(device.Id);
    }

    public void Assign(IDevice device, InputCode input) {
        Debug.LogFormat("Assigning device to player {1}: {0}", device, Player);

        _devices.Add(device.Id, device);

        if (input.IsKeyboard()) {
            KeyboardMouse = true;
            if (input.IsKeyboard()) {
                HasKeyboard = true;
                _keyboard = device;
            } 
        }
    }

    public void Deassign(IDevice device) {
        Debug.LogFormat("Deassigning device from player {1}: {0}", device, Player);
        _devices.Remove(device.Id);

        if (_devices.Count == 0) {
            // we'll need to update the instructions in this case
            // this will happen if player selects a keyboard or a mouse, and
            // that device gets unplugged
            KeyboardMouse = false;
            FindObjectOfType<MIDemoAssignDevices>().SetInstructions();
        }

        DeassignPawn(device, _leftPawn);
        DeassignPawn(device, _rightPawn);
    }

    private void DeassignPawn(IDevice device, MIDemoPlayerPawn pawn) {
        if (pawn == null) {
            return;
        }

        var controller = pawn.Controller;
        if (controller.Device == null || controller.Device.Id != device.Id) {
            return;
        }

        Debug.LogWarningFormat("Player {0} has lost their {1} pawn controller", Player, controller.IsLeftPawn ? "left" : "right");
        controller.Device = null;
    }

    public void CreatePawns() {
        if (KeyboardMouse)
        {
            _leftPawn = CreatePawn(_keyboard, true, MIDemoPawnType.Keyboard);
        }
    }

    private MIDemoPlayerPawn CreatePawn(IDevice device, bool isLeft, MIDemoPawnType type) {
        var pawnObj = Instantiate(PawnPrefab);
        pawnObj.transform.SetParent(transform);

        // this puts the sprite somewhere inside the camera view
        // there should be enough space to make all of them visible,
        // but if they overlap nothing bad will happen
        var position = new Vector3(Random.value, Random.value, 0f);
        position = Camera.main.ViewportToWorldPoint(position);
        pawnObj.transform.position = new Vector3(position.x, position.y, 0f);

        var pawn = pawnObj.GetComponent<MIDemoPlayerPawn>();
        pawn.SetPlayer(Player, device, isLeft, type);
        return pawn;
    }

    private void OnDeviceStateChanged(IDevice device, DeviceEvent deviceEvent) {
        // if the device got unplugged, we deassign it
        //
        // if the device only got unusable then it can still become usable again,
        // so we don't need to explicitly handle these events here
        if (deviceEvent == DeviceEvent.Removed && HasDevice(device)) {
            Deassign(device);
        }
    }
}
