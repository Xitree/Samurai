using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeaponAnimationEvent : MonoBehaviour{

    private AICombatSystem _combatSystem;
    [SerializeField] private Transform hipGS;
    [SerializeField] private Transform handGS;
    [SerializeField] private Transform handKatana;
    
    
    

    private void ShowGs() {
        // if (!handGS.gameObject.activeSelf) {
        //     handGS.gameObject.SetActive(true);
        //     hipGS.gameObject.SetActive(false);
        //     handKatana.gameObject.SetActive(false);
        // }
    }

    private void HideGs() {
        // if (handGS.gameObject.activeSelf) {
        //     handGS.gameObject.SetActive(false);
        //     hipGS.gameObject.SetActive(true);
        //     handKatana.gameObject.SetActive(true);
        // }
    }

    private void HideKatana() {
        handKatana.gameObject.SetActive(false);
    }

    private void ShowKatana() {
        handKatana.gameObject.SetActive(true);
    }
}
