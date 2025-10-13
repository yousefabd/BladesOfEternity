using CodeMonkey.Utils;
using System;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    private float interactionRadius = 0.2f;

    public event Action<CombatUnit> OnClickUnit;
    public event Action<CombatUnit> OnRightClickUnit;
    public event Action OnSwitchUnit;
    public event Action<Vector3> OnMoveUnit;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(TrySelectUnit(out CombatUnit combatUnit))
            {
                OnClickUnit?.Invoke(combatUnit);
            }
            else
            {
                OnMoveUnit?.Invoke(UtilsClass.GetMouseWorldPosition());
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(TrySelectUnit(out CombatUnit combatUnit))
            {
                OnRightClickUnit?.Invoke(combatUnit);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSwitchUnit?.Invoke();
        }
    }
    private bool TrySelectUnit(out CombatUnit combatUnit)
    {
        Collider2D collider = Physics2D.OverlapCircle(UtilsClass.GetMouseWorldPosition(), interactionRadius);
        if (collider != null)
        {
            if (collider.TryGetComponent(out combatUnit))
            {
                return true;
            }
        }
        combatUnit = null;
        return false;
    }
}
