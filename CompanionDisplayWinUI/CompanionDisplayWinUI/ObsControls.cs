using EmbedIO.Sessions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.Logging;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompanionDisplayWinUI
{
    internal class ObsControls
    {
        public OBSWebsocket obs = new OBSWebsocket()
        {
            WSTimeout = TimeSpan.FromSeconds(1),
        };
        public bool connectionSuccessful = false;
        public bool initialized = false;
        public delegate void HandleStupidEvents();
        public delegate void HandleStupidEvents2();
        public event HandleStupidEvents callUpdate;
        public event HandleStupidEvents2 callUpdateStats;
        public void connect()
        {
            try
            {
                obs.Connected += connectionstep2;
                obs.Disconnected += disconnect;
                obs.ConnectAsync(Globals.obsIP, Globals.obsPass);
            }
            catch (Exception ex)
            {
            }
        }
        private void reqStats()
        {
            while (connectionSuccessful)
            {
                try
                {
                    obsData = obs.GetStats();
                    outputStatus = obs.GetStreamStatus();
                    recStatus = obs.GetRecordStatus();
                    onUpdate();
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }
        private void disconnect(object sender, ObsDisconnectionInfo e)
        {
            connectionSuccessful = false;
        }
        protected virtual void onUpdate()
        {
            callUpdateStats?.Invoke(); // Pass EventArgs.Empty for no data
        }
        protected virtual void OnMyEvent()
        {
            callUpdate?.Invoke(); // Pass EventArgs.Empty for no data
        }
        private void connectionstep2(object sender, EventArgs e)
        {
            obs.CurrentProgramSceneChanged += updateSelection;
            Thread thread = new(reqStats);
            thread.Start();
            connectionSuccessful = true;
            updateSelection(sender, null);
        }

        private void updateSelection(object sender, ProgramSceneChangedEventArgs e)
        {
            try
            {
                currentSession = obs.GetCurrentProgramScene();
                scenes = obs.ListScenes().ToArray();
                OnMyEvent();
            }
            catch
            {
                connectionSuccessful = false;
            }
        }
        public ObsStats obsData = new ObsStats();
        public OutputStatus outputStatus = new OutputStatus();
        public RecordingStatus recStatus = new RecordingStatus();
        public void manualConnectReq()
        {
            try
            {
                obs.ConnectAsync(Globals.obsIP, Globals.obsPass);
            }
            catch
            {

            }
        }
        public SceneBasicInfo[] scenes;
        public string currentSession = "";
        public void setScene(string newScene)
        {
            obs.SetCurrentProgramScene(newScene);
        }
        public void StartStreaming()
        {
            obs.StartStream();
        }
        public void StartRecording()
        {
            obs.StartRecord();
        }
        public void StopStreaming()
        {
            obs.StopStream();
        }
        public void StopRecording()
        {
            obs.StopRecord();
        }
        public void pauseToggle()
        {
            obs.ToggleRecordPause();
        }
        public void cameraToggle()
        {
            obs.ToggleVirtualCam();
        }
        public void bufferToggle()
        {
            obs.ToggleReplayBuffer();
        }
        public void bufferSave()
        {
            obs.SaveReplayBuffer();
        }
        public bool micMute = false;
        public void micToggle()
        {
            foreach(var input in obs.GetInputList())
            {
                try
                {
                    obs.ToggleInputMute(input.InputName);
                }
                catch
                {

                }
                micMute = !micMute;
            }
        }
        public void Reconnect()
        {
            obs.Disconnect();
            connect();
        }
    }
}
