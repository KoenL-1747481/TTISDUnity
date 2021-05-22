using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using UnityEngine;

namespace TTISDProject
{
    class LoopRecorder
    {
        private static int BPM;
        private static int Bars;
        private static bool Recording = false;
        private static bool StartedRecording = false;
        private static Timer ClickTimer;

        private static float[] record_buffer = new float[10000000];
        public static float[] recorded_audio;
        private static int amount_recorded = 0;
        private static object record_lock = new object();

        public static void Dispose()
        {
            Recording = false;
            StartedRecording = false;
            ClickTimer?.Stop();
            ClickTimer?.Close();
        }

        public static bool IsRecording()
        {
            return StartedRecording || Recording;
        }

        public static void HandleAudio(float[] audio, int count)
        {
            lock (record_lock)
            {
                if (StartedRecording && Recording)
                {
                    Array.Copy(audio, 0, record_buffer, amount_recorded, count);
                    amount_recorded += count;
                }
            }
        }

        private static void StopRecording()
        {
            lock (record_lock)
            {
                if (StartedRecording && Recording)
                {
                    StartedRecording = false;
                    Recording = false;
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        RecordButton.btn.interactable = true;
                    });
                    recorded_audio = new float[amount_recorded];
                    Array.Copy(record_buffer, 0, recorded_audio, 0, amount_recorded);
                    SessionManager.instance.TrySendLoop(recorded_audio);
                }
            }
        }
        public static void StartRecording(int bpm, int bars)
        {
            if (!StartedRecording && !Recording)
            {
                AudioHandler.StopLoop();
                RecordButton.btn.interactable = false;
                StartedRecording = true;
                BPM = bpm;
                Bars = bars;
                amount_recorded = 0;

                // Play click track, then start recording by setting Recording to true after 4 clicks
                // Set recording back to false after time determined by BPM and Bars
                double clickInterval = (1.0 / (bpm / 60.0)) * 1000.0;
                var timer = new Timer(clickInterval);
                int clicks = 0;
                timer.Elapsed += (s, e_) =>
                {
                    clicks++;
                    if (clicks == 5)
                    { // Begin recording at 5th click
                        Debug.Log("Started recording.");
                        AudioHandler.StartLoop();
                        Recording = true;
                        AudioHandler.PlayClickSound();
                    }
                    else if ((clicks - 1) / Bars == (Bars + 1))
                    { // End recording at start of bar "Bars + 1" 
                        Debug.Log("Stopped recording.");
                        timer.Stop();
                        StopRecording();
                    } else
                    {
                        AudioHandler.PlayClickSound();
                    }
                };
                timer.AutoReset = true;
                timer.Enabled = true;
            }
        }
    }
}
