using System;
using C__Classes.Managers;
using C__Classes.SceneManagement;
using C__Classes.Singletons;
using Enemy.Scripts;
using UnityEngine;

namespace C__Classes.Systems
{
    public class Universe : SingletonPersistant<Universe>
    {
        public static event Action ChangeSpawningState;
        
        private static int Day = 1;
        [Range(0, 24)] private static int Hour = 0;
        [Range(0,60)] private static float Minute = 0;
        [Range(0,24)] private static float RealTime = 0; //should be used for light movement, angle, etc.
        
        private static Phase TimeOfDay;

        [Header("Time Speed")]
        [SerializeField] private float Ratio = 1;

        [Header("Starting settings")]
        [SerializeField] private int StartingHour = 8;
        [SerializeField] private float StartingMinute = 0;
        [Header("Day Phases Thresholds")]
        [SerializeField] private int DayThreshold = 8;
        [SerializeField] private int SundownThreshold = 18;
        [SerializeField] private int NightThreshold = 21;

        private void Start()
        {
            Hour = StartingHour;
            Minute = StartingMinute;
            
            EnemySpawner.Instance.StartLatenedSpawning();
        }
        
        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (TimeOfDay == Phase.Night)
            {
                return;
            }
            //@todo jakoś zaimplementować to że podczas nocy czas nie leciał -- można pewnie całe SetDayPhase wyjebać
            var increment = Time.deltaTime * Ratio;
            Minute += increment;

            if (Minute >= 60)
            {
                Hour += 1;
                Minute = 0;
            }

            if (Hour >= 24)
            {
                Day += 1;
                Hour = 0;
            }
            
            RealTime += increment;
            RealTime %= 24f;
            
            SetDayPhase();
            // PrintTime();
        }

        private void SetDayPhase()
        {
            if (Hour >= NightThreshold || Hour < DayThreshold)
            {
                if (TimeOfDay == Phase.Night)
                {
                    return;
                }
                TimeOfDay = Phase.Night;
                if (GameObject.FindGameObjectWithTag("player").GetComponent<PlayerController>().IsInside)
                {
                    SceneManagment.KickPlayerOut();
                }
                if (WaveManager.Instance.AlreadyStarted)
                {
                    return;
                }
                EnemySpawner.Instance.StopSpawning();
                StartCoroutine(WaveManager.Instance.StartWave());
                WaveManager.Instance.OnWaveCompleted += HandleWaveCompletion;
            } 
            else if (Hour >= SundownThreshold)
            {
                TimeOfDay = Phase.Sundown;
            } 
            else if (Hour >= DayThreshold)
            {
                TimeOfDay = Phase.Day;
            } 
        }

        private void HandleWaveCompletion()
        {
            //@todo implement switching back to day
            #if UNITY_EDITOR
            print("Wave ended, starting day");
            #endif
            Hour = DayThreshold;
            TimeOfDay = Phase.Day;
            Day += 1;
            ChangeSpawningState?.Invoke();
            EnemySpawner.Instance.StartLatenedSpawning();
        }

        public void StartFinalWave()
        {
            if (WaveManager.Instance.AlreadyStarted || TimeOfDay == Phase.Night)
            {
                return;
            }

            WaveManager.Instance.OnWaveCompleted -= HandleWaveCompletion;
            TimeOfDay = Phase.Night; //possibly to change the way we stop time flow, as it can introduce some issues
            EnemySpawner.Instance.StopSpawning();
            StartCoroutine(WaveManager.Instance.StartWave());
            // WaveManager.Instance.OnWaveCompleted += /*metoda która kończy gre*/;
            //@todo design a game end screen and method to invoke it, subscribe it to this event
        }
        
        private static void PrintTime() //just a debug method
        {
            print($"Day: {Day} Hour: {Hour} Minute: {(int) Minute} Phase: {TimeOfDay} Realtime: {RealTime}");
        }

        public static int GetDay()
        {
            return Day;
        }

        public static int GetHour()
        {
            return Hour;
        }

        public static float GetMinute()
        {
            return Minute;
        }

        public static float GetRealTime()
        {
            return RealTime;
        }

        public static Phase GetTimeOfDay()
        {
            return TimeOfDay;            
        }
        
        public enum Phase
        {
            Day,
            Sundown,
            Night,
        }

        public float GetRatio()
        {
            return Ratio;
        }

        private void OnDestroy()
        {
            WaveManager.Instance.OnWaveCompleted -= HandleWaveCompletion;
        }

        public static void SetDay(int day)
        {
            Day = day;
        }

        public static void SetHour(int hour)
        {
            Hour = hour;
        }

        public static void SetMinute(int minute)
        {
            Minute = minute;
        }
    }
}