using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
