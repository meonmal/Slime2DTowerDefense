using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHp;
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    [SerializeField]
    private PlayerHp playerHp;
    [SerializeField]
    private PlayerGold playerGold;

    private void Update()
    {
        textPlayerHp.text = playerHp.CurrentHp + "/" + playerHp.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
    }
}
