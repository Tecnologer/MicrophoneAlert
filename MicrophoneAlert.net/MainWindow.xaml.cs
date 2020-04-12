using System.ComponentModel;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace MicrophoneAlert.net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string decibelsValue;
        private SolidColorBrush backgroundColor;

        private Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        public SolidColorBrush BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                if (backgroundColor?.Color == value?.Color) return;
                backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public string DecibelsValue
        {
            get
            {
                return decibelsValue;
            }
            set
            {
                if (decibelsValue == value) return;
                decibelsValue = value;
                OnPropertyChanged(nameof(DecibelsValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertiesName)
        {
            foreach (var propertyName in propertiesName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false;
                var vol = AudioDevices.Instance.GetVolume();

                Dispatcher.Invoke(delegate
                {
                    DecibelsValue = ((int)vol).ToString();
                    var color = vol >= 70 ? Colors.Red : Colors.Lime;
                    BackgroundColor = new SolidColorBrush(color);
                });
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
