using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Results_Dialog.xaml
    /// </summary>
    public partial class Results_Dialog : Window
    {
        private DiscreteMeasurement Measurement;

        public Results_Dialog(DiscreteMeasurement measurement)
        {
            InitializeComponent();
            Measurement = measurement;
            TextBox_GroupIndex.Text = measurement.CurrentSegment[0].ToString();
            TextBox_SegmentIndex.Text = measurement.CurrentSegment[1].ToString();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void SetResult_Click(object sender, RoutedEventArgs e)
        {
            var gi = int.Parse(TextBox_GroupIndex.Text);
            var si = int.Parse(TextBox_SegmentIndex.Text);
            Measurement.SetResult(gi, si, float.Parse(TextBox_Result.Text,System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), TextBox_Result.Text, true, (bool)CheckBox_IsNegative.IsChecked);
        }

        private void TextBox_GroupIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_SegmentIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_Result_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void SetAllResults_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();

            for (int i = 0; i < groups.Length; i++)
            {
                if(Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
                {
                    if(groups[i] == null)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                        continue;
                    }

                    for(int j = 0; j < groups[i].Segments.Length; j++)
                    {
                        float rand = (float)(r.NextDouble() * 200f - 100f);
                        results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                    }
                }
                Measurement.SetResults(i, results, false);
            }
        }

        private void SetAllResultsOld_Click(object sender, RoutedEventArgs e)
        {
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();

            for (int i = 0; i < groups.Length; i++)
            {
                List<MeasurementResult> results = new List<MeasurementResult>();

                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    continue;
                }

                if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
                {
                    for (int j = 0; j < groups[i].Segments.Length; j++)
                    {
                        float rand = (float)(r.NextDouble() * 200f - 100f);

                        results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                    }
                }
                
                foreach (var res in results)
                {
                    Measurement.SetResult(i, res, false);
                }
            }


        }

        private void FrontResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 0;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }

        private void TopResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 1;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }

        private void RightResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 3;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }

        private void LeftResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 2;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }

        private void BottomResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 4;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }

        private void BackResult_Click(object sender, RoutedEventArgs e)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var groups = Measurement.Surface.SegmentGroups;

            System.Diagnostics.Debug.WriteLine("groups length is " + groups.Length);

            Random r = new Random();
            int i = 5;
            if (Measurement.Surface.SurfaceStates[i] == SurfaceState.Measure)
            {
                if (groups[i] == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Group {0} is null", i));
                    return;
                }

                for (int j = 0; j < groups[i].Segments.Length; j++)
                {
                    float rand = (float)(r.NextDouble() * 200f - 100f);
                    results.Add(new MeasurementResult(j, rand, rand.ToString("0.0") + " dB(A)", false));
                }
            }
            Measurement.SetResults(i, results, false);
        }
    }
}
