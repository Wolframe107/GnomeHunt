using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using TMPro;

public class PlayerNameDisplay : MonoBehaviour
{
    public GameObject playerText;
    public GameObject playerText2;
    private PlayerManagerNetwork playerManager;

    public void UpdatePlayerText(FixedString64Bytes newName)
    {
        playerText.GetComponent<TextMeshProUGUI>().text = newName.ToString();
        playerText2.GetComponent<TextMeshProUGUI>().text = newName.ToString();
    }
}
