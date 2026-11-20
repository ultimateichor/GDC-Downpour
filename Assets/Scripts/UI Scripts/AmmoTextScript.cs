using TMPro;
using UnityEngine;

public class AmmoTextScript : MonoBehaviour
{
    private TMP_Text ammoText;

    private void Start()
    {
        if (ammoText == null)
        {
            ammoText = GetComponent<TMP_Text>();
        }
    }

    public void SetAmmoText(int ammo)
    {
        ammoText.text = "Ammo: " + ammo;
    }
}
