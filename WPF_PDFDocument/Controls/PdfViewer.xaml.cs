using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

using System.Collections.Concurrent;
using System.Threading;

//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
namespace WPF_PDFDocument.Controls
{
    public partial class PdfViewer : UserControl
    {
        ConcurrentBag<Task> tasks;
        private Boolean PathAssigned;
        public string OriginalPdfPath;
        public int NumberOfPages; //Need improve
        private int _CurrentPage;
        CancellationToken canceltoken;

        public int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                _CurrentPage = value;
                Pages.SelectedIndex = _CurrentPage;
            }
        }
        private double rzoomvalue;
        public double zoomvalue
        {
            get
            {
                return rzoomvalue;
            }
            set
            {
                double a = value;
                if (a > 0.01 && a < 2)
                    rzoomvalue = a;
                else
                    return;

            }
        }
        public PdfViewer()
        {
            InitializeComponent();
            canceltoken = new CancellationToken(false);
            tasks = new ConcurrentBag<Task>();
            PathAssigned = false;
            CurrentPage = new int();
            zoomvalue = 1;
        }

       
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PdfViewer));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected void onClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(args);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            onClick();
        }


        #region Bindable Properties

        public string PdfPath
        {
            get { return (string)GetValue(PdfPathProperty); }
            set { SetValue(PdfPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PdfPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PdfPathProperty = DependencyProperty.Register("PdfPath", typeof(string), typeof(PdfViewer), new PropertyMetadata(null, propertyChangedCallback: OnPdfPathChanged));

        private static void OnPdfPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Đây là cái PdfViewer
            var pdfDrawer = (PdfViewer)d;
            

            if (!string.IsNullOrEmpty(pdfDrawer.PdfPath))
            {
                var path = System.IO.Path.GetFullPath(pdfDrawer.PdfPath);

                StorageFile.GetFileFromPathAsync(path).AsTask(pdfDrawer.canceltoken).ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask(), pdfDrawer.canceltoken).Unwrap()
                .ContinueWith(t2 => PdfToImages(pdfDrawer, t2.Result), TaskScheduler.FromCurrentSynchronizationContext());

                if (pdfDrawer.PathAssigned == false)
                {
                    pdfDrawer.OriginalPdfPath = path;
                    pdfDrawer.PathAssigned = true;
                }
            }
        }
        #endregion


        private async static Task PdfToImages(PdfViewer pdfViewer, PdfDocument pdfDoc)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            
            var items = pdfViewer.PagesContainer.Items;

            //Small update
            var comboboxitem = pdfViewer.Pages.Items;

            items.Clear();

            if (pdfDoc == null) return;
            

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                using (var page = pdfDoc.GetPage(i))
                {
                    var bitmap = await PageToBitmapAsync(page);
                    var image = new Image
                    {
                        Source = bitmap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 4),
                        MaxWidth = 800
                    };
                    items.Add(image);
                    //Small update
                    comboboxitem.Add(i + 1 + "/" + pdfDoc.PageCount);                   
                }
            }
           
        }

        //Disposed
        private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
        {
            BitmapImage image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream.AsStream();
                image.EndInit();
                stream.Dispose();
            }

            page.Dispose();
            
            return image;
        }

        private void PagesContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = e.Source as UIElement;
            MessageBox.Show(e.GetPosition(element).ToString());

            MessageBox.Show(element.RenderSize.ToString());

        }

        private void Btntest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paused");
        }

        private void jumpToPage(int n)
        {
            try
            {
                Image image = PagesContainer.Items.GetItemAt(n - 1) as Image;
                image.BringIntoView();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                if (n == 0)
                    return;
                MessageBox.Show("Please wait...");
            }
        }

        private void JumptoPage(object sender, SelectionChangedEventArgs e)
        {
            jumpToPage(Pages.SelectedIndex);
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {

            //Xây dựng ma trận transform dựa trên vị trí của chuột
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var element = PagesContainer as UIElement;
                var position = e.GetPosition(element);

                var transform = element.RenderTransform as MatrixTransform;
                var matrix = transform.Matrix;
                var scale = e.Delta >= 0 ? 1.2 : (1.0 / 1.2); // 
                zoomvalue *= scale;
                slider.Value = Math.Log10(zoomvalue);

                matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);

                element.RenderTransform = new MatrixTransform(matrix);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseWheel(e);
            }
        }

        private void SliderZoom_ValueChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var element = PagesContainer as UIElement;
            var trasnform = element.RenderTransform as MatrixTransform;
            var matrix = trasnform.Matrix;
            var scale = Math.Pow(10, e.NewValue) / Math.Pow(10, e.OldValue);

            matrix.Scale(scale, scale);
            element.RenderTransform = new MatrixTransform(matrix);

            e.Handled = true;
        }
        //Begin Update
        private bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;
            Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        private void UpdatePageNumber()
        {
            var items = this.PagesContainer.Items;
            if (items.Count == 0)
                return;
            //List Lưu trữ số các trang đang nhìn thấy
            System.Collections.Generic.List<int> ListPage = new System.Collections.Generic.List<int>();
            ListPage.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (IsUserVisible(items.GetItemAt(i) as FrameworkElement, this.scrollview))
                {
                    ListPage.Add(i);
                }
            }

            int position = new int();
            if (ListPage.Count == 1)
            {
                position = ListPage[0];
                goto labelx;
            }
            if (ListPage.Count == 2)
            {
                position = ListPage[0];
                goto labelx;
            }
            try
            {
                position = ListPage[ListPage.Count / 2];
            }catch(ArgumentOutOfRangeException)
            {
                return;
            }
            labelx:  this.Pages.Items.MoveCurrentToPosition(position);
            this.Pages.UpdateLayout();
            this.CurrentPage = position;

        }

        private void ItemControl_Scroll(object sender, MouseWheelEventArgs e)
        {
            UpdatePageNumber();
            //this.Pages.Items.MoveCurrentToLast();
            //MessageBox.Show(Pages.Items.CurrentItem.ToString());
        }
        //End Update    


        private void canvas_mup(object sender, MouseButtonEventArgs e)
        {

            PDFAction.DrawStop(canvas);
           // PDFAction.SaveComment(this.PdfPath, PageCursorFocus(e));
        }

        private void grid_mup(object sender, MouseButtonEventArgs e)
        {

        }
       
        private void scrollwiew_mup(object sender, MouseButtonEventArgs e)
        {

        }
        private void itemcontrol_mup(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_mmove(object sender, MouseEventArgs e)
        {
            PDFAction.Draw(canvas as UIElement, e);
        }

        private void scrollwiew_mmove(object sender, MouseEventArgs e)
        {         
               
        }     

        private void grip_mmove(object sender, MouseEventArgs e)
        {
            
           
        }      

        private void itemcontrol_mmove(object sender, MouseEventArgs e)
        {
           
           
        }

        private void canvas_lmdown(object sender, MouseButtonEventArgs e)
        {
            PDFAction.DrawStart(canvas, e);
        }

        private void grid_lmdown(object sender, MouseButtonEventArgs e)
        {
            PDFAction.DrawStart(canvas, e);
        }

        private void scrollwiew_lmdown(object sender, MouseButtonEventArgs e)
        {
            PDFAction.DrawStart(canvas, e);
        }

        private void itemcontrol_lmdown(object sender, MouseButtonEventArgs e)
        {
             PDFAction.DrawStart(canvas, e);
             //PageCursorFocus(e);
        }

       
        //public int PageCursorFocus(MouseButtonEventArgs e)
        //{
        //    UIElement UIe = e.Source as UIElement;
        //    Point CusorFromPage = e.GetPosition(UIe);
        //    Point CusorFromScrollwiewer = e.GetPosition(scrollview);          
        //    if (CusorFromPage.Y + 10 < CusorFromScrollwiewer.Y)
        //    {
        //        return _CurrentPage;
        //    }
        //    return _CurrentPage - 1;
        //}
       
    }
}
