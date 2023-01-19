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
    /// Interaction logic for Path_Dialog.xaml
    /// </summary>
    public partial class Path_Dialog : Window
    {
        private ScanMeasurement measurement;
        public ScanMeasurement.ScanDirection Direction
        {
            get
            {
                if ((bool)radio_down.IsChecked)
                {
                    return ScanMeasurement.ScanDirection.Down;
                }

                if ((bool)radio_left.IsChecked)
                {
                    return ScanMeasurement.ScanDirection.Left;
                }

                if ((bool)radio_up.IsChecked)
                {
                    return ScanMeasurement.ScanDirection.Up;
                }

                if ((bool)radio_right.IsChecked)
                {
                    return ScanMeasurement.ScanDirection.Right;
                }

                return ScanMeasurement.ScanDirection.Down;
            }
        }
        public bool IsWholeGroup
        {
            get
            {
                return (bool)checkbox_group.IsChecked;
            }
        }
        public int SegmentGroupIndex { get { return int.Parse(TextBox_GroupIndex.Text); } }
        public int SegmentIndex { get { return int.Parse(TextBox_SegmentIndex.Text); } }
        public int xSegs { get { try { return int.Parse(TextBox_xSegs.Text); } catch { return 2; } } }
        public int ySegs { get { try { return int.Parse(TextBox_ySegs.Text); } catch { return 2; } } }

        public Path_Dialog(Measurement m)
        {
            InitializeComponent();
            measurement = (ScanMeasurement)m;
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWholeGroup)
            {
                measurement.SetScanPath(SegmentGroupIndex, SegmentIndex, Direction, xSegs, ySegs);
            }
            else
            {
                for (int i = 0; i < measurement.Surface.SegmentGroups[SegmentGroupIndex].Segments.Length; i++)
                {
                    measurement.SetScanPath(SegmentGroupIndex, i, Direction, xSegs, ySegs);
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
