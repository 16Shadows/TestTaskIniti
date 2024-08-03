using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Visualization.Views
{
    /// <summary>
    /// Interaction logic for CarQueue.xaml
    /// </summary>
    public partial class CarQueue : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty CarBrushProperty = DependencyProperty.Register(nameof(CarBrush), typeof(Brush), typeof(CarQueue), new PropertyMetadata(Brushes.Black));
        public static DependencyProperty TextBrushProperty = DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(CarQueue), new PropertyMetadata(Brushes.Red));
        public static DependencyProperty CarsCountProperty = DependencyProperty.Register(nameof(CarsCount), typeof(int), typeof(CarQueue), new PropertyMetadata(0, (target, e) =>
        {
            if (target is CarQueue q)
                q.PropertyChanged?.Invoke(q, new PropertyChangedEventArgs(nameof(ComputedVisibility)));
        }));

        public CarQueue()
        {
            InitializeComponent();
        }

        public Brush CarBrush
        {
            get => (Brush)GetValue(CarBrushProperty);
            set => SetValue(CarBrushProperty, value);
        }

        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public int CarsCount
        {
            get => (int)GetValue(CarsCountProperty);
            set => SetValue(CarsCountProperty, value);
        }

        public Visibility ComputedVisibility
        {
            get => CarsCount > 0 ? Visibility.Visible : Visibility.Hidden;
        }

		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
