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

using HoloMetrix.Net.Utilities;

namespace HoloMetrix_API_Explorer.Dialogs
{
    /// <summary>
    /// Interaction logic for Gradient_Dialog.xaml
    /// </summary>
    public partial class Gradient_Dialog : Window
    {
        public Gradient_Dialog()
        {
            InitializeComponent();
        }

        private void Gradient_Click(object sender, RoutedEventArgs e)
        {
            Gradient = FromGradientBrush((LinearGradientBrush)((Button)sender).Background);
            if(TextBox_Min.Text == "Min" || string.IsNullOrWhiteSpace(TextBox_Min.Text))
            {
                Min = float.NaN;
            }
            else
            {
                Min = float.Parse(TextBox_Min.Text);
            }
            if (TextBox_Max.Text == "Max" || string.IsNullOrWhiteSpace(TextBox_Max.Text))
            {
                Max = float.NaN;
            }
            else
            {
                Max = float.Parse(TextBox_Max.Text);
            }
            this.DialogResult = true;
        }

        private ColorGradient FromGradientBrush(LinearGradientBrush brush)
        {
            List<HoloMetrix.Net.Utilities.GradientStop> stops = new List<HoloMetrix.Net.Utilities.GradientStop>();
            foreach(var stop in brush.GradientStops)
            {
                stops.Add(new HoloMetrix.Net.Utilities.GradientStop(stop.Color, stop.Offset));
            }
            return new ColorGradient(stops);
        }

        public ColorGradient Gradient { get; private set; }
        public float Min { get; private set; }
        public float Max { get; private set; }
        public bool IsClamped { get { return (bool)CheckBox_Clamp.IsChecked; } }
        public bool IsButterfly { get { return (bool)CheckBox_Butterfly.IsChecked; } }

        private void TextBox_Min_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
            Console.WriteLine(e.Text + e.Handled);

        }

        private void TextBox_Max_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
            Console.WriteLine(e.Text + e.Handled);
        }
    }
}
