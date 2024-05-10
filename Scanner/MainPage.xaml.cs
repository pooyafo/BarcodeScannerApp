using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;


namespace Scanner
{
    public partial class MainPage : ContentPage
    {
        private string _dbname = "products.db";

        List<Product> products;
        List<string> barcodes;
        private string _lastscannedbarcode;

        public string Lastscannedbarcode
        {
            get { return _lastscannedbarcode; }
            set { _lastscannedbarcode = value; }
        }

        public MainPage()
        {
            InitializeComponent();
            LoadInitialDatabase();
        }

        public string dbName
        {
            get { return _dbname; }
        }

        private void LoadInitialDatabase()
        {
            List<Product> initialListProducts;
            initialListProducts = new List<Product>()
            {
                new Product { Id = 1, Name = "Product A", Price = 100.5, Quantity = 4, Barcode = "1000000" },
                new Product { Id = 2, Name = "Product B", Price = 10.25, Quantity = 10, Barcode = "2000000" },
                new Product { Id = 3, Name = "Product C", Price = 37.8, Quantity = 40, Barcode = "3000000" },
                new Product { Id = 4, Name = "Product D", Price = 50.0, Quantity = 50, Barcode = "4000000" },
                new Product { Id = 5, Name = "Product E", Price = 32.0, Quantity = 40, Barcode = "787099257798" }
            };

            using (SQLiteConnection conn = new SQLiteConnection(Path.Combine(
                       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName)))
            {
                conn.CreateTable<Product>();
                barcodes = conn.Table<Product>().Select(p => p.Barcode).ToList();
                products = conn.Table<Product>().ToList();

                foreach (Product product in initialListProducts)
                {
                    if (CheckBarcodeExists(product.Barcode) == false)
                    {
                        conn.Insert(product);
                    }
                }

                barcodes = conn.Table<Product>().Select(p => p.Barcode).ToList();
                products = conn.Table<Product>().ToList();
            }
        }

        public (List<string>, List<Product>) ReloadDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Path.Combine(
                       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName)))
            {
                conn.CreateTable<Product>();
                barcodes = conn.Table<Product>().Select(p => p.Barcode).ToList();
                products = conn.Table<Product>().ToList();
            }

            return (barcodes, products);
        }

        public bool CheckBarcodeExists(string barcodeToCheck)
        {
            foreach (string barcode in barcodes)
            {
                if (barcode == barcodeToCheck || string.IsNullOrEmpty(barcodeToCheck))
                {
                    return true; // Barcode exists in the database
                }
            }

            return false; // Barcode does not exist in the database
        }

        public async void FindProductFromBarcode()
        {
            foreach (Product product in products)
            {
                if (product.Barcode == Lastscannedbarcode)
                {
                    // Device.BeginInvokeOnMainThread(() => { FoundProductName.Text = product.Name; });
                    ShowProductInformation(product);
                }
            }
        }

        private async void ShowProductInformation(Product product)
        {
            await DisplayAlert("Product Found",
                "Name:\n" + product.Name + "\n\nPrice:\n" + product.Price + "\n\nQuantity:\n" +
                product.Quantity +
                "\n\nBarcode:\n" + product.Barcode, "OK");
        }

        private void ScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lastScannedBarcode.Text = result.Text;
                Lastscannedbarcode = result.Text;
                if (CheckBarcodeExists(Lastscannedbarcode))
                {
                    FindProductFromBarcode();
                }
                else
                {
                    QuestionOnAddingNewProduct();
                }
            });
        }

        private async void QuestionOnAddingNewProduct()
        {
            var ans = await DisplayAlert("New Barcode Found!", "Do you want to add new Product?", "Yes", "No");
            if (ans)
            {
                await Navigation.PushAsync(new NewProductPage(this), true);
            }
        }

        private async void SearchProduct_Clicked(object sender, EventArgs e)
        {
            if (CheckBarcodeExists(Lastscannedbarcode))
            {
                FindProductFromBarcode();
            }
            else
            {
                QuestionOnAddingNewProduct();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // reload the database when the main page reload after first time.
            (barcodes, products) = ReloadDatabase();
        }

        private void LastScannedBarcode_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // set the entry field value inside the Lastscannedbarcode property
            Lastscannedbarcode = lastScannedBarcode.Text;
        }
    }
}