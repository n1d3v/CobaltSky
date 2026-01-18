using System.Windows;
using System.Windows.Controls;

namespace CobaltSky.UserControls
{
    public partial class TrendItem : UserControl
    {
        public TrendItem()
        {
            InitializeComponent();
        }

        public string TrendName
        {
            get { return (string)GetValue(TrendNameProperty); }
            set { SetValue(TrendNameProperty, value); }
        }

        public static readonly DependencyProperty TrendNameProperty =
            DependencyProperty.Register(
                "TrendName",
                typeof(string),
                typeof(TrendItem),
                new PropertyMetadata(""));
    }
}
