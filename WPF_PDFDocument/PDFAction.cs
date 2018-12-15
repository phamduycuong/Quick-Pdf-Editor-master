using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.Pdf.Canvas;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;



namespace WPF_PDFDocument
{
    class PDFAction
    {
        public static int number = 0;
        public static int drawType = 0;
        public static Color drawColor = Colors.Black;
        public static int thinkness = 2;
        public static bool _isDrawing = false;
        public static bool _isActive = false;
        public static Point mousePos;
        public static Point startPos;
        public static Point PosToPage;
        public static Line line;
        public static Rectangle rect;
        public static Ellipse ellipes;

        public static int Undo_max = 5;
        private static Stack<PdfPage> backuppageStack = new Stack<PdfPage>(); 
        private static double left, top, width, height;


        public static void InsertPageFromPdf(string PdfSourcePath, string PdfDesPath, System.Collections.Generic.List<int> ListPage, int offset)
        {
            //PDF Merger
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Merged" + number++ + ".pdf");
            PdfDocument pdfMergered = new PdfDocument(new PdfWriter(path));
            PdfMerger pdfMerger = new PdfMerger(pdfMergered);

            //Source and Des
            PdfDocument source = new PdfDocument(new PdfReader(PdfSourcePath));
            PdfDocument des = new PdfDocument(new PdfReader(PdfDesPath));

            //Check offset
            if (offset == 0)
            {
                for (int i = 0; i < ListPage.Count; i++)
                {
                    pdfMerger.Merge(source, ListPage[i] + 1, ListPage[i] + 1);
                }

                if (offset + 1 <= des.GetNumberOfPages())
                {
                    pdfMerger.Merge(des, offset + 2, des.GetNumberOfPages());
                }

                source.Close();
                des.Close();
                pdfMergered.Close();
                return;
            }



            pdfMerger.Merge(des, 1, offset);

            for (int i = 0; i < ListPage.Count; i++)
            {
                pdfMerger.Merge(source, ListPage[i] + 1, ListPage[i] + 1);
            }

            if (offset + 1 <= des.GetNumberOfPages())
            {
                pdfMerger.Merge(des, offset + 2, des.GetNumberOfPages());
            }

            source.Close();
            des.Close();
            pdfMergered.Close();
        }

        public static string InsertPageFromPdf(string PdfSourcePath, System.Collections.Generic.List<int> ListPage)
        {
            //PDF Merger
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Merged" + number++ + ".pdf");
            PdfDocument pdfMergered = new PdfDocument(new PdfWriter(path));
            PdfMerger pdfMerger = new PdfMerger(pdfMergered);

            //Source and Des
            PdfDocument source = new PdfDocument(new PdfReader(PdfSourcePath));

            //Check offset
            for (int i = 0; i < ListPage.Count; i++)
            {
                pdfMerger.Merge(source, ListPage[i] + 1, ListPage[i] + 1);
            }
            source.Close();
            pdfMergered.Close();
            pdfMerger.Close();
            return path;
        }




