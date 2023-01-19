using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using HoloMetrix.Net.Remote;

namespace HoloMetrix_API_Explorer.Dialogs
{
    /// <summary>
    /// Interaction logic for GetDevices_Dialog.xaml
    /// </summary>
    public partial class GetDevices_Dialog : Window
    {
        public GetDevices_Dialog()
        {
            InitializeComponent();
            var devices = Bluetooth.GetDevices();

            ListBox_Devices.ItemsSource = devices;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (ListBox_Devices.Items.Count > 0)
                ListBox_Devices.SelectedIndex = 0;

            Button_Select.Focus();
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public BluetoothDevice SelectedDevice
        {
            get
            {
                var d = (BluetoothDevice)ListBox_Devices.SelectedItem;
                Debug.WriteLine(d.DeviceInfo.ClassOfDevice.MajorDevice);
                Debug.WriteLine(d.DeviceInfo.ClassOfDevice.Device);
                return (BluetoothDevice)ListBox_Devices.SelectedItem;
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Properties_Button_Click(object sender, RoutedEventArgs e)
        {
            var device = (BluetoothDevice)ListBox_Devices.SelectedItem;
            device.DeviceInfo.ShowDialog();
        }
    }
}
