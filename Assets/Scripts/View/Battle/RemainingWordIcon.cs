using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View {
    public class RemainingWordIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        void Start()
        {
            ChangeState(true);
        }

        public void ChangeState(bool state)
        {
            _icon.gameObject.SetActive(state);
        }
    }
}
