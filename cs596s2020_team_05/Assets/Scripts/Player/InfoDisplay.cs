using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Text healthCurrent = null;
    [SerializeField] private Text healthTotal = null;
    //
    [Header("Ammo")]
    [SerializeField] private Text[] ammoCurrent = null;
    [SerializeField] private Text[] ammoTotal = null;


    //[SerializeField] private float updateSpeedSeconds = 0.2f;    

    private void Awake()
    {
        GetComponentInParent<HealthBase>().OnHealthAmountChanged += HandleHealthAmountChanged;
        GetComponentInParent<PlayerAttack>().OnAmmoAmountChanged += HandleAmmoAmountChanged;
    }

    private void HandleHealthAmountChanged(float health_current, float health_total)
    {
        // foregroundImage.fillAmount = pct;
        //healthCurrent.Text = pct.ToString("000");
        healthCurrent.text = health_current.ToString("000");
        healthTotal.text = health_total.ToString("000");
    }

    private void HandleAmmoAmountChanged(float ammo_current, float ammo_total, int num) // num = gunIndex
    {
        // foregroundImage.fillAmount = pct;
        //healthCurrent.Text = pct.ToString("000");
        ammoCurrent[num].text = ammo_current.ToString("000");
        ammoTotal[num].text = ammo_total.ToString("000");
    }





}
