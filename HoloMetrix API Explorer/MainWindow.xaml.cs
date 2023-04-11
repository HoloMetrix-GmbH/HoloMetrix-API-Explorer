using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

using HoloMetrix.Net.Remote;
using HoloMetrix.Net.Remote.SoundIntensity;
using HoloMetrix.Net.Utilities;
using HoloMetrix_API_Explorer.Dialogs;

namespace HoloMetrix_API_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BluetoothDevice device;
        SoundIntensityApp siApp;
        CuboidObject dut;
        CuboidSurface surface;
        Measurement measurement;

        public MainWindow()
        {
            InitializeComponent();
            var debugOut = new DebugOutput(txtbox_log);
            Console.SetOut(debugOut);
            TextWriterTraceListener debugListener = new TextWriterTraceListener(debugOut);
            Debug.Listeners.Add(debugListener);
            RemoteSession.DeveloperKey = GetLicense("K:\\HoloMetrix\\GIT\\API\\API_Explorer\\license.txt");
            if (string.IsNullOrEmpty(RemoteSession.DeveloperKey))
            {
                Log("Developer Key not found.");
            }
            else
            {
                Log("Developer key registered.  " + RemoteSession.DeveloperKey);
            }

            Logger.NewLog += Log;

            GeneralButtons.Visibility = Visibility.Hidden;
            MeasurementButtons.Visibility = Visibility.Hidden;
            ScanButtons.Visibility = Visibility.Collapsed;
            DiscreteButtons.Visibility = Visibility.Collapsed;
        }

        private void Logger_NewLog(object sender, LogEventArgs e)
        {
            Log(e.Message);
        }

        private string GetLicense(string licensePath)
        {
            StreamReader reader = new StreamReader(licensePath);
            return reader.ReadToEnd();
        }

        private void Connect_DevicePicker(object sender, RoutedEventArgs e)
        {
            ConnectAsync();
            //remoteSession = Bluetooth.TryConnectToHoloMetrixHub();
            //if(remoteSession != null)
            //{
            //    remoteSession.RemoteChanged_AppStatus += RemoteSession_RemoteChanged_AppStatus;
            //    remoteSession.Battery_ReportUpdated += RemoteSession_Battery_ReportUpdated;
            //}
        }

        private async void ConnectAsync()
        {
            await Task.Run(() =>
            {
                Bluetooth.TryConnectToHoloMetrixHub();
            });
        }

        private void BluetoothConnection_ConnectionLost(object sender, EventArgs e)
        {
            if (RemoteSession.Instance != null)
            {
                RemoteSession.Instance.RemoteChanged_AppStatus -= RemoteSession_RemoteChanged_AppStatus;
                RemoteSession.Instance.Battery_ReportUpdated -= RemoteSession_Battery_ReportUpdated;
            }
            siApp = null;
            //txt_connectionState.Text = "Connection Lost";
        }

        private async void BluetoothConnection_ConnectionEstablished(object sender, ConnectionEstablishedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                txt_connectionState.Text = "Connected";
            });
            
            Log("Connected");

            Debug.WriteLine("Trying to launch app");
            siApp = await RemoteSession.Instance.TryLaunchAppAsync<SoundIntensityApp>();
            //siApp = RemoteSession.Instance.TryLaunchApp<SoundIntensityApp>();
            if (siApp != null)
            {
                Dispatcher.Invoke(() =>
                {
                    GeneralButtons.Visibility = Visibility.Visible;
                });
                MeasurementMethod method = MeasurementMethod.Scan;
                measurement = siApp.Setup(method, new CuboidObject(300, 300, 300), new CuboidSurface(500, 500, 500, 3, 3, 3, new SurfaceState[6] { SurfaceState.Measure, SurfaceState.Measure, SurfaceState.Measure, SurfaceState.Measure, SurfaceState.Solid, SurfaceState.Ignore }));
                measurement.SelectNextSegment(0, 1);
                ((ScanMeasurement)measurement).SetScanPath(0, 1, ScanMeasurement.ScanDirection.Down, 3, 3);

                measurement.RejectResultRequested += Measurement_RejectResultRequested;
                measurement.SelectSegmentRequested += Measurement_SelectSegmentRequested;
                measurement.StartMeasurementRequested += Measurement_StartMeasurementRequested;
                measurement.PositionReceived += Measurement_PositionReceived;
                Debug.WriteLine("Subscribed to events");
                Log("Subscribed to events");

                Dispatcher.Invoke(() =>
                {

                    MeasurementButtons.Visibility = Visibility.Visible;

                    if (method == MeasurementMethod.Discrete)
                    {
                        DiscreteButtons.Visibility = Visibility.Visible;
                        ScanButtons.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        DiscreteButtons.Visibility = Visibility.Collapsed;
                        ScanButtons.Visibility = Visibility.Visible;
                    }
                });

            }
            else
            {
                Debug.WriteLine("siApp is null");
            }

            isConnecting = false;
        }

        private void GetDevices(object sender, RoutedEventArgs e)
        {
            GetDevices_Dialog devicesDialog = new GetDevices_Dialog();
            if (devicesDialog.ShowDialog() == true)
            {
                device = devicesDialog.SelectedDevice;
                Debug.WriteLine("Selected device: " + device.DeviceInfo.DeviceName);
                Log("Selected device: " + device.DeviceInfo.DeviceName);
            }
        }

        private void ConnectToDevice(object sender, RoutedEventArgs e)
        {
            if (device != null)
            {
                device.TryConnectToService();
            }
        }

        private void RemoteSession_Battery_ReportUpdated(object sender, DevicePowerEventArgs e)
        {
            Debug.WriteLine("Updating battery charge.");
            Log("Updating battery charge.");
            Dispatcher.Invoke(new Action(() =>
            {
                txt_power.Text = e.RemainingCharge.ToString() + " %";
            }
            ));
        }

        private void RemoteSession_RemoteChanged_AppStatus(object sender, RemoteChangedEventArgs<RemoteAppStatus> e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                txt_appState.Text = e.Value.ToString();
            }
            ));
        }

        private void SiApp_RemoteChanged_ConnectionStatus(object sender, RemoteChangedEventArgs<bool> e)
        {
            Debug.WriteLine("Reacting to Connection Status Changed.");
            Log("Reacting to Connection Status Changed.");
            Dispatcher.Invoke(new Action(() =>
            {
                txt_connectionState.Text = e.Value.ToString();
            }
            ));
        }

        private void SiApp_RemoteChanged_AppStatus(object sender, RemoteChangedEventArgs<SoundIntensityApp.AppStatus> e)
        {
            Debug.WriteLine("Reacting to App Status Changed.");
            Log("Reacting to App Status Changed.");
            Dispatcher.Invoke(new Action(() =>
            {
                txt_siAppState.Text = e.Value.ToString();
            }
            ));
        }

        private async void LaunchApp(object sender, RoutedEventArgs e)
        {
            if(RemoteSession.Instance != null)
            {
                /*
                await Task.Run(() =>
                {
                    siApp = remoteSession.TryLaunchApp<SoundIntensityApp>();
                });

                if (siApp != null)
                {
                    siApp.AppStatusChange += SiApp_RemoteChanged_AppStatus;
                    siApp.ConnectionStatusChange += SiApp_RemoteChanged_ConnectionStatus;
                    this.Dispatcher.Invoke(() => { GeneralButtons.Visibility = Visibility.Visible; });
                }
                */

                siApp = await RemoteSession.Instance.TryLaunchAppAsync<SoundIntensityApp>();
                Debug.WriteLine("siApp is null?" + (siApp == null).ToString());
                if (siApp != null)
                {
                    siApp.AppStatusChange += SiApp_RemoteChanged_AppStatus;
                    siApp.ConnectionStatusChange += SiApp_RemoteChanged_ConnectionStatus;
                    this.Dispatcher.Invoke(() => { GeneralButtons.Visibility = Visibility.Visible; });
                }
            }
        }

        private void Setup(object sender, RoutedEventArgs e)
        {
            if(siApp != null)
            {
                Setup_Dialog setupDialog = new Setup_Dialog(dut, surface);
                if(setupDialog.ShowDialog() == true)
                {
                    dut = setupDialog.Object;
                    surface = setupDialog.Surface;
                    if (string.IsNullOrEmpty(setupDialog.AnchorPath))
                    {
                        measurement = siApp.Setup(setupDialog.Method, dut, surface);
                    }
                    else
                    {
                        //var name = Path.GetFileNameWithoutExtension(setupDialog.AnchorPath);
                        var name = setupDialog.AnchorPath;
                        //var data = File.ReadAllBytes(setupDialog.AnchorPath);
                        measurement = siApp.LoadSetup(setupDialog.Method, dut, surface, name);
                    }
                    measurement.SelectSegmentRequested += Measurement_SelectSegmentRequested;
                    measurement.StartMeasurementRequested += Measurement_StartMeasurementRequested;
                    measurement.RejectResultRequested += Measurement_RejectResultRequested;
                    measurement.PositionReceived += Measurement_PositionReceived;
                    Debug.WriteLine("Subscribed to events");
                    Log("Subscribed to events");

                    if (setupDialog.TestInit)
                    {
                        siApp.AppStatusChange += SiApp_AppStatusChange;
                    }

                    MeasurementButtons.Visibility = Visibility.Visible;
                    
                    if (setupDialog.Method == MeasurementMethod.Discrete)
                    {
                        DiscreteButtons.Visibility = Visibility.Visible;
                        ScanButtons.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        DiscreteButtons.Visibility = Visibility.Collapsed;
                        ScanButtons.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                Debug.WriteLine("App is null");
            }
        }

        private void Measurement_RejectResultRequested(object sender, SegmentEventArgs e)
        {
            Debug.WriteLine("Reject Result received. Rejecting result.");
            Log("Reject Result received. Rejecting result.");

            measurement.ClearResult(e.SegmentGroupIndex, e.SegmentIndex);
        }

        private void SiApp_AppStatusChange(object sender, RemoteChangedEventArgs<SoundIntensityApp.AppStatus> e)
        {
            switch (e.Value)
            {
                case SoundIntensityApp.AppStatus.Start:
                    break;
                case SoundIntensityApp.AppStatus.Initial:
                    break;
                case SoundIntensityApp.AppStatus.ObjectPlacement:
                    break;
                case SoundIntensityApp.AppStatus.ObjectAdjustment:
                    break;
                case SoundIntensityApp.AppStatus.SurfaceSetup:
                    break;
                case SoundIntensityApp.AppStatus.GeneratingSurface:
                    break;
                case SoundIntensityApp.AppStatus.SegmentAdjustment:
                    break;
                case SoundIntensityApp.AppStatus.Measurement:
                    TransferTestResults();
                    break;
                case SoundIntensityApp.AppStatus.Measuring:
                    break;
                case SoundIntensityApp.AppStatus.Analysis:
                    break;
            }
        }

        private void TransferTestResults()
        {
            Debug.WriteLine("Transfering initial data.");
            Log("Transfering initial data.");
            List<GradientStop> stops = new List<GradientStop>();

            for (int i = 0; i < 255 * 3; i++)
            {
                stops.Add(
                    new GradientStop(
                        System.Windows.Media.Color.FromRgb(
                            (byte)Math.Min(i - 510, 255), 
                            (byte)Math.Min(i - 255, 255), 
                            (byte)Math.Min(i, 255)), i / (255 *3)));
            }
            ((DiscreteMeasurement)measurement).SetAnalysisGradient(new ColorGradient(stops));
            ((DiscreteMeasurement)measurement).ClampGradient(true, false, -100.0f, 100.0f);

            for (int i = 0; i < 6; i++)
            {
                if (measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
                {
                    for (int j = 0; j < measurement.Surface.SegmentGroups[i].Segments.Length; j++)
                    {
                        float val = (float)(30.0 + i + (j + i + 1) * 10 * Math.Sin(i + j));
                        var result = new MeasurementResult(j, val, val.ToString("0.0") + " dB", false);
                        ((DiscreteMeasurement)measurement).SetResult(i, result, false);
                    }
                }
            }

            measurement.SelectNextSegment(0, 2);
        }

        private void Measurement_PositionReceived(object sender, AnchorReceivedEventArgs e)
        {
            Debug.WriteLine("Position received in demo: " + e.ID);
            Log("Position received in demo: " + e.ID);
            /*
            try
            {
                string filePath = "K:/ARA/tmp/" + e.ID + ".hmxanchor";
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(e.Data, 0, e.Data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception caught in process: {0}", ex);
                return;
            }
            */
        }

        private void Measurement_StartMeasurementRequested(object sender, MeasurementEventArgs e)
        {
            Debug.WriteLine("Start Measurement Requested");
            Log("Start Measurement Requested");
        }

        private void Measurement_SelectSegmentRequested(object sender, SegmentEventArgs e)
        {
            if (!measurement.Surface.SegmentGroups[e.SegmentGroupIndex].Segments[e.SegmentIndex].IsComplete)
            {
                measurement.SelectNextSegment(e.SegmentGroupIndex, e.SegmentIndex);
            }
            else
            {
                measurement.SelectCurrentSegment(e.SegmentGroupIndex, e.SegmentIndex);
            }
        }

        private void SelectSegment(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                SelectSegment_Dialog selectDialog = new SelectSegment_Dialog(measurement, measurement.CurrentSegment[0], measurement.CurrentSegment[1]);
                selectDialog.ShowDialog();
            }
        }

        private void SetGradient(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                Gradient_Dialog gradientDialog = new Gradient_Dialog();
                if (gradientDialog.ShowDialog() == true)
                {
                    ((DiscreteMeasurement)measurement).SetAnalysisGradient(gradientDialog.Gradient);
                    ((DiscreteMeasurement)measurement).ClampGradient(gradientDialog.IsClamped, gradientDialog.IsButterfly, gradientDialog.Min, gradientDialog.Max);
                }
            }
        }

        private void StartMeasurement(object sender, RoutedEventArgs e)
        {
            if(measurement != null)
            {
                Measuring_Dialog measuringDialog = new Measuring_Dialog(measurement);
                measuringDialog.ShowDialog();
            }
        }

        private void SetResult(object sender, RoutedEventArgs e)
        {
            if(measurement != null)
            {
                Results_Dialog resultsDialog = new Results_Dialog(measurement);
                resultsDialog.ShowDialog();
            }
        }

        private void SendStatusMessages(object sender, RoutedEventArgs e)
        {
            if(measurement != null)
            {
                List<StatusMessage> messages = new List<StatusMessage>();
                messages.Add(new StatusMessage(StatusMessage.Severity.Green, "First Status"));
                messages.Add(new StatusMessage(StatusMessage.Severity.Yellow, "Second Status"));
                messages.Add(new StatusMessage(StatusMessage.Severity.Red, "Third Status"));
                measurement.SetStatusMessages(messages);
            }
        }

        public void Log(string s)
        {
            txtbox_log.Dispatcher.Invoke(new Action(() =>
            {
                txtbox_log.AppendText(s + "\n");
            }));
        }

        public void Log(object sender, LogEventArgs e)
        {
            Log(e.Message);
        }

        private void ConnectLaunchSetup(object sender, RoutedEventArgs e)
        {
            dut = new CuboidObject(100f, 100f, 100f);
            surface = new CuboidSurface
                (
                    100f, 100f, 100f,
                    2, 2, 2,
                    new SurfaceState[6]
                        {
                        SurfaceState.Measure,
                        SurfaceState.Measure,
                        SurfaceState.Measure,
                        SurfaceState.Measure,
                        SurfaceState.Solid,
                        SurfaceState.Measure
                        }
                );

            BluetoothConnection.ConnectionEstablished -= BluetoothConnection_ConnectionEstablished;
            BluetoothConnection.ConnectionEstablished += BluetoothConnection_ConnectionEstablished;
            Bluetooth.TryConnectToHoloMetrixHub();
        }

        private void FinishMeasuring(object sender, RoutedEventArgs e)
        {
            if(measurement != null)
            {
                measurement.SetFinished(true);
            }
        }        
        private void ResumeMeasuring(object sender, RoutedEventArgs e)
        {
            if(measurement != null)
            {
                measurement.SetFinished(false);
            }
        }

        private void SendTestData(object sender, RoutedEventArgs e)
        {
            if(RemoteSession.Instance != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.DefaultExt = ".hmxanchor";
                //openFileDialog.Filter = "HoloMetrix Anchor (.hmxanchor)|*.hmxanchor";
                string filePath;
                byte[] data = null;

                if (openFileDialog.ShowDialog() == true)
                {
                    filePath = openFileDialog.FileName;
                    data = File.ReadAllBytes(filePath);
                }

                //Bluetooth.SendMessage(RemoteSession.BluetoothConnection, "test", data);

                SoundIntensityApp.SendAnchorTest(data, "MyAnchorID");
            }
        }

        private void CloseSession(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => { 
                GeneralButtons.Visibility = Visibility.Hidden;
                MeasurementButtons.Visibility = Visibility.Hidden;
                ScanButtons.Visibility = Visibility.Collapsed;
                DiscreteButtons.Visibility = Visibility.Collapsed;
            });
            measurement = null;

            if (RemoteSession.Instance != null)
            {
                RemoteSession.Instance.CloseSession();
            }
        }

        bool isSearching = false;
        bool isConnecting = false;
        bool deviceFound = false;

        private void BeginDiscovery(object sender, RoutedEventArgs e)
        {
            if (isSearching)
            {
                Debug.WriteLine("Search still running!");
                return;
            }
            if (isConnecting)
            {
                Debug.WriteLine("Still trying to connect.");
                return;
            }

            deviceFound = false;
            isSearching = true;
            Debug.WriteLine("Beginning Discovery");
            Bluetooth.DeviceFound -= Bluetooth_DeviceFound;
            Bluetooth.DeviceFound += Bluetooth_DeviceFound;

            Bluetooth.DeviceDiscoveryComplete -= Bluetooth_DeviceDiscoveryComplete;
            Bluetooth.DeviceDiscoveryComplete += Bluetooth_DeviceDiscoveryComplete;

            Bluetooth.DeviceDiscoveryCanceled -= Bluetooth_DeviceDiscoveryCanceled;
            Bluetooth.DeviceDiscoveryCanceled += Bluetooth_DeviceDiscoveryCanceled;

            Bluetooth.GetDevicesAsync();
        }

        private void Bluetooth_DeviceDiscoveryCanceled(object sender, DeviceDiscoveryEventArgs e)
        {
            Debug.WriteLine("Search Cancelled");
            isSearching = false;
        }

        private void Bluetooth_DeviceDiscoveryComplete(object sender, DeviceDiscoveryEventArgs e)
        {
            Debug.WriteLine("Search Complete");
            isSearching = false;
        }

        private void Bluetooth_DeviceFound(object sender, DeviceDiscoveryEventArgs e)
        {
            if (deviceFound)
            {
                return;
            }

            deviceFound = true;
            isSearching = false;

            if (RemoteSession.Instance != null)
            {
                return;
            }

            Bluetooth.CancelDeviceDiscovery();
            Debug.WriteLine("API_Explorer: Device Found. -- " + e.Device.DeviceInfo.DeviceName);
            isConnecting = true;

            BluetoothConnection.ConnectionEstablished -= BluetoothConnection_ConnectionEstablished;
            BluetoothConnection.ConnectionEstablished += BluetoothConnection_ConnectionEstablished;

            e.Device.TryConnectToService();
        }

        private void ClearSegmentResult(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                measurement.ClearResult(measurement.CurrentSegment[0], measurement.CurrentSegment[1]);
            }

        }

        private void SetScanPath(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                Path_Dialog pathDialog = new Path_Dialog(measurement);
                pathDialog.ShowDialog();
            }
        }

        private void SetSegmentState(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                SetSegmentState_Dialog stateDialog = new SetSegmentState_Dialog(measurement);
                stateDialog.ShowDialog();
            }
        }

        private void StopMeasurement(object sender, RoutedEventArgs e)
        {
            if (measurement != null)
            {
                measurement.StopMeasurement();
            }
        }

        private void CancelDiscovery(object sender, RoutedEventArgs e)
        {
            Bluetooth.CancelDeviceDiscovery();
        }
    }
}
