using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTISDProject
{
    public class PlayerVolumeSlider : MonoBehaviour
    {
        Slider slider;
        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(delegate { onVolumeChanged(); });
        }

        private void onVolumeChanged()
        {
            AudioHandler.SetPlayerVolume(slider.value);
        }
    }
}
