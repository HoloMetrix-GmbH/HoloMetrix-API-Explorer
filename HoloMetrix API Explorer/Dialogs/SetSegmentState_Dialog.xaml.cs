using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HoloMetrix.Net.Remote.SoundIntensity;

namespace HoloMetrix_API_Explorer.Dialogs
{
    /// <summary>
    /// Interaction logic for SetSegmentState_Dialog.xaml
    /// </summary>
    public partial class SetSegmentState_Dialog : Window
    {
        private ScanMeasurement measurement;

        public SetSegmentState_Dialog(Measurement m)
        {
            InitializeComponent();
            combobox_state.ItemsSource = Enum.GetValues(typeof(ScanMeasurement.IndicatorState)).Cast<ScanMeasurement.IndicatorState>();
            measurement = (ScanMeasurement)m;
        }

        private void SetSegmentState(object sender, RoutedEventArgs e)
        {
            int mi;
            if ((bool)radio_1.IsChecked)
            {
                mi = 0;
            }
            else
            {
                mi = 1;
            }
            measurement.SetSegmentState(measurement.CurrentSegment[0], measurement.CurrentSegment[1], mi, (ScanMeasurement.IndicatorState)combobox_state.SelectedItem);
        }
    }
}
