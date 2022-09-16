using UnityEngine;
using UnityEngine.UI;

public class PlayerLine : MonoBehaviour
{
    [SerializeField]
    private Image _hitMark;

    public void ShowMark(bool value)
    {
        _hitMark.color = value ? Color.green : Color.white;
    }
}
