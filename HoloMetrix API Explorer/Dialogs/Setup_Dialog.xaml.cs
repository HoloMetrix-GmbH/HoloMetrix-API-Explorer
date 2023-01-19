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
using Microsoft.Win32;

using HoloMetrix.Net.Remote.SoundIntensity;

namespace HoloMetrix_API_Explorer.Dialogs
{
    /// <summary>
    /// Interaction logic for Setup_Dialog.xaml
    /// </summary>
    public partial class Setup_Dialog : Window
    {
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TextBox_ObjX.Text = "0";
            TextBox_ObjY.Text = "0";
            TextBox_ObjZ.Text = "0";

            if (_obj != null)
            {
                TextBox_ObjX.Text = _obj.SizeX.ToString();
                TextBox_ObjY.Text = _obj.SizeY.ToString();
                TextBox_ObjZ.Text = _obj.SizeZ.ToString();
            }

            TextBox_SurfX.Text = "0";
            TextBox_SurfY.Text = "0";
            TextBox_SurfZ.Text = "0";

            TextBox_SegsX.Text = "1";
            TextBox_SegsY.Text = "1";
            TextBox_SegsZ.Text = "1";

            ComboBox_front.SelectedIndex = 0;
            ComboBox_top.SelectedIndex = 0;
            ComboBox_left.SelectedIndex = 0;
            ComboBox_right.SelectedIndex = 0;
            ComboBox_bottom.SelectedIndex = 1;
            ComboBox_back.SelectedIndex = 0;
            
            if (_surf != null)
            {
                TextBox_SurfX.Text = _surf.DistanceXToObj.ToString();
                TextBox_SurfY.Text = _surf.DistanceYToObj.ToString();
                TextBox_SurfZ.Text = _surf.DistanceZToObj.ToString();

                TextBox_SegsX.Text = _surf.SegmentCountX.ToString();
                TextBox_SegsY.Text = _surf.SegmentCountY.ToString();
                TextBox_SegsZ.Text = _surf.SegmentCountZ.ToString();

                ComboBox_front.SelectedIndex = (int)_surf.SurfaceStates[0];
                ComboBox_top.SelectedIndex = (int)_surf.SurfaceStates[1];
                ComboBox_left.SelectedIndex = (int)_surf.SurfaceStates[2];
                ComboBox_right.SelectedIndex = (int)_surf.SurfaceStates[3];
                ComboBox_bottom.SelectedIndex = (int)_surf.SurfaceStates[4];
                ComboBox_back.SelectedIndex = (int)_surf.SurfaceStates[5];
            }            
        }

        public Setup_Dialog(CuboidObject obj, CuboidSurface surf)
        {
            InitializeComponent();

            if(obj != null)
            {
                _obj = obj;
            }
            if(surf != null)
            {
                _surf = surf;
            }

            ComboBox_front.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();
            ComboBox_top.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();
            ComboBox_left.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();
            ComboBox_right.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();
            ComboBox_bottom.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();
            ComboBox_back.ItemsSource = Enum.GetValues(typeof(SurfaceState)).Cast<SurfaceState>();

            TestInit = false;
        }

        private CuboidObject _obj;
        private CuboidSurface _surf;
        public CuboidObject Object 
        {
            get 
            {
                return new CuboidObject(float.Parse(TextBox_ObjX.Text), float.Parse(TextBox_ObjY.Text), float.Parse(TextBox_ObjZ.Text));
            }
            private set
            {
                TextBox_ObjX.Text = value.SizeX.ToString();
                TextBox_ObjY.Text = value.SizeY.ToString();
                TextBox_ObjZ.Text = value.SizeZ.ToString();
            }
        }
        public CuboidSurface Surface
        {
            get
            {
                SurfaceState[] states = new SurfaceState[6];
                states[0] = (SurfaceState)ComboBox_front.SelectedItem;
                states[1] = (SurfaceState)ComboBox_top.SelectedItem;
                states[2] = (SurfaceState)ComboBox_left.SelectedItem;
                states[3] = (SurfaceState)ComboBox_right.SelectedItem;
                states[4] = (SurfaceState)ComboBox_bottom.SelectedItem;
                states[5] = (SurfaceState)ComboBox_back.SelectedItem;

                MeasurementMethod[] methods = new MeasurementMethod[6];
                for (int i = 0; i < methods.Length; i++)
                {
                    methods[i] = MeasurementMethod.Discrete;
                }

                return new CuboidSurface(
                    float.Parse(TextBox_SurfX.Text)
                    ,float.Parse(TextBox_SurfY.Text)
                    ,float.Parse(TextBox_SurfZ.Text)
                    ,uint.Parse(TextBox_SegsX.Text)
                    ,uint.Parse(TextBox_SegsY.Text)
                    ,uint.Parse(TextBox_SegsZ.Text)
                    ,states
                    //,methods
                    );
            }
            private set
            {
                TextBox_SurfX.Text = value.DistanceXToObj.ToString();
                TextBox_SurfY.Text = value.DistanceYToObj.ToString();
                TextBox_SurfZ.Text = value.DistanceZToObj.ToString();

                TextBox_SegsX.Text = value.SegmentCountX.ToString();
                TextBox_SegsY.Text = value.SegmentCountY.ToString();
                TextBox_SegsZ.Text = value.SegmentCountZ.ToString();
            }
        }

        public MeasurementMethod Method
        {
            get
            {
                if ((bool)radio_discrete.IsChecked)
                {
                    return MeasurementMethod.Discrete;
                }
                else
                {
                    return MeasurementMethod.Scan;
                }
            }
        }

        public string AnchorPath
        {
            get
            {
                return txt_AnchorFile.Text;
            }
            set
            {
                txt_AnchorFile.Text = value;
            }
        }

        public bool TestInit = false;

        private void Button_Setup_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBox_ObjX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_ObjY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_ObjZ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_SurfX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_SurfY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_SurfZ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.float_Regex);
        }

        private void TextBox_SegsX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_SegsY_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void TextBox_SegsZ_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = InputChecker.IsValid(e.Text, InputChecker.int_Regex);
        }

        private void btn_LoadAnchor_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".hmxanchor";
            openFileDialog.Filter = "HoloMetrix Anchor (.hmxanchor)|*.hmxanchor";
            if(openFileDialog.ShowDialog() == true)
            {
                AnchorPath = openFileDialog.FileName;
            }
        }

        private void btn_RemovePath_Click(object sender, RoutedEventArgs e)
        {
            AnchorPath = string.Empty;
        }

        private void btn_LoadTestAnchor_Click(object sender, RoutedEventArgs e)
        {
            AnchorPath = "MyTestAnchor";
        }

        private void btn_LoadMeasurement_Click(object sender, RoutedEventArgs e)
        {
            AnchorPath = "826de6b8-f4e4-443e-a5a5-db66d4df77cf";
            TestInit = true;
            this.DialogResult = true;
        }
    }
}
