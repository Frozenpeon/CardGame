//using FMOD.Studio;
//using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Manager
{
	public class SoundManager : MonoBehaviour
	{
		//[field: SerializeField] private List<EventReference> _listMusicEventReference = new List<EventReference>();

		//private List<EventInstance> _listEventInstances = new List<EventInstance>();

		private const string PATH_VCA_MUSIC = "vca:/Music";

		public float _musicVolume = 1f;

		//private VCA _musicVCA;

		//private EventInstance _musicEventInstance;
		
		private static SoundManager instance;

		private SoundManager() { }

		public static SoundManager GetInstance
		{
			get { return instance; }
			private set { instance = value; }
		}

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(this);
				Debug.Log("This instance of " + GetType().Name + " already exist, destroying the last one added");
				return;
			}
			else instance = this;

			DontDestroyOnLoad(gameObject);

			//_musicVCA = RuntimeManager.GetVCA(PATH_VCA_MUSIC);
		}

		// Start is called before the first frame update
		void Start()
		{
			//InitializedMusic(_listMusicEventReference[0]);
		}

		//private EventInstance CreateInstance(EventReference pEventReference)
		//{
		//	EventInstance lEventInstance = RuntimeManager.CreateInstance(pEventReference);
		//	_listEventInstances.Add(lEventInstance);
		//	return lEventInstance;
		//}

		//private void InitializedMusic(EventReference pMusicEventReference)
		//{
		//	_musicEventInstance = CreateInstance(pMusicEventReference);
		//	_musicEventInstance.start();
		//}

		//private void CleanUp()
		//{
		//	foreach(EventInstance lEventInstance in _listEventInstances)
		//	{
		//		lEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		//		lEventInstance.release();
		//	}
		//}

		//public void OnMusicVolumeChange(float pVolume)
		//{
		//	_musicVolume = pVolume;
		//	_musicVCA.setVolume(_musicVolume);
		//}
	}
}