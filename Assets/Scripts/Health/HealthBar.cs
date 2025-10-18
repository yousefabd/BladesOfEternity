using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private HealthSystem HealthSystem;
    [SerializeField]private Image BarImage;

    public void Setup(HealthSystem healthSystem) {
        this.HealthSystem = healthSystem;

        Debug.Log("health bar setup " + this.HealthSystem.GetHealth());

        // this.BarImage.fillAmount = (float)0.5;
    }
}
