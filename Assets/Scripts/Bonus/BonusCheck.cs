using UnityEngine;

public class BonusCheck : MonoBehaviour
{
    public bool isActive { get; private set; }

    public event System.Action BonusActiveChanged;

    public void SetBonusActive(bool active)
    {
        isActive = active;
        BonusActiveChanged?.Invoke();
    }
}