using SQLite;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewProductPage : ContentPage
    {
        private MainPage mainPage;

        public NewProductPage(MainPage _mainPage)
        {
            mainPage = _mainPage;
            InitializeComponent();
            lastScannedBarcode.Text = mainPage.Lastscannedbarcode;
        }

        private async void SaveButtonNewProduct_OnClicked(object sender, EventArgs e)
        {
            var ans = await DisplayAlert("New Product", "Are you sure to add new product?", "Yes", "No");
            if (ans)
            {
                if (string.IsNullOrEmpty(lastScannedBarcode.Text) || string.IsNullOrEmpty(inputQuantity.Text) ||
                    string.IsNullOrEmpty(inputPrice.Text) ||
                    string.IsNullOrEmpty(inputName.Text) || !int.TryParse(inputQuantity.Text, out _))
                {
                    await DisplayAlert("Error", "Fill all of the fields. Also, check fields type.", "OK");
                }
                else
                {
                    // add new product to database
                    AddNewProductToDatabase(mainPage.dbName);
                    await DisplayAlert("Successful",
                        "New product added to database with following barcode:\n" + lastScannedBarcode.Text, "OK");
                    // get back to the previous page
                    await Navigation.PopToRootAsync();
                }
            }
        }

        private void AddNewProductToDatabase(string dbname)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Path.Combine(
                       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                       dbname)))
            {
                // read the related table
                conn.CreateTable<Product>();
                // create new product
                Product newProduct = new Product
                {
                    Name = inputName.Text,
                    Price = Double.Parse(inputPrice.Text),
                    Quantity = Int32.Parse(inputQuantity.Text),
                    Barcode = lastScannedBarcode.Text
                };
                // insert in the database
                conn.Insert(newProduct);
            }
        }
    }
}