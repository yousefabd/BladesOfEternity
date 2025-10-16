using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private HealthSystem HealthSystem;
    public void Setup(HealthSystem healthSystem) {
        this.HealthSystem = healthSystem;
    } 

    private void Update() {
        transform.Find("Bar").localScale = new Vector3(this.HealthSystem.GetHealthPercent(),1);
    }
}
