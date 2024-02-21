using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RavingBots.MultiInput;

using UnityEngine;
using UnityEngine.UI;


public class KeyPadManager : MonoBehaviour
{
    private static readonly IList<InputCode> InterestingAxes;

    private InputState _inputState;
    public Dictionary<long, IDevice> _devices;

    public GameObject AssignButton;

    private IDevice _keyboard1;
    private IDevice _keyboard2;
    private IDevice _keyboard3;
    private IDevice _keyboard4;
    // public Text AvailableDevices;
    private string _devicesPrefix;
    public Text KeypadText;
    public string ResponseString;

    bool listening;
    private int answer1;
    private int answer2;
    private int answer3;
    private int answer4;
    private int confidence1;
    private int confidence2;
    private int confidence3;
    private int confidence4;
    public bool AnswersDone;

    public static KeyPadManager keyPadManager;

    private static bool IsUninteresting(InputCode axis)
    {
        switch (axis)
        {
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
    private void Awake()
    {
        listening = false;
       // _players = null;
        _inputState = GetComponent<InputState>();
       // _devicesPrefix = AvailableDevices.text;
        _inputState = FindObjectOfType<InputState>();
        _devices = new Dictionary<long, IDevice>();

       // AssignDevices.SetActive(false);

        _inputState.DeviceStateChanged.AddListener(OnDeviceStateChanged);

        // we don't need the arguments here
        // _inputState.DeviceStateChanged.AddListener((d, e) => UpdateDeviceList());
        // _inputState.DevicesEnumerated.AddListener(UpdateDeviceList);

        keyPadManager = this;
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartListenForAnswers()
    {
        AnswersDone = false;
        answer1 = -1;
        answer2 = -1;
        answer3 = -1;
        answer4 = -1;
        confidence1 = -1;
        confidence2 = -1;
        confidence3 = -1;
        confidence4 = -1;
        listening = true;
    }

   


    public bool HasDevice(IDevice device)
    {
        return _devices.ContainsKey(device.Id);
    }

    public void StartAssigningDevices()
    {
        _devices.Clear();
        InvokeRepeating("ListenForDevices", 0.5f, 0.1f);
    }


    void ListenForDevices()
    {
        if (_devices.Count() < StaticVariables.NumPlayers) // listen for new devices
        {
            IDevice device;
            SetInstructions();
            InputCode input;
            if (!FindInput(out device, out input))
            {
                return;
            }


            if (!_devices.ContainsKey(device.Id) && input.IsKeyboard())
            {
                Debug.Log("assigndevice");
                _devices.Add(device.Id, device);
                if (_devices.Count == 1)
                {
                    _keyboard1 = device;
                }
                if (_devices.Count == 2)
                {
                    _keyboard2 = device;
                }
                if (_devices.Count == 3)
                {
                    _keyboard3 = device;
                }
                if (_devices.Count == 4)
                {
                    _keyboard4 = device;
                }
            }
        }
        else
        {
            SetInstructions();
            CancelInvoke("ListenForDevices");
            AssignButton.SetActive(false);
        }
    }
    

    void RegisterKeyPress(int key, int dev)
    {
        if(dev == 1)
        {
            if(answer1 == -1)
            {
                answer1 = key;
            }
            else
            {
                if (confidence1 == -1)
                {
                    confidence1 = key;
                }
            }
        }

        if (dev == 2)
        {
            if (answer2 == -1)
            {
                answer2 = key;
            }
            else
            {
                if (confidence2 == -1)
                {
                    confidence2 = key;
                    Debug.Log("conf");
                }
            }
        }

        if (dev == 3)
        {
            if (answer3 == -1)
            {
                answer3 = key;
            }
            else
            {
                if (confidence3 == -1)
                {
                    confidence3 = key;
                    Debug.Log("conf");
                }
            }
        }

        if (dev == 4)
        {
            if (answer4 == -1)
            {
                answer4 = key;
            }
            else
            {
                if (confidence4 == -1)
                {
                    confidence4 = key;
                    Debug.Log("conf");
                }
            }
        }
        if(StaticVariables.NumPlayers == 2)
        {
            ResponseString = "A1: " + answer1.ToString() + ", C1: " + confidence1.ToString() + ", A2: " + answer2.ToString() + ", C2: " + confidence2.ToString();

        }
        if (StaticVariables.NumPlayers == 3)
        {
            ResponseString = "A1: " + answer1.ToString() + ", C1: " + confidence1.ToString() + ", A2: " + answer2.ToString() + ", C2: " + confidence2.ToString() + ", A3: " + answer3.ToString() + ", C3: " + confidence3.ToString();

        }
        if (StaticVariables.NumPlayers == 4)
        {
            ResponseString = "A1: " + answer1.ToString() + ", C1: " + confidence1.ToString() + ", A2: " + answer2.ToString() + ", C2: " + confidence2.ToString() + ", A3: " + answer3.ToString() + ", C3: " + confidence3.ToString() + ", A4: " + answer4.ToString() + ", C4: " + confidence4.ToString();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(listening)
        {
            int dev = 1;
            foreach (KeyValuePair<long, IDevice> _device_ in _devices)
            {
                IDevice _device = _device_.Value;
                if (_device[InputCode.KeyNum0].IsDown)
                {
                    RegisterKeyPress(0, dev);

                }

                if (_device[InputCode.KeyNum1].IsDown)
                {
                    RegisterKeyPress(1, dev);
                }

                if (_device[InputCode.KeyNum2].IsDown)
                {
                    RegisterKeyPress(2, dev);
                }

                if (_device[InputCode.KeyNum3].IsDown)
                {
                    RegisterKeyPress(3, dev);
                }

                if (_device[InputCode.KeyNum4].IsDown)
                {
                    RegisterKeyPress(4, dev);
                }

                if (_device[InputCode.KeyNum5].IsDown)
                {
                    RegisterKeyPress(5, dev);
                }

                if (_device[InputCode.KeyNum6].IsDown)
                {
                    RegisterKeyPress(6, dev);
                }

                if (_device[InputCode.KeyNum7].IsDown)
                {
                    RegisterKeyPress(7, dev);
                }

                if (_device[InputCode.KeyNum8].IsDown)
                {
                    RegisterKeyPress(8, dev);
                }
                if (_device[InputCode.KeyNum9].IsDown)
                {
                    RegisterKeyPress(9, dev);
                }
               // if (_device[InputCode.KeyNumEnter].IsDown)
               // {
                 //   RegisterKeyPress(10, dev);
               // }


                dev += 1;
            }

            if (StaticVariables.NumPlayers == 4)
            {
                if (confidence1 != -1 && confidence2 != -1 && confidence3 != -1 && confidence4 != -1)
                {
                    AnswersDone = true;
                    listening = false;

                }

            }

            if (StaticVariables.NumPlayers == 3)
            {
                if (confidence1 != -1 && confidence2 != -1 && confidence3 != -1)
                {
                    AnswersDone = true;
                    listening = false;

                }

            }

            if (StaticVariables.NumPlayers == 2)
            {
                if (confidence1 != -1 && confidence2 != -1)
                {
                    AnswersDone = true;
                    listening = false;

                }

            }

        }
     
    }

    void SetInstructions()
    {
       if(_devices.Count == 0)
        {
            KeypadText.enabled = true;
        }
        if (_devices.Count == 1)
        {
            KeypadText.text = "Press Key on Pad 2";
        }
        if (_devices.Count == 2)
        {
            KeypadText.text = "Press Key on Pad 3";
        }
        if (_devices.Count == 3)
        {
            KeypadText.text = "Press Key on Pad 4";
        }
        if (_devices.Count == StaticVariables.NumPlayers)
        {
            KeypadText.enabled = false;
        }


    }

    private bool FindInput(out IDevice outDevice, out InputCode outCode)
    {
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
      //  var player = _players[_currentPlayer];

        Func<IDevice, IVirtualAxis, bool> predicate = (device, axis) => {
            if (IsAssigned(device))
            {
                return false;
            }


        
            return axis.IsHeld;
        };

        // see IsUninteresting above
        IVirtualAxis outAxis;
        if (_inputState.FindFirst(out outDevice, out outAxis, predicate, InterestingAxes))
        {
            outCode = outAxis.Code;
            return true;
        }

        outDevice = null;
        outCode = InputCode.None;
        return false;
    }

    private void OnDeviceStateChanged(IDevice device, DeviceEvent deviceEvent)
    {
        // if the device got unplugged, we deassign it
        //
        // if the device only got unusable then it can still become usable again,
        // so we don't need to explicitly handle these events here
        if (deviceEvent == DeviceEvent.Removed && HasDevice(device))
        {
            Deassign(device);
        }
    }


    private bool IsAssigned(IDevice device)
    {
        return _devices.ContainsKey(device.Id);
    }

    public void Deassign(IDevice device)
    {
        _devices.Remove(device.Id);

    }
}
