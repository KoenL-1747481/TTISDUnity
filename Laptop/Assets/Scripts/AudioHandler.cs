using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Threading;

namespace TTISDProject
{
    class AudioHandler : MonoBehaviour
    {
        public static AudioHandler instance;

        private static readonly int SAMPLE_RATE = 44100;
        private static readonly int INPUT_CHANNELS = 2;
        private static readonly int OUTPUT_CHANNELS = 2;
        private static readonly int BUFFER_SIZE = 1024 * 32;
        private static readonly WaveFormat SAMPLE_FORMAT = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, OUTPUT_CHANNELS);

        private static bool allowPlayerAudio = false;

        private static AsioOut AsioDriver;
        private static Dictionary<int, BufferedSampleProvider> PlayerAudio = new Dictionary<int, BufferedSampleProvider>();
        private static CachedSound ClickSound = new CachedSound("Assets/Audio/click.wav");
        public static float ClickVolume = 1.0f;

        private static List<LoopSampleProvider> Loops = new List<LoopSampleProvider>();
        private static bool SavedLoop = true;
        private static int LoopLength = 0;
        private static bool LoopsPaused = false;
        private static object pause_lock = new object();

        private static float[] float_buffer = new float[1024 * 16];

        private static MixingSampleProvider Mixer;
        private static FileWriteProvider AudioWriter;

        private static MixingSampleProvider LoopMixer;
        private static VolumeSampleProvider LoopVolumeChanger;

        private static MixingSampleProvider PlayerMixer;
        private static VolumeSampleProvider PlayerVolumeChanger;

        /* Intialize stuff */
        public void Start()
        {
            Mixer = new MixingSampleProvider(SAMPLE_FORMAT);

            LoopMixer = new MixingSampleProvider(SAMPLE_FORMAT);
            // Filler input so loopmixer doesn't get discarded by mixer
            var fillerLoop = new LoopSampleProvider(new CachedSoundSampleProvider(new CachedSound(new float[1], SAMPLE_FORMAT)));
            Loops.Add(fillerLoop);
            LoopMixer.AddMixerInput(fillerLoop);
            var compressor = new SimpleCompressorEffect(LoopMixer);
            compressor.Enabled = true;
            compressor.MakeUpGain = 0;
            LoopVolumeChanger = new VolumeSampleProvider(compressor);

            // PlayerMixer wont get discarded because we add buffered sample provider of us (player -1)
            PlayerMixer = new MixingSampleProvider(SAMPLE_FORMAT);
            compressor = new SimpleCompressorEffect(PlayerMixer);
            compressor.Enabled = true;
            compressor.MakeUpGain = 0;
            PlayerVolumeChanger = new VolumeSampleProvider(compressor);

            Mixer.AddMixerInput(LoopVolumeChanger);
            Mixer.AddMixerInput(PlayerVolumeChanger);

            AddPlayer(-1);
        }

        private void OnApplicationQuit()
        {
            Dispose();
        }

        public static void Dispose()
        {
            allowPlayerAudio = false;
            AsioDriver?.Dispose();
            foreach (var b in PlayerAudio.Values)
            {
                b.ClearBuffer();
            }
            AudioWriter?.Dispose();
            
        }
        public static void SetAsio(string driverName)
        {
            Dispose();

            var uselessProvider = new VolumeSampleProvider(Mixer);

            /* Init AsioOut for recording and playback */
            IWaveProvider asioProvider;
            if (Constants.DO_AUDIO_WRITE)
            {
                AudioWriter = new FileWriteProvider(Mixer, "trace.wav", SAMPLE_FORMAT);
                asioProvider = AudioWriter.ToWaveProvider16();
            } else
            {
                asioProvider = Mixer.ToWaveProvider16();
            }
            AsioDriver = new AsioOut(driverName);
            AsioDriver.AudioAvailable += OnAsioOutAudioAvailable;
            AsioDriver.InitRecordAndPlayback(asioProvider, INPUT_CHANNELS, SAMPLE_RATE);
            AsioDriver.Play();
 
            allowPlayerAudio = true;
        }

        public static void SaveLoop(string fileName)
        {
            SavedLoop = false;
            Debug.Log("Saving loop...");
            // Disable save button
            ThreadManager.ExecuteOnMainThread(() =>
            {
                SaveButton.btn.interactable = false;
            });
            // Create mixer with all saved loops
            MixingSampleProvider saveMixer = new MixingSampleProvider(SAMPLE_FORMAT);
            saveMixer.MixerInputEnded += OnSavedFile;
            foreach (LoopSampleProvider loop in Loops)
            {
                saveMixer.AddMixerInput(new CachedSoundSampleProvider(loop.source.cachedSound));
            }
            WaveFileWriter.CreateWaveFile(fileName, saveMixer.ToWaveProvider());
        }

