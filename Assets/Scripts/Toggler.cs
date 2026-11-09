// PHOEBE (1033478)
// TOGGLER.CS

using UnityEngine;

public class Toggler : MonoBehaviour
{
    public Transform parent;

    public void Trigger()
    {
        foreach (Transform toggle in parent)
        {
            toggle.gameObject.SetActive(false);
        }
    }
}
