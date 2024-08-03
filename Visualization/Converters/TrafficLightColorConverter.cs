using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Visualization.ViewModels;

namespace Visualization.Converters
{
	class TrafficLightColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TrafficLightColor color = (TrafficLightColor)value;
			switch (color)
			{
				case TrafficLightColor.Red:
					return Brushes.Red;
				case TrafficLightColor.Green:
					return Brushes.Green;
			}
			throw new ArgumentException("Invalid color", nameof(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
