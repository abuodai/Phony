﻿using LiteDB;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Phony.Kernel;
using Phony.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Phony.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        public Settings(int i)
        {
            InitializeComponent();
            SettingsTabControl.SelectedIndex = i;
        }

        public IEnumerable<Swatch> Swatches = new SwatchesProvider().Swatches;
        DbConnectionStringBuilder ConnectionStringBuilder = new DbConnectionStringBuilder();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Theme == "BaseDark")
            {
                ThemeS.SelectedIndex = 1;
            }
            else
            {
                ThemeS.SelectedIndex = 0;
            }
            if (Properties.Settings.Default.SalesBillsPaperSize == "A4")
            {
                BillReportPaperSizeCb.SelectedIndex = 0;
            }
            else
            {
                BillReportPaperSizeCb.SelectedIndex = 1;
            }
            ThemePC.Text = Properties.Settings.Default.PrimaryColor;
            foreach (var item in Swatches)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                SolidColorBrush brush1 = new SolidColorBrush(item.ExemplarHue.Color);
                SolidColorBrush brush2 = new SolidColorBrush(item.ExemplarHue.Foreground);
                cbi.Background = brush1;
                cbi.Foreground = brush2;
                cbi.Content = item.Name;
                ThemePC.Items.Add(cbi);
                if (item.Name == Properties.Settings.Default.PrimaryColor.ToLowerInvariant())
                {
                    ThemePC.SelectedItem = cbi;
                }
            }
            ThemeAC.Text = Properties.Settings.Default.AccentColor;
            foreach (var item in Swatches)
            {
                if (item.AccentExemplarHue == null)
                {
                    continue;
                }
                ComboBoxItem cbi = new ComboBoxItem();
                SolidColorBrush brush1 = new SolidColorBrush(item.AccentExemplarHue.Color);
                SolidColorBrush brush2 = new SolidColorBrush(item.AccentExemplarHue.Foreground);
                cbi.Background = brush1;
                cbi.Foreground = brush2;
                cbi.Content = item.Name;
                ThemeAC.Items.Add(cbi);
                if (item.Name == Properties.Settings.Default.AccentColor.ToLowerInvariant())
                {
                    ThemeAC.SelectedItem = cbi;
                }
            }
            ConnectionStringBuilder.ConnectionString = Properties.Settings.Default.DBFullName;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DBFullName))
            {
                DbFullPathTextBox.Text = ConnectionStringBuilder["Filename"].ToString();
                EncryptionkeyTextBox.Text = ConnectionStringBuilder["Password"].ToString();
            }
        }

        private async void SaveB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new PaletteHelper().ReplacePrimaryColor(ThemePC.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                new PaletteHelper().ReplaceAccentColor(ThemeAC.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (ThemeS.SelectedIndex == 0)
            {
                Properties.Settings.Default.Theme = "BaseLight";
                new PaletteHelper().SetLightDark(false);
            }
            else
            {
                Properties.Settings.Default.Theme = "BaseDark";
                new PaletteHelper().SetLightDark(true);
            }
            if (!(bool)UseLocalDefaultCheckBox.IsChecked)
            {
                if (string.IsNullOrWhiteSpace(DbFullPathTextBox.Text))
                {
                    await this.ShowMessageAsync("تحذير", "من فضلك اختار مكان لحفظ قاعده البيانات");
                    return;
                }
            }
            Properties.Settings.Default.PrimaryColor = ThemePC.Text;
            Properties.Settings.Default.AccentColor = ThemeAC.Text;
            Properties.Settings.Default.SalesBillsPaperSize = BillReportPaperSizeCb.Text;
            if (!(bool)UseLocalDefaultCheckBox.IsChecked)
            {
                ConnectionStringBuilder["Filename"] = DbFullPathTextBox.Text + "Phony.db";
                if (string.IsNullOrWhiteSpace(EncryptionkeyTextBox.Text))
                {
                    ConnectionStringBuilder["Password"] = EncryptionkeyTextBox.Text;
                }
            }
            else
            {
                ConnectionStringBuilder["Filename"] = Core.UserLocalAppFolderPath() + "..\\..\\Phony.db";
            }
            Properties.Settings.Default.DBFullName = ConnectionStringBuilder.ConnectionString;
            Properties.Settings.Default.Save();
            if (!Properties.Settings.Default.IsConfigured)
            {
                if (!Properties.Settings.Default.IsConfigured)
                {
                    try
                    {
                        using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                        {
                            var userCol = db.GetCollection<User>(ViewModel.DBCollections.Users.ToString());
                            var user = userCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (user == null)
                            {
                                userCol.Insert(new User
                                {
                                    Id = 1,
                                    Name = "admin",
                                    Pass = SecurePasswordHasher.Hash("admin"),
                                    Group = ViewModel.UserGroup.Manager,
                                    IsActive = true
                                });
                            }
                            var clientCol = db.GetCollection<Client>(ViewModel.DBCollections.Clients.ToString());
                            var client = clientCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (client == null)
                            {
                                clientCol.Insert(new Client
                                {
                                    Id = 1,
                                    Name = "كاش",
                                    Balance = 0,
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            var companyCol = db.GetCollection<Company>(ViewModel.DBCollections.Companies.ToString());
                            var company = companyCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (company == null)
                            {
                                companyCol.Insert(new Company
                                {
                                    Id = 1,
                                    Name = "لا يوجد",
                                    Balance = 0,
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            var salesMenCol = db.GetCollection<SalesMan>(ViewModel.DBCollections.SalesMen.ToString());
                            var salesMen = salesMenCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (salesMenCol == null)
                            {
                                salesMenCol.Insert(new SalesMan
                                {
                                    Id = 1,
                                    Name = "لا يوجد",
                                    Balance = 0,
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            var suppliersCol = db.GetCollection<Supplier>(ViewModel.DBCollections.Suppliers.ToString());
                            var supplier = suppliersCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (supplier == null)
                            {
                                suppliersCol.Insert(new Supplier
                                {
                                    Id = 1,
                                    Name = "لا يوجد",
                                    Balance = 0,
                                    SalesManId = 1,
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            var storesCol = db.GetCollection<Store>(ViewModel.DBCollections.Stores.ToString());
                            var store = storesCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (store == null)
                            {
                                storesCol.Insert(new Store
                                {
                                    Id = 1,
                                    Name = "التوكل",
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            var treasuriesCol = db.GetCollection<Treasury>(ViewModel.DBCollections.Treasuries.ToString());
                            var treasury = treasuriesCol.Find(x => x.Id == 1).FirstOrDefault();
                            if (treasury == null)
                            {
                                treasuriesCol.Insert(new Treasury
                                {
                                    Id = 1,
                                    Name = "الرئيسية",
                                    StoreId = 1,
                                    Balance = 0,
                                    CreatedById = 1,
                                    CreateDate = DateTime.Now,
                                    EditById = null,
                                    EditDate = null
                                });
                            }
                            db.GetCollection<Bill>(ViewModel.DBCollections.Treasuries.ToString());
                            db.GetCollection<BillItemMove>(ViewModel.DBCollections.BillsItemsMoves.ToString());
                            db.GetCollection<BillServiceMove>(ViewModel.DBCollections.BillsServicesMoves.ToString());
                            db.GetCollection<ClientMove>(ViewModel.DBCollections.ClientsMoves.ToString());
                            db.GetCollection<CompanyMove>(ViewModel.DBCollections.CompaniesMoves.ToString());
                            db.GetCollection<Item>(ViewModel.DBCollections.Items.ToString());
                            db.GetCollection<Note>(ViewModel.DBCollections.Notes.ToString());
                            db.GetCollection<SalesManMove>(ViewModel.DBCollections.SalesMenMoves.ToString());
                            db.GetCollection<ServiceMove>(ViewModel.DBCollections.ServicesMoves.ToString());
                            db.GetCollection<SupplierMove>(ViewModel.DBCollections.SuppliersMoves.ToString());
                            db.GetCollection<TreasuryMove>(ViewModel.DBCollections.TreasuriesMoves.ToString());
                        }
                        Properties.Settings.Default.IsConfigured = true;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        Properties.Settings.Default.IsConfigured = false;
                        Properties.Settings.Default.Save();
                        Core.SaveException(ex);
                    }
                }
                Close();
            }
            await this.ShowMessageAsync("تم الحفظ", "لقد تم تغيير اعدادات البرنامج و حفظها بنجاح");
        }
    }
}