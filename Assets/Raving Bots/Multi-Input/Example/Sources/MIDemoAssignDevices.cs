using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RavingBots.MultiInput;

using UnityEngine;
using UnityEngine.UI;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

// this class is responsible for assigning devices to players
// and updating the UI
public class MIDemoAssignDevices : MonoBehaviour {
    private static readonly IList<InputCode> InterestingAxes;

    public GameObject AssignDevices;
    public Text Instructions1;
    public Text Instructions2;
    public Text AvailableDevices;
    public GameObject Controls;
    public GameObject PlayerPrefab;

    private MIDemoPlayer[] _players;
    private InputState _inputState;
    private string _devicesPrefix;
    private int _currentPlayer;

    // we want to check all axes *but* few analog ones
    // that might trigger false positives or be otherwise
    // inconvenient (like mouse movement or gamepad sticks)
    static MIDemoAssignDevices() {
        InterestingAxes = InputStateExt
            .AllAxes
            .Where(axis => !IsUninteresting(axis))
            .ToList();
    }

    private static bool IsUninteresting(InputCode axis) {
        switch (axis) {
            case InputCode.MouseX:
            case InputCode.MouseY:
            case InputCode.MouseXLeft:
            case InputCode.MouseXRight:
            case InputCode.MouseYUp:
            case InputCode.MouseYDown:
            case InputCode.PadLeftStickY:
            case InputCode.PadLeftStickX:
            case InputCode.PadLeftStickDown:
            case InputCode.PadLeftStickUp:
            case InputCode.PadLeftStickLeft:
            case InputCode.PadLeftStickRight:
            case InputCode.PadRightStickY:
            case InputCode.PadRightStickX:
            case InputCode.PadRightStickDown:
            case InputCode.PadRightStickUp:
            case InputCode.PadRightStickLeft:
            case InputCode.PadRightStickRight:
                return true;
            default:
                return false;
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake() {
        _players = null;
        _inputState = GetComponent<InputState>();
        _devicesPrefix = AvailableDevices.text;

        AssignDevices.SetActive(false);

        // we don't need the arguments here
        _inputState.DeviceStateChanged.AddListener((d, e) => UpdateDeviceList());
        _inputState.DevicesEnumerated.AddListener(UpdateDeviceList);
    }

    // ReSharper disable once UnusedMember.Local
    private void Update() {
        if (_players == null) {
            return;
        }

        var player = _players[_currentPlayer];

        IDevice device;
        InputCode input;
        if (!FindInput(out device, out input)) {
            return;
        }

        player.Assign(device, input);
        UpdateDeviceList();

        if (player.Ready) {
            var next = _currentPlayer + 1;

            if (next < _players.Length) {
                SetCurrentPlayer(next);
            } else {
                StartGame();
            }
        } else {
            SetInstructions(true);
        }
    }

    private void StartGame() {
        AssignDevices.SetActive(false);
        Controls.SetActive(true);

        foreach (var player in _players) {
            player.CreatePawns();
        }

        enabled = false;
    }

    private bool FindInput(out IDevice outDevice, out InputCode outCode) {
        // this returns first unassigned device with non-zero
        // input on one of the axes that we're interested in
        //
        // this is how you can implement key/button mapping, too:
        // simply run through supported axes and see which
        // one reports input (see InputStateExt.FindFirst documentation
        // for details)
        //
        // we also grab the InputCode so we can tell whether the device
        // is (likely) a keyboard, a mouse or a gamepad
        var player = _players[_currentPlayer];

        Func<IDevice, IVirtualAxis, bool> predicate = (device, axis) => {
            if (IsAssigned(device)) {
                return false;
            }

   

            return axis.IsUp;
        };

        // see IsUninteresting above
        IVirtualAxis outAxis;
        if (_inputState.FindFirst(out outDevice, out outAxis, predicate, InterestingAxes)) {
            outCode = outAxis.Code;
            return true;
        }

        outDevice = null;
        outCode = InputCode.None;
        return false;
    }

    public void SetPlayerCount(int count) {
        // this is called from MIDemoSelectPlayerCount after user
        // presses 'start' button; here we create all of the player
        // objects (without pawns) and show the assignment instructions UI
        _players = new MIDemoPlayer[count];

        for (var idx = 0; idx < count; ++idx) {
            var player = idx + 1;
            var playerObj = Instantiate(PlayerPrefab);
            playerObj.name = string.Format("Player {0}", player);
            _players[idx] = playerObj.GetComponent<MIDemoPlayer>();
            _players[idx].Player = player;
        }

        SetCurrentPlayer(0);
        AssignDevices.SetActive(true);

        // we need to discard all current input, otherwise
        // UI actions (e.g. the LMB/Enter/A button press on
        // the Start button) would cause that device to immediately
        // become assigned to first player
        _inputState.Reset();
    }

    private void SetCurrentPlayer(int player) {
        _currentPlayer = player;
        SetInstructions();
    }

    public void SetInstructions(bool partial = false) {
        // this updates the on-screen instructions to tell the player
        // that we're expecting a button press to assign a device to them
        //
        // because we expect every player to either have both keyboard and mouse,
        // or a gamepad, we need to handle the situation where player has either a keyboard
        // or a mouse, but not both -- in that case 'partial' will be true, and we'll check
        // what device has already been assigned and update the text accordingly
        var instructions = new StringBuilder();
        Instructions1.text = string.Format("<b>Selecting devices for player #{0}</b>", _players[_currentPlayer].Player);

        if (!partial) {
            instructions.AppendLine("Please select your device (keyboard, mouse, or pad) by pressing any button.");
        } 

     //   Instructions2.text = instructions.ToString();
    }

    private void UpdateDeviceList() {
        var names = new StringBuilder();
        names.AppendLine(_devicesPrefix);
        names.AppendLine();

        foreach (var device in _inputState.Devices) {
            if (!device.IsUsable) {
                continue;
            }

            var color = GetDeviceColor(device);
            var label = GetDeviceLabel(device);
            names.AppendFormat("<color={0}>[#{1}] \"{2}\" {3}</color>\n", color, device.Id, device.Name, label);
        }

        AvailableDevices.text = names.ToString();
    }

    private string GetDeviceColor(IDevice device) {
        return IsAssigned(device) ? "green" : "#343434FF";
    }

    private string GetDeviceLabel(IDevice device) {
        if (!IsAssigned(device))
            return "";

        var player = _players.First(p => p.HasDevice(device));
        return string.Format("[assigned to player {0}]", player.Player);
    }

    private bool IsAssigned(IDevice device) {
        return _players != null && _players.Any(player => player.HasDevice(device));
    }
}
