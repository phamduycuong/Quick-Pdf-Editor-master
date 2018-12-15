using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WPF_PDFDocument.Controls
{
    /// <summary>
    /// Interaction logic for TabButton.xaml
    /// </summary>
    public partial class TabButton : UserControl
    {
        public string IconPath
        {
            get { return (string)GetValue(_IconPath); }
            set { SetValue(_IconPath, value); }
        }

        public static readonly DependencyProperty _IconPath = DependencyProperty.Register("IconPath", typeof(string), typeof(TabButton), new FrameworkPropertyMetadata(OnIconPathChanged));

        private static void OnIconPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as TabButton;
            if (button != null)
            {
                string currentAssemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string URI = System.IO.Path.Combine(currentAssemblyPath, e.NewValue as string);
                Uri uri = new Uri(URI);
                button.Icon.Source = new BitmapImage(uri);
            }
        }

        public string Text
        {
            get { return (string)GetValue(_Text); }
            set { SetValue(_Text, value); }
        }

        public static readonly DependencyProperty _Text = DependencyProperty.Register("Text", typeof(string), typeof(TabButton), new FrameworkPropertyMetadata(OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as TabButton;
            if (button != null)
            {
                button.label.Content = e.NewValue as string;
            }
        }

        public TabButton()
        {
            InitializeComponent();

        }

        private void Icon_MouseEnter(object sender, MouseEventArgs e)
        {
            //Do nothing
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Do nothing, just bubble
        }

        //Event Click
        //public static readonly RoutedEvent Click = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(TabButton));

        //// Provide CLR accessors for the event
        //public event RoutedEventHandler Tap
        //{
        //    add { AddHandler(Click, value); }
        //    remove { RemoveHandler(Click, value); }
        //}

        //// This method raises the Tap event
        //void RaiseTapEvent()
        //{
        //    RoutedEventArgs newEventArgs = new RoutedEventArgs(TabButton.Click);
        //    RaiseEvent(newEventArgs);
        //}
        //// For demonstration purposes we raise the event when the MyButtonSimple is clicked



        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    RaiseTapEvent();
        //}

    }
}
