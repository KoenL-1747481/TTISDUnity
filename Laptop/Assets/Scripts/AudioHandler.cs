using System.Collections.Generic;
using UnityEngine;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace TTISDProject
{
    class AudioHandler : MonoBehaviour
    {
        private static readonly int SAMPLE_RATE = 44100;
        private static readonly int INPUT_CHANNELS = 2;
        private static readonly int OUTPUT_CHANNELS = 2;
        private static readonly int BUFFER_SIZE = 1024 * 32;
        private static readonly WaveFormat SAMPLE_FORMAT = WaveFormat.CreateIeeeFloatWaveFormat(SAMPLE_RATE, OUTPUT_CHANNELS);

        private static AsioOut AsioDriver;
        private static BufferedSampleProvider[] PlayerAudio = new BufferedSampleProvider[Server.MAX_PLAYERS];
        private static CachedSound ClickSound = new CachedSound("Assets/Audio/click.wav");
        private static MixingSampleProvider Mixer;

        private static List<LoopSampleProvider> Loops = new List<LoopSampleProvider>();
        private static int LoopLength = 0;
        private static bool LoopsPaused = false;
        private static object pause_lock = new object();

        private static float[] float_buffer = new float[1024 * 16];

        // Start is called before the first frame update
        void Start()
        {
            StartAudioHandler(AsioOut.GetDriverNames()[0]);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnApplicationQuit()
        {
            Dispose();
        }

        public static void Dispose()
        {
            AsioDriver?.Dispose();
            foreach (var buff in PlayerAudio)
            {
                buff?.ClearBuffer();
            }
            Mixer?.RemoveAllMixerInputs();
            Loops.Clear();
            LoopLength = 0;
            LoopsPaused = false;
        }
        public static void StartAudioHandler(string driverName)
        {
            Dispose();

            Mixer = new MixingSampleProvider(SAMPLE_FORMAT);
            Mixer.MixerInputEnded += Mixer_MixerInputEnded;
            // Create input buffers for player audio
            for (int i = 0; i < PlayerAudio.Length; i++)
            {
                PlayerAudio[i] = new BufferedSampleProvider(SAMPLE_FORMAT);
                PlayerAudio[i].BufferLength = BUFFER_SIZE;
                PlayerAudio[i].DiscardOnBufferOverflow = true;
                Mixer.AddMixerInput(PlayerAudio[i]);
            }
            var compressor = new SimpleCompressorEffect(Mixer);
            compressor.Enabled = true;
            compressor.MakeUpGain = 0;
            //compressor.Attack = 0.1;
            //compressor.Ratio = 10;
            //compressor.Threshold = -50;
            //compressor.Release = 1;*/
            var audioOut = new SampleToWaveProvider16(compressor);

            // Init AsioOut for recording and playback
            AsioDriver = new AsioOut(driverName);
            AsioDriver.AudioAvailable += OnAsioOutAudioAvailable;
            AsioDriver.InitRecordAndPlayback(audioOut, INPUT_CHANNELS, SAMPLE_RATE);
            AsioDriver.Play();
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

        public static void AddLoop(float[] audio)
        {
            if (LoopLength == 0)
            { // First loop to be added
                LoopLength = audio.Length;
                LoopSampleProvider loop = new LoopSampleProvider(new CachedSoundSampleProvider(new CachedSound(audio, SAMPLE_FORMAT)));
                Loops.Add(loop);
                Mixer.AddMixerInput(loop);
            }
            else
            { // Not first loop: check length, clip if necessary, and set loop on correct position
                if (audio.Length != LoopLength)
                {
                    audio = ResizeAudio(audio, LoopLength);
                    Console.WriteLine("Succesfully resized audio");
                }
                LoopSampleProvider loop = new LoopSampleProvider(new CachedSoundSampleProvider(new CachedSound(audio, SAMPLE_FORMAT)));
                loop.Position = Loops[0].Position;
                Loops.Add(loop);
                Mixer.AddMixerInput(loop);
                // TODO: Volume control
                // ...
            }
        }

        private static void Mixer_MixerInputEnded(object sender, SampleProviderEventArgs e)
        {
            //Console.WriteLine("Mixer input ended!");
        }

        public static void PlayClickSound()
        {
            Mixer?.AddMixerInput(new CachedSoundSampleProvider(ClickSound));
        }

        public static void PlayPlayerAudio(float[] audio, int count, int player_id)
        {
            if (player_id < Server.MAX_PLAYERS && player_id >= 0)
            {
                PlayerAudio[player_id]?.AddSamples(audio, 0, count);
            }
        }

        private static void OnAsioOutAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            // Get samples
            e.GetAsInterleavedSamples(float_buffer);
            int amount_samples = e.SamplesPerBuffer * e.InputBuffers.Length;
            // Play to self
            PlayerAudio[0].AddSamples(float_buffer, 0, amount_samples);
            // Send to peers
            //Client.SendAudioToPeers(float_buffer, amount_samples);
            // Send to loop recorder
            //LoopRecorder.HandleAudio(float_buffer, amount_samples);
        }
    }
}
