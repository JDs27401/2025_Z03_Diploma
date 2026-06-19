using System;
using C__Classes.Managers;
using C__Classes.SaveSystem;
using C__Classes.SceneManagement;
using C__Classes.Singletons;
using DG.Tweening;
using Enemy.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Systems
{
    public class Universe : SingletonNonPersistant<Universe>
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

        private GameObject _bed;
        private GameObject _player;

        [Header("Game End")]
        [SerializeField] private GameObject gameEndScreenPrefab;

        private void Start()
        {
            Hour = StartingHour;
            Minute = StartingMinute;

            _bed = GameObject.FindGameObjectWithTag("bed");
            if (_bed == null)
            {
                throw new Exception("Bed not found");
            }
            _player = GameObject.FindGameObjectWithTag("player");
            if (_player == null)
            {
                throw new Exception("Player not found");
            }
            
            EnemySpawner.Instance.StartLatenedSpawning();
            WaveManager.Instance.OnWaveCompleted += HandleWaveCompletion;
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
                WaveManager.Instance.StartWaveCoroutine();
            } 
            else if (Hour >= SundownThreshold)
            {
                if (TimeOfDay == Phase.Sundown)
                {
                    return;
                }
                TimeOfDay = Phase.Sundown;
            } 
            else if (Hour >= DayThreshold)
            {
                if (TimeOfDay == Phase.Day)
                {
                    return;
                }
                TimeOfDay = Phase.Day;
            } 
        }

        private void HandleWaveCompletion()
        {
            Hour = DayThreshold;
            TimeOfDay = Phase.Day;
            Day += 1;

            GameObject fadeGO = new GameObject("FadeOverlay", typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = fadeGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            GameObject blackImg = new GameObject("Black");
            blackImg.transform.SetParent(fadeGO.transform, false);
            Image img = blackImg.AddComponent<Image>();
            img.color = Color.black;
            RectTransform rt = blackImg.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            CanvasGroup cg = fadeGO.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            Sequence seq = DOTween.Sequence();
            seq.Append(cg.DOFade(1f, 1f));
            seq.AppendCallback(() =>
            {
                ChangeSpawningState?.Invoke();
                EnemySpawner.Instance.StartLatenedSpawning();
                MovePlayerToBed();
            });
            seq.Append(cg.DOFade(0f, 1f));
            seq.OnComplete(() =>
            {
                Destroy(fadeGO);
                SaveGameManager.Instance.SaveGame();
            });
        }

        public void StartFinalWave()
        {
            if (WaveManager.Instance.AlreadyStarted || TimeOfDay == Phase.Night)
            {
                return;
            }

            WaveManager.Instance.OnWaveCompleted -= HandleWaveCompletion;
            WaveManager.Instance.OnWaveCompleted += HandleFinalWaveCompletion;
            TimeOfDay = Phase.Night;
            EnemySpawner.Instance.StopSpawning();
            WaveManager.Instance.StartWaveCoroutine();
        }

        private void HandleFinalWaveCompletion()
        {
            WaveManager.Instance.OnWaveCompleted -= HandleFinalWaveCompletion;

            if (gameEndScreenPrefab != null)
            {
                GameObject endScreenGO = Instantiate(gameEndScreenPrefab);
                endScreenGO.GetComponent<GameEndScreenManager>().Show();
            }
        }

        private void MovePlayerToBed()
        {
            Vector3 newPlayerPos = new Vector3(_bed.transform.position.x, _bed.transform.position.y - 1, _bed.transform.position.z);
            _player.transform.position = newPlayerPos;
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
            WaveManager.Instance.OnWaveCompleted -= HandleFinalWaveCompletion;
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