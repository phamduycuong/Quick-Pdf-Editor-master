using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace WPF_PDFDocument
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string currentAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string URI = System.IO.Path.Combine(currentAssemblyPath,@"Icon\ProgramIcon.png");
            Uri uri = new Uri(URI);
            this.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(uri);
        }


        private void Pdfviewer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void OpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Portable Document Format(*.pdf)|*.pdf|All files (*.*)|*.*";

            FileInfo info;
            if (openFile.ShowDialog() == true)
            {
                info = new FileInfo(openFile.FileName);
                if (info.Extension != ".pdf")
                {
                    MessageBox.Show("Can't open file");
                    return;
                }
                //pdfviewer.PdfPath = openFile.FileName;

                //Nếu file đã open
                foreach (System.Windows.Controls.TabItem item in TabController.Items)
                {
                    var pdfviewer = item.Content as Controls.PdfViewer;
                    if (pdfviewer.OriginalPdfPath == info.Name)
                    {
                        this.TabController.SelectedItem = item;
                        MessageBox.Show("File is already opened!", "Quick PDF Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                //Nếu File chưa Open
                System.Windows.Controls.TabItem tabItem = new System.Windows.Controls.TabItem();
                tabItem.Header = info.Name;

                //Opened File
                Controls.PdfViewer pdfViewer = new Controls.PdfViewer();
                pdfViewer.PdfPath = openFile.FileName;
                tabItem.Content = pdfViewer;
                this.TabController.Items.Add(tabItem);
                Dispatcher.BeginInvoke((Action)(() => this.TabController.SelectedIndex = this.TabController.Items.Count - 1));
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseAll_Click(sender, e);
            this.Close();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Dialog.About about = new Dialog.About();
            about.Show();
        }

        private void TabButton_Loaded_1(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Show();
        }

        private void InsertPage(object sender, MouseButtonEventArgs e)
        {
            var tabItem = TabController.SelectedItem as System.Windows.Controls.TabItem;
            if (tabItem == null)
            {
                MessageBox.Show("There is no file opening!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Pdf|*.pdf";
            openFile.ShowDialog();

            if (openFile.FileName == "")
                return;

            //get path

            var pdfview = tabItem.Content as Controls.PdfViewer;
            string path;
            if (pdfview!= null)
                path = pdfview.PdfPath;
            Dialog.InsertPage insertPage = new Dialog.InsertPage(tabItem);
            insertPage.PreviewPDF.PdfPath = openFile.FileName;
            insertPage.Show();
        }

        private void NewTab_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TabItem tabItem = new System.Windows.Controls.TabItem();
            tabItem.Header = "Blank Document";
            this.TabController.Items.Add(tabItem);
            this.TabController.SelectedItem = tabItem;
        }

        private void NewBlankPdf_Click(object sender, RoutedEventArgs e)
        {

            ///Chưa hoàn thiện
            //Nếu File chưa Open

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Check
            if (TabController.SelectedIndex == -1)
            {
                MessageBox.Show("There is nothing to save!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var tabitem = TabController.SelectedItem as System.Windows.Controls.TabItem;
            if (tabitem.Content == null)
            {
                MessageBox.Show("Cannot save blank document!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pdfviewer = tabitem.Content as Controls.PdfViewer;
            if (pdfviewer.OriginalPdfPath == pdfviewer.PdfPath)
            {
                return;
            }

            var tab = this.TabController.SelectedItem as System.Windows.Controls.TabItem;

            string originalpath = pdfviewer.OriginalPdfPath;
            string currentpath = pdfviewer.PdfPath;

            if (pdfviewer.PdfPath == pdfviewer.OriginalPdfPath)
            {
                return;
            }


            label1:

            try
            {
                if (pdfviewer.OriginalPdfPath != "")
                    System.IO.File.Delete(pdfviewer.OriginalPdfPath);
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Pdf Document|*.pdf|All Files|*.*";
                    saveFileDialog.ShowDialog();

                    if(File.Exists(saveFileDialog.FileName))
                    {
                        if(saveFileDialog.OverwritePrompt== true)
                        {
                            try
                            {
                                File.Delete(saveFileDialog.FileName);
                                File.Move(pdfviewer.PdfPath, saveFileDialog.FileName);
                                MessageBox.Show("File saved!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                            catch(UnauthorizedAccessException)
                            {
                                MessageBox.Show("Access denied!");
                            }
                        }
                    }
                    else
                    {
                        FileInfo Info = new FileInfo(pdfviewer.PdfPath);
                        Info.MoveTo(saveFileDialog.FileName);
                        pdfviewer.PdfPath = saveFileDialog.FileName;
                        pdfviewer.OriginalPdfPath = saveFileDialog.FileName;
                        MessageBox.Show("File Saved!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Please Wait...");
                goto label1;
            }


            FileInfo fileInfo = new FileInfo(pdfviewer.PdfPath);
            fileInfo.MoveTo(pdfviewer.OriginalPdfPath);
            pdfviewer.PdfPath = pdfviewer.OriginalPdfPath;
            MessageBox.Show("File Saved!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (this.TabController.Items.Count == 0)
            {
                MessageBox.Show("There is nothing to close!", "Quick PDF Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //Compare original path with pdfpath
            var tab = this.TabController.SelectedItem as System.Windows.Controls.TabItem;
            var pdfviewer = tab.Content as Controls.PdfViewer;
            if (tab == null || pdfviewer == null)
                goto label1;

            if (pdfviewer.OriginalPdfPath != pdfviewer.PdfPath)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save this document?", "Quick Pdf Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                    return;
                if (result == MessageBoxResult.Yes)
                {
                    //Can't use sender and e
                    this.Save_Click(sender, e);
                }
            }

            label1:
            int index = TabController.SelectedIndex;
            this.TabController.Items.RemoveAt(index);
        }

        private void CloseAll_Click(object sender, RoutedEventArgs e)
        {
            if (TabController.Items.Count >= 0)
            {
                TabController.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("There is nothing to close!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            while (TabController.Items.Count > 0)
            {
                TabController.SelectedIndex = 0;
                Close_Click(sender, e);
            }
        }

        private void DeletePage_Click(object sender, MouseButtonEventArgs e)
        {
            var tabItem = TabController.SelectedItem as System.Windows.Controls.TabItem;
            if (tabItem == null)
            {
                MessageBox.Show("There is no file opening!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Dialog.DeletePage deletePage = new Dialog.DeletePage(this.TabController.SelectedItem as System.Windows.Controls.TabItem);
            deletePage.Show();
        }

        private void CloseTab_Click(object sender, MouseButtonEventArgs e)
        {
            Close_Click(sender, e);
        }

        private void btnCloseAll_Click(object sender, MouseButtonEventArgs e)
        {
            CloseAll_Click(sender, e as MouseButtonEventArgs);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (TabController.SelectedIndex == -1)
            {
                MessageBox.Show("There is nothing to save!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var tabitem = TabController.SelectedItem as System.Windows.Controls.TabItem;
            if (tabitem.Content == null)
            {
                MessageBox.Show("Cannot save blank document!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pdfviewer = tabitem.Content as Controls.PdfViewer;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf Document|.pdf";
            saveFileDialog.FileName = pdfviewer.OriginalPdfPath;
            if (saveFileDialog.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(saveFileDialog.FileName);
                if (info.Exists)
                {
                    if (info.FullName == saveFileDialog.FileName)
                        return;
                    MessageBoxResult result = MessageBox.Show("File is already exist. Do you want to overwrite?", "Quick Pdf Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        info.Delete();
                        try
                        {
                            File.Copy(pdfviewer.PdfPath, saveFileDialog.FileName);
                            MessageBox.Show("File saved!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show("File IO Error!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);

                        }

                    }
                    if (result == MessageBoxResult.No)
                    {
                        saveFileDialog.ShowDialog();
                    }
                }
                else
                {
                    try
                    {
                        File.Copy(pdfviewer.PdfPath, saveFileDialog.FileName);
                        MessageBox.Show("File saved!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("File IO Error!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
        }

        private void Open_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog_Click(sender, e);
        }

        private void NewBlankClick(object sender, MouseButtonEventArgs e)
        {
            NewBlankPdf_Click(sender, e);
        }

        private void Save(object sender, MouseButtonEventArgs e)
        {
            Save_Click(sender, e);
        }

        private void SaveAll(object sender, MouseButtonEventArgs e)
        {
            SaveAll_Click(sender, e);
        }

        private void NewTabClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TabItem tabItem = new System.Windows.Controls.TabItem();
            tabItem.Header = "Blank Document";
            this.TabController.Items.Add(tabItem);
            this.TabController.SelectedItem = tabItem;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close_Click(sender, e);
        }

        private void NewLine_Click(object sender, MouseButtonEventArgs e)
        {
            var tab = this.TabController.SelectedItem as System.Windows.Controls.TabItem;
            var pdfviewer = tab.Content as Controls.PdfViewer;
            PDFAction.DrawInit(pdfviewer.canvas);
            PDFAction.drawType = 0;
        }

        private void NewRectangle_Click(object sender, MouseButtonEventArgs e)
        {
            var tab = this.TabController.SelectedItem as System.Windows.Controls.TabItem;
            var pdfviewer = tab.Content as Controls.PdfViewer;
            PDFAction.DrawInit(pdfviewer.canvas);
            PDFAction.drawType = 1;
        }

        private void NewEllipes_Click(object sender, MouseButtonEventArgs e)
        {
            var tab = this.TabController.SelectedItem as System.Windows.Controls.TabItem;
            var pdfviewer = tab.Content as Controls.PdfViewer;
            PDFAction.DrawInit(pdfviewer.canvas);
            PDFAction.drawType = 2;
        }
    }
}
