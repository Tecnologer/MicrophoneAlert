using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace MicrophoneAlert.net
{
    public class AudioDevices
    {
        #region Singleton
        private static readonly object _syncRoot = new object();
        private static AudioDevices instance;
        public static AudioDevices Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    return instance ?? (instance = new AudioDevices());
                }
            }
        }
        #endregion

        private string selectedDeviceId;
        private MMDevice selectedDevice;
        private SemaphoreSlim semaphore; 
        public AudioDevices()
        {
            semaphore = new SemaphoreSlim(1, 1);
            SelectedDevice = null;
            Limit = 70;
            SelectedDeviceId = null;
            Devices = new List<InputDevice>();
        }

        public List<InputDevice> Devices { get; private set; }
        public Dispatcher Dispatcher { private get; set; }

        public string SelectedDeviceId
        {
            get
            {
                return selectedDeviceId;
            }
            set
            {
                if (selectedDeviceId == value) return;
                selectedDeviceId = value;
                UpdateListDevices();
            }
        }

        public int Limit { get; set; }

        public MMDevice SelectedDevice 
        { 
            get => selectedDevice; 
            set 
            {
                selectedDevice = value;
                if (selectedDevice != null && selectedDevice.ID != selectedDeviceId)
                {
                    selectedDeviceId = selectedDevice.ID;
                }
            } 
        }

        public void UpdateListDevices()
        {
            semaphore.WaitAsync();
            try
            {
                var enumerator = new MMDeviceEnumerator();
                var originalDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                Devices.Clear();
                Devices.AddRange(originalDevices.ToList().Select(d => new InputDevice(d.ID, d.FriendlyName)).ToList());

                if (string.IsNullOrWhiteSpace(selectedDeviceId))
                {
                    SelectedDevice = originalDevices.First();
                }
                else
                {

                    SelectedDevice = enumerator.GetDevice(SelectedDeviceId);
                }

#if DEBUG
                foreach (var wasapi in originalDevices)
                {
                    Debug.WriteLine($"{wasapi.ID}:{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}");
                }
#endif
            }
            finally
            {
                semaphore.Release();
            }
        }

        public float GetVolume()
        {
            if (SelectedDevice == null || SelectedDevice.ID != SelectedDeviceId)
            {
                UpdateListDevices();
            }

            if (selectedDevice == null) return 0;

            return SelectedDevice != null ? SelectedDevice.AudioMeterInformation.MasterPeakValue * 100 : 0;
        }
    }
}
