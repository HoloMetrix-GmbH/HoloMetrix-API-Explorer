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
    /// Interaction logic for Measuring_Dialog.xaml
    /// </summary>
    public partial class Measuring_Dialog : Window
    {
        private Measurement Measurement;

        public Measuring_Dialog(Measurement measurement)
        {
            InitializeComponent();
            Measurement = measurement;
            TextBox_GroupIndex.Text = measurement.CurrentSegment[0].ToString();
            TextBox_SegmentIndex.Text = measurement.CurrentSegment[1].ToString();
        }

        private void Measure_Click(object sender, RoutedEventArgs e)
        {
            Measurement.SelectSegment(int.Parse(TextBox_GroupIndex.Text), int.Parse(TextBox_SegmentIndex.Text));
            Measurement.StartMeasurement(int.Parse(TextBox_Duration.Text));
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void TextBox_GroupIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_SegmentIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_Duration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }
    }
}
