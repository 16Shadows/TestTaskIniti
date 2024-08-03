using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Visualization.Views
{
    /// <summary>
    /// Interaction logic for PedestrianQueue.xaml
    /// </summary>
    public partial class PedestrianQueue : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty PedestrianBrushProperty = DependencyProperty.Register(nameof(PedestrianBrush), typeof(Brush), typeof(PedestrianQueue), new PropertyMetadata(Brushes.Black));
        public static DependencyProperty TextBrushProperty = DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(PedestrianQueue), new PropertyMetadata(Brushes.Red));
        public static DependencyProperty PedestriansCountProperty = DependencyProperty.Register(nameof(PedestriansCount), typeof(int), typeof(PedestrianQueue), new PropertyMetadata(0, (target, e) =>
        {
            if (target is PedestrianQueue q)
                q.PropertyChanged?.Invoke(q, new PropertyChangedEventArgs(nameof(ComputedVisibility)));
        }));

        public PedestrianQueue()
        {
            InitializeComponent();
        }

        public Brush PedestrianBrush
        {
            get => (Brush)GetValue(PedestrianBrushProperty);
            set => SetValue(PedestrianBrushProperty, value);
        }

        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public int PedestriansCount
        {
            get => (int)GetValue(PedestriansCountProperty);
            set => SetValue(PedestriansCountProperty, value);
        }

        public Visibility ComputedVisibility
        {
            get => PedestriansCount > 0 ? Visibility.Visible : Visibility.Hidden;
        }

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
