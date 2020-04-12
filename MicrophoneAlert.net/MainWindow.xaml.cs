using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using WinForm = System.Windows.Forms;

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

            TrayIcon();
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

        private void TrayIcon()
        {
            WinForm.NotifyIcon ni = new WinForm.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("./main.ico");
            ni.Visible = true;
            ni.ContextMenu = new WinForm.ContextMenu();
            ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("Configuration", Config_Click));
            //ni.ContextMenu.MenuItems.Add("-");
            //ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("About", About_Click));
            ni.ContextMenu.MenuItems.Add("-");
            ni.ContextMenu.MenuItems.Add(new WinForm.MenuItem("Exit", Exit_Click));            
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            Close();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            var config = new Configuration();
            config.ShowDialog();
        }

        private void About_Click(object sender, EventArgs e)
        {

        }
    }
}
