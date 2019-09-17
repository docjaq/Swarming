using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextUpdater : MonoBehaviour {

        private Text textValue;
        //Annoying fix due to bug in 2019.2.5 (dynamic values are not working)
        [SerializeField] Slider slider;
        
        private void Start() {
                textValue = GetComponent<Text>();
        }
        
        public void UpdateValue() {
                textValue.text = slider.value.ToString();
        }
        
}
