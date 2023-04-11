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
    /// Interaction logic for SelectSegment_Dialog.xaml
    /// </summary>
    public partial class SelectSegment_Dialog : Window
    {
        private Measurement measurement;
        public SelectSegment_Dialog(Measurement m, int groupIndex, int segmentIndex)
        {
            InitializeComponent();
            TextBox_GroupIndex.Text = groupIndex.ToString();
            TextBox_SegmentIndex.Text = segmentIndex.ToString();
            measurement = m;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TextBox_GroupIndex.SelectAll();
            TextBox_GroupIndex.Focus();
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            measurement.SelectNextSegment(SegmentGroupIndex, SegmentIndex);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public int SegmentGroupIndex { get { return int.Parse(TextBox_GroupIndex.Text); } }
        public int SegmentIndex { get { return int.Parse(TextBox_SegmentIndex.Text); } }

        private void TextBox_GroupIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_SegmentIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            measurement.SelectNextSegment(SegmentGroupIndex, SegmentIndex);
        }

        private void Button_Current_Click(object sender, RoutedEventArgs e)
        {
            measurement.SelectCurrentSegment(SegmentGroupIndex, SegmentIndex);
        }
    }
}
