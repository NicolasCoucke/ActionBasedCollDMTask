using UnityEngine;
using UnityEngine.UI;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global

// this class is responsible for handling the 'select player count' UI
// and switching to assign devices screen when start button is pressed
public class MIDemoSelectPlayerCount : MonoBehaviour {
    public GameObject SelectPlayerCount;
    public Dropdown PlayerCount;

    // ReSharper disable once UnusedMember.Local
    private void Awake() {
        SelectPlayerCount.SetActive(true);
    }

    // ReSharper disable once UnusedMember.Global
    public void OnStartPressed() {
        var count = PlayerCount.value + 1;
        SelectPlayerCount.SetActive(false);

        var assign = GetComponent<MIDemoAssignDevices>();
        assign.SetPlayerCount(count);
        enabled = false;
    }
}
