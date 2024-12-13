using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class Clicker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonUp(1)) { AugmentHandler.Instance.Show(); }
          
        }
    }
}
