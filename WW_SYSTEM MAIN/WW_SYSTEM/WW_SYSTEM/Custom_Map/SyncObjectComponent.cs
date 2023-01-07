using AdminToys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace WW_SYSTEM.Custom_Map
{
    public class SyncObjectComponent : MonoBehaviour
    {
        private GameObject _parent;
        private Light _Parentlight;
        private LightSourceToy _light;
        private bool _IsLight;

        public void Init(GameObject parent, bool IsLight)
        {
            _parent = parent;
            _IsLight = IsLight;
            if (IsLight) 
            {
                _Parentlight = parent.GetComponent<Light>();
                _light = GetComponent<LightSourceToy>();
            }
            
        }



        void FixedUpdate()
        {
            if (_parent == null || (_IsLight && (_light == null || _Parentlight == null))) return;

            if (_IsLight)
            {
                _light.NetworkLightColor = _Parentlight.color;
                _light.NetworkLightIntensity = _Parentlight.intensity;
                _light.NetworkLightRange = _Parentlight.range;
                _light.NetworkLightShadows = _Parentlight.shadows == LightShadows.Soft;
            }
            transform.position = _parent.transform.position;
            transform.localScale = _parent.transform.localScale;
            transform.rotation = _parent.transform.rotation;
        }

    }
}
