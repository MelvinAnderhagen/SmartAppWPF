using Microsoft.Azure.Devices;
using SmartAppWPF.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SmartAppWPF.MVVM.ViewModels
{
    internal class KitchenViewModel
    {
        private DispatcherTimer timer;
        private ObservableCollection<DeviceItem> _devices;
        private readonly RegistryManager registryManager = RegistryManager
            .CreateFromConnectionString("HostName=TestIotV10.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=65IG+I38my66zPsmGkxx4eCzpW6hRE62KP1Ku2KxL/I=");

        public KitchenViewModel()
        {
            _devices = new ObservableCollection<DeviceItem>();

            PopulateDeviceListsAsync().ConfigureAwait(false);
            setInterval(TimeSpan.FromSeconds(5));
        }

        public string Title { get; set; } = "Kitchen";
        public string Temperature { get; set; } = "26 °C";
        public string Humidity { get; set; } = "21 %";

        public IEnumerable<DeviceItem> Devices => _devices;

        private void setInterval(TimeSpan interval)
        {
            timer = new DispatcherTimer()
            {
                Interval = interval
            };

            timer.Tick += new EventHandler(timer_Tick);

            timer.Start();
        }
        private async void timer_Tick(object sender, EventArgs s)
        {
            await PopulateDeviceListsAsync();
            await UpdateDeviceItemsAsync();
        }

        private async Task UpdateDeviceItemsAsync()
        {
            ObservableCollection<DeviceItem> devices = _devices;

            foreach (var iten in devices.ToList())
            {
                var device = await registryManager.GetDeviceAsync(iten.DeviceId);

                if (device == null)
                {
                    _devices.Remove(iten);
                }
            }
        }

        private async Task PopulateDeviceListsAsync()
        {
            var result = registryManager.CreateQuery("SELECT * FROM Devices");

            if (result.HasMoreResults)
            {
                foreach (var twin in await result.GetNextAsTwinAsync())
                {
                    var _device = _devices.FirstOrDefault(e => e.DeviceId == twin.DeviceId);
                    if (_device == null)
                    {
                        _device = new DeviceItem()
                        {
                            DeviceId = twin.DeviceId
                        };

                        try { _device.DeviceName = twin.Properties.Reported["DeviceName"]; }
                        catch { _device.DeviceName = twin.DeviceId; }
                        try { _device.DeviceType = twin.Properties.Reported["DeviceType"]; }
                        catch { }


                        switch (_device.DeviceType.ToLower())
                        {
                            case "fan":
                                _device.IconActive = "\uf863";
                                _device.IconInActive = "\uf863";
                                _device.StateActive = "ON";
                                _device.StateInActive = "OFF";
                                break;

                            case "light":
                                _device.IconActive = "\uf672";
                                _device.IconInActive = "\uf0eb";
                                _device.StateActive = "ON";
                                _device.StateInActive = "OFF";
                                break;

                            default:
                                _device.IconActive = "\uf2db";
                                _device.IconInActive = "\uf2db";
                                _device.StateActive = "ENABLE";
                                _device.StateInActive = "DISABLE";
                                break;
                        }
                        _devices.Add(_device);
                    }
                    else
                    {

                    }
                }

            }
            else
            {
                _devices.Clear();
            }
        }
    }
}