        public static void UndoLoop()
        {
            if (Loops.Count > 1)
            {
                LoopMixer.RemoveMixerInput(Loops[Loops.Count - 1]);
                Loops.RemoveAt(Loops.Count - 1);
            }
        }

        private static void OnSavedFile(object sender, SampleProviderEventArgs e)
        {
            if (!SavedLoop)
            {
                Debug.Log("Loop saved!");
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    SaveButton.btn.interactable = true;
                });
                SavedLoop = true;
            }
        }

        public static void SetPlayerVolume(float value)
        {
            PlayerVolumeChanger.Volume = 2*value;
        }

        public static void SetLoopVolume(float value)
        {
            LoopVolumeChanger.Volume = 2 * value;
        }

        public static void StopLoop()
        {
            lock (pause_lock)
            {
                if (LoopsPaused == false)
                {
                    LoopsPaused = true;
                    foreach (var loop in Loops)
                    {
                        loop.Paused = true;
                        loop.Position = 0;
                    }
                }
            }
        }

        public static void StartLoop()
        {
            lock (pause_lock)
            {
                if (LoopsPaused == true)
                {
                    LoopsPaused = false;
                    foreach (var loop in Loops)
                    {
                        loop.Paused = false;
                    }
                }
            }
        }

        private static float[] ResizeAudio(float[] audio, int new_length)
        {
            float[] new_audio = new float[new_length];
            int length_diff = audio.Length - new_length;
            if (length_diff > 0)
            { // Audio must be clipped
                Array.Copy(audio, 0, new_audio, 0, new_length);
            }
            else
            { // Audio must be zero padded
                Array.Copy(audio, 0, new_audio, 0, audio.Length);
            }
            return new_audio;
        }

        public static void SetLoopLength(int length)
        {
            if (LoopLength == 0)
            {
                LoopLength = length;
            }
        }

        public static int GetLoopLength()
        {
            return LoopLength;
        }

        public static void AddLoop(float[] audio)
        {
            if (LoopLength == 0)
            { // First loop to be added
                LoopLength = audio.Length;
                LoopSampleProvider loop = new LoopSampleProvider(new CachedSoundSampleProvider(new CachedSound(audio, SAMPLE_FORMAT)));
                Loops.Add(loop);
                LoopMixer.AddMixerInput(loop);
            }
            else
            { // Not first loop: check length, clip if necessary, and set loop on correct position
                if (audio.Length != LoopLength)
                {
                    audio = ResizeAudio(audio, LoopLength);
                    Debug.Log("Succesfully resized audio");
                }
                LoopSampleProvider loop = new LoopSampleProvider(new CachedSoundSampleProvider(new CachedSound(audio, SAMPLE_FORMAT)));
                loop.Position = Loops[Loops.Count - 1].Position;
                Loops.Add(loop);
                LoopMixer.AddMixerInput(loop);
            }
            // TODO: Volume control
            // ...
        }

        public static void PlayClickSound()
        {
            var click = new VolumeSampleProvider(new CachedSoundSampleProvider(ClickSound));
            click.Volume = ClickVolume;
            Mixer?.AddMixerInput(click);
        }

        public static void AddPlayer(int id)
        {
            var b = new BufferedSampleProvider(SAMPLE_FORMAT);
            b.BufferLength = BUFFER_SIZE;
            b.DiscardOnBufferOverflow = true;
            PlayerAudio.Add(id, b);

            PlayerMixer.AddMixerInput(b);
        }

        public static void PlayPlayerAudio(float[] audio, int count, int player_id)
        {
            if (allowPlayerAudio)
            {
                BufferedSampleProvider b;
                if (PlayerAudio.TryGetValue(player_id, out b))
                {
                    b.AddSamples(audio, 0, count);
                }
            }
        }

        private static void OnAsioOutAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            // Get samples
            e.GetAsInterleavedSamples(float_buffer);
            int amount_samples = e.SamplesPerBuffer * e.InputBuffers.Length;
            // Play to self
            BufferedSampleProvider b;
            if (PlayerAudio.TryGetValue(-1, out b))
            {
                b.AddSamples(float_buffer, 0, amount_samples);
            }
            // Send to peers
            if (!LoopRecorder.IsRecording())
                SessionManager.SendAudioToPeers(float_buffer, amount_samples);
            // Send to loop recorder
            LoopRecorder.HandleAudio(float_buffer, amount_samples);
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
    }
}