        public static string DeletePage(string Path, int from, int to)
        {
            //PDF Merger
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DeletePage")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DeletePage"));
            }

            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DeletePage", "Merged" + number++ + ".pdf");


            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            PdfDocument pdfMergered = new PdfDocument(new PdfWriter(path));
            PdfMerger pdfMerger = new PdfMerger(pdfMergered);

            //Source and Des
            PdfDocument source = new PdfDocument(new PdfReader(Path));
            for (int i = 1; i < from; i++)
            {
                pdfMerger.Merge(source, i, i);
            }

            for (int i = to + 1; i <= source.GetNumberOfPages(); i++)
            {
                pdfMerger.Merge(source, i, i);
            }

            source.Close();
            pdfMerger.Close();
            pdfMergered.Close();
            return path;
        }

        public static void DrawInit(Canvas canvas)
        {    
            canvas.Cursor = System.Windows.Input.Cursors.Cross;
            _isActive = true;         
        }

        public static void DrawStart( Canvas canvas, System.Windows.Input.MouseEventArgs e)
        {
            


            if (!_isDrawing && _isActive)
            {   
                _isDrawing = true;

                UIElement UIe = e.Source as UIElement;
                PosToPage = e.GetPosition(UIe);
                startPos = e.GetPosition(canvas);
                
                switch (drawType)
                {
                    case 0:
                        line = new Line();
                        line.StrokeThickness = thinkness;
                        line.Stroke = new SolidColorBrush(drawColor);
                        canvas.Children.Add(line);
                        break;
                    case 1:
                        rect = new Rectangle();
                        rect.Stroke = new SolidColorBrush(drawColor);
                        rect.StrokeThickness = thinkness;
                        Canvas.SetLeft(rect, startPos.X);
                        Canvas.SetTop(rect, startPos.Y);
                        canvas.Children.Add(rect);
                        break;
                    case 2:
                        ellipes = new Ellipse();
                        ellipes.Stroke = new SolidColorBrush(drawColor);
                        ellipes.StrokeThickness = thinkness;
                        Canvas.SetLeft(ellipes, startPos.X);
                        Canvas.SetTop(ellipes, startPos.Y);
                        canvas.Children.Add(ellipes);
                        break;
                }
            }
        }

        public static void Draw( UIElement container, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDrawing && _isActive)
            {               
                mousePos = e.GetPosition(container);              

                switch (drawType)
                {

                    case 0:

                        line.X1 = startPos.X;
                        line.Y1 = startPos.Y;
                        line.X2 = mousePos.X;
                        line.Y2 = mousePos.Y;
                        break;

                    case 1:
                        left = startPos.X;
                        top = startPos.Y;
                        width = mousePos.X - startPos.X;
                        height = mousePos.Y - startPos.Y;

                        if (mousePos.X < startPos.X)
                        {
                            left = mousePos.X;
                            width = startPos.X - mousePos.X;
                        }

                        if (mousePos.Y < startPos.Y)
                        {
                            top = mousePos.Y;
                            height = startPos.Y - mousePos.Y;
                        }

                        Canvas.SetLeft(rect, left);
                        Canvas.SetTop(rect, top);
                        rect.Width = width;
                        rect.Height = height;
                        break;


                    case 2:
                        left = startPos.X;
                        top = startPos.Y;
                        width = mousePos.X - startPos.X;
                        height = mousePos.Y - startPos.Y;

                        if (mousePos.X < startPos.X)
                        {
                            left = mousePos.X;
                            width = startPos.X - mousePos.X;
                        }

                        if (mousePos.Y < startPos.Y)
                        {
                            top = mousePos.Y;
                            height = startPos.Y - mousePos.Y;
                        }


                        Canvas.SetLeft(ellipes, left);
                        Canvas.SetTop(ellipes, top);
                        ellipes.Width = width;
                        ellipes.Height = height;
                        break;
                }
            }
        }

        public static void DrawStop( Canvas canvas)
        {
            if (_isDrawing && _isActive)
            {
                _isDrawing = _isActive = false;
                switch (drawType)
                {
                    case 0:
                        canvas.Children.Remove(line);
                        break;
                    case 1:
                        canvas.Children.Remove(rect);
                        break;
                    case 2:
                        canvas.Children.Remove(ellipes);
                        break;
                }            
                canvas.Cursor = System.Windows.Input.Cursors.Arrow;
            }         
        }

        //public static void SaveComment(string PdfSourcePath, int  pageIndex )
        //{

            
        //     PdfDocument pdfSource = new PdfDocument(new PdfWriter(PdfSourcePath));
        //    PdfCanvas canvas = new PdfCanvas(pdfSource.GetPage(pageIndex));          
            
        //    canvas.SetLineWidth(thinkness);
        //    double _top;
        //    switch (drawType)
        //    {   
        //        case 0:                    
        //            canvas.MoveTo(startPos.X, startPos.Y);
        //            canvas.LineTo(mousePos.X, mousePos.Y);
        //            break;
        //        case 1:
        //            _top = pdfSource.GetPage(pageIndex).GetPageSize().GetHeight() - top;
        //            canvas.Rectangle(left, _top, width, height);
        //            break;
        //        case 2:
        //            _top = pdfSource.GetPage(pageIndex).GetPageSize().GetHeight() - top;
        //            canvas.Rectangle(left, _top, width, height);
        //            break;
        //    }
        //    canvas.Stroke();
        //    pdfSource.Close();
        //}
        
    }
}
