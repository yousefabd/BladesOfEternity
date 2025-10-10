using CodeMonkey.Utils;
using System;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    private float interactionRadius = 0.2f;

    public event Action<CombatUnit> OnClickUnit;
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
            Collider2D collider = Physics2D.OverlapCircle(UtilsClass.GetMouseWorldPosition(), interactionRadius);
            if (collider != null)
            {
                if(collider.TryGetComponent(out CombatUnit combatUnit))
                {
                    OnClickUnit?.Invoke(combatUnit);
                }
            }
            else
            {
                OnMoveUnit?.Invoke(UtilsClass.GetMouseWorldPosition());
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSwitchUnit?.Invoke();
        }
    }
}
