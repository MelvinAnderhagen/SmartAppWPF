using SmartAppWPF.MVVM.Cores;
using SmartAppWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAppWPF.MVVM.ViewModels
{
    internal class WeatherComponentViewModel : ObservableObject
    {
        private string? _currentWeatherCondition;
        public readonly IWeatherService _weatherService;


        public WeatherComponentViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
            SetWeatherAsync().ConfigureAwait(false);
        }

        public string CurrentWeatherCondition
        {
            get => _currentWeatherCondition!;
            set
            {
                _currentWeatherCondition = value;
                OnPropertyChanged();
            }
        }

        private string? _currentTemperature;
        public string CurrentTemperature
        {
            get => _currentTemperature!;
            set
            {
                _currentTemperature = value;
                OnPropertyChanged();
            }
        }

        private async Task SetWeatherAsync()
        {
            var weather = await _weatherService.GetWeatherDataAsync();
            CurrentTemperature = weather.Temperature.ToString();
            CurrentWeatherCondition = weather.WeatherCondition ?? "";
        }
    }
}
