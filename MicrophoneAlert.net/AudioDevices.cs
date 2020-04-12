using NAudio.CoreAudioApi;
using System.Diagnostics;

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

        private MMDeviceCollection mMDevices;
        private string selectedDeviceId;
        public AudioDevices()
        {
            SelectedDevice = null;
            SelectedDeviceId = "{0.0.1.00000000}.{e08360f0-1cb0-4e44-87be-89fc924d260e}";
        }

        public MMDeviceCollection Devices { get => mMDevices; }

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

        public MMDevice SelectedDevice { get; private set; }

        public void UpdateListDevices()
        {
            var enumerator = new MMDeviceEnumerator();            
            mMDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            SelectedDevice = enumerator.GetDevice(SelectedDeviceId);
            foreach (var wasapi in mMDevices)
            {
                Debug.WriteLine($"{wasapi.ID}:{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}");
            }
        }

        public float GetVolume()
        {
            if(SelectedDevice == null || SelectedDevice.ID != SelectedDeviceId)
            {
                UpdateListDevices();
            }
            SelectedDevice.AudioEndpointVolume.Mute = false;
            return SelectedDevice != null ? SelectedDevice.AudioMeterInformation.MasterPeakValue * 100 : 0;
        }
    }
}
