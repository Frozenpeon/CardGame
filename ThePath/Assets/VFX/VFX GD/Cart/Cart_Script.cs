using Com.IsartDigital.F2P.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.VFX_Cart
{
    public class Cart_Script : MonoBehaviour
    {
        [SerializeField] private GameObject _DragonEgg;
        [SerializeField] private GameObject _MoleEgg;
        [SerializeField] private GameObject _Mole;
        [SerializeField] private GameObject _Dragon;


        [SerializeField] private GameObject _DragonParticle;
        [SerializeField] private GameObject _MoleParticle;

        [SerializeField] private GameObject _GainWheatParticle;
        [SerializeField] private GameObject _GainArrowParticle;


        [SerializeField] private List<ParticleSystem> particles = new();
        public Animator animator => GetComponent<Animator>();

        private void Start()
        {
            _Mole.SetActive(false);
            _Dragon.SetActive(false);
            _MoleEgg.SetActive(false);
            _DragonEgg.SetActive(false);

            EggManager.onEggFinish += ActivePet;
            EggManager.onEggFinish += DesactiveEgg;
            EggManager.onNewEgg += ActiveEgg;
        }
        public void ActivePet(EggType pType)
        {
            if (pType == EggType.Mole)
            {
                _Mole.SetActive(true);
                Instantiate(_MoleParticle, _Mole.transform);
            }
            else
            {
                _Dragon.SetActive(true);
                Instantiate(_DragonParticle, _Dragon.transform);
            }
        }
        public void ActiveEgg(EggType pType)
        {
            if (pType == EggType.Mole) _MoleEgg.SetActive(true);
            else _DragonEgg.SetActive(true);
        }
        public void DesactiveEgg(EggType pType)
        {
            if (pType == EggType.Mole) _MoleEgg.SetActive(false);
            else _DragonEgg.SetActive(false);
        }

        public void Activate_Particle(int index)
        {
            particles[index].Play();
        }
        public void Deactivate_Particle(int index)
        {
            particles[index].Stop();
        }

        public void GainWheat() 
        {
            Instantiate(_GainWheatParticle,transform);
        }
        public void GainArrow()
        {
            Instantiate(_GainArrowParticle, transform);
        }

        [HideInInspector] public bool isWin = false;
        [HideInInspector] public int whichRessource = 0;
        public void WinAnimation()
        {
            /* 
             * whichRessource :
             *  - 0 = wheat
             *  - 1 = arrow
             *  - 2 = wheat & arrow
            */
            if (whichRessource == 0 || whichRessource == 2)
            {
                particles[7].Play();
            }
            else if (whichRessource == 1 || whichRessource == 2)
            {
                particles[8].Play();
            }
        }

        public void OnDefeat()
        {
            particles[0].Play();
            particles[1].Play();
            particles[2].Play();
            particles[3].Play();
        }

        private void OnDestroy()
        {
            EggManager.onEggFinish -= ActivePet;
            EggManager.onEggFinish -= DesactiveEgg;
            EggManager.onNewEgg -= ActiveEgg;
        }
    }
}
