﻿using LiteDB;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Phony.Extensions;
using Phony.Kernel;
using Phony.Model;
using Phony.Utility;
using Phony.View;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Phony.ViewModel
{
    class MainPageVM : CommonBase
    {
        int _itemsCount;
        int _clientsCount;
        int _shortagesCount;
        int _servicesCount;
        int _suppliersCount;
        int _cardsCount;
        int _companiesCount;
        int _salesMenCount;
        int _numbersCount;
        int _usersCount;
        string _userName;
        string _password;
        string _newPassword;
        string _phone;
        string _group;

        bool isBacking;

        public int ItemsCount
        {
            get => _itemsCount;
            set
            {
                if (value != _itemsCount)
                {
                    _itemsCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ClientsCount
        {
            get => _clientsCount;
            set
            {
                if (value != _clientsCount)
                {
                    _clientsCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ShortagesCount
        {
            get => _shortagesCount;
            set
            {
                if (value != _shortagesCount)
                {
                    _shortagesCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ServicesCount
        {
            get => _servicesCount;
            set
            {
                if (value != _servicesCount)
                {
                    _servicesCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int SuppliersCount
        {
            get => _suppliersCount;
            set
            {
                if (value != _suppliersCount)
                {
                    _suppliersCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CardsCount
        {
            get => _cardsCount;
            set
            {
                if (value != _cardsCount)
                {
                    _cardsCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CompaniesCount
        {
            get => _companiesCount;
            set
            {
                if (value != _companiesCount)
                {
                    _companiesCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int SalesMenCount
        {
            get => _salesMenCount;
            set
            {
                if (value != _salesMenCount)
                {
                    _salesMenCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int NumbersCount
        {
            get => _numbersCount;
            set
            {
                if (value != _numbersCount)
                {
                    _numbersCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int UsersCount
        {
            get => _usersCount;
            set
            {
                if (value != _usersCount)
                {
                    _usersCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value != _password)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (value != _newPassword)
                {
                    _newPassword = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                if (value != _group)
                {
                    _group = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand ChangeSource { get; set; }
        public ICommand OpenItemsWindow { get; set; }
        public ICommand OpenClientsWindow { get; set; }
        public ICommand OpenBillsWindow { get; set; }
        public ICommand OpenSalesBillsWindow { get; set; }
        public ICommand OpenShortagesWindow { get; set; }
        public ICommand OpenServicesWindow { get; set; }
        public ICommand OpenSuppliersWindow { get; set; }
        public ICommand OpenCardsWindow { get; set; }
        public ICommand OpenCompaniesWindow { get; set; }
        public ICommand OpenSalesMenWindow { get; set; }
        public ICommand TakeBackup { get; set; }
        public ICommand RestoreBackup { get; set; }
        public ICommand OpenStoreInfoWindow { get; set; }
        public ICommand OpenNumbersWindow { get; set; }
        public ICommand OpenUsersWindow { get; set; }
        public ICommand OpenBarcodesWindow { get; set; }
        public ICommand SignOut { get; set; }
        public ICommand SaveUser { get; set; }

        Users.LoginVM CurrentUser = new Users.LoginVM();

        MainWindowVM v = new MainWindowVM();

        MainWindow Message = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        DispatcherTimer Timer = new DispatcherTimer();

        public MainPageVM()
        {
            LoadCommands();
            Timer.Tick += Timer_Tick;
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Start();
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var u = db.GetCollection<User>(DBCollections.Users.ToString()).Find(x => x.Id == CurrentUser.Id).FirstOrDefault();
                UserName = u.Name;
                Phone = u.Phone;
                Group = Enumerations.GetEnumDescription(u.Group);
            }
        }

        public void LoadCommands()
        {
            OpenItemsWindow = new CustomCommand(DoOpenItemsWindow, CanOpenItemsWindow);
            OpenClientsWindow = new CustomCommand(DoOpenClientsWindow, CanOpenClientsWindow);
            OpenBillsWindow = new CustomCommand(DoOpenBillsWindow, CanOpenBillsWindow);
            OpenSalesBillsWindow = new CustomCommand(DoOpenSalesBillsWindow, CanOpenSalesBillsWindow);
            OpenShortagesWindow = new CustomCommand(DoOpenShortagesWindow, CanOpenShortagesWindow);
            OpenServicesWindow = new CustomCommand(DoOpenServicesWindow, CanOpenServicesWindow);
            OpenSuppliersWindow = new CustomCommand(DoOpenSuppliersWindow, CanOpenSuppliersWindow);
            OpenCardsWindow = new CustomCommand(DoOpenCardsWindow, CanOpenCardsWindow);
            OpenCompaniesWindow = new CustomCommand(DoOpenCompaniesWindow, CanOpenCompaniesWindow);
            OpenSalesMenWindow = new CustomCommand(DoOpenSalesMenWindow, CanOpenSalesMenWindow);
            TakeBackup = new CustomCommand(DoTakeBackup, CanTakeBackup);
            RestoreBackup = new CustomCommand(DoRestoreBackup, CanRestoreBackup);
            OpenStoreInfoWindow = new CustomCommand(DoOpenStoreInfoWindow, CanOpenStoreInfoWindow);
            OpenNumbersWindow = new CustomCommand(DoOpenNumbersWindow, CanOpenNumbersWindow);
            OpenUsersWindow = new CustomCommand(DoOpenUsersWindow, CanOpenUsersWindow);
            OpenBarcodesWindow = new CustomCommand(DoOpenBarcodesWindow, CanOpenBarcodesWindow);
            SignOut = new CustomCommand(DoSignOut, CanSignOut);
            SaveUser = new CustomCommand(DoSaveUser, CanSaveUser);
        }

        async Task CountEveryThing()
        {
            if (isBacking)
            {
                return;
            }
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        ItemsCount = db.GetCollection<Item>(DBCollections.Items.ToString()).Count(x => x.Group == ItemGroup.Other);
                    });
                    await Task.Run(() =>
                    {
                        ClientsCount = db.GetCollection<Client>(DBCollections.Clients.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        ShortagesCount = db.GetCollection<Item>(DBCollections.Items.ToString()).Count(x => x.QTY == 0);
                    });
                    await Task.Run(() =>
                    {
                        ServicesCount = db.GetCollection<Service>(DBCollections.Services.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        SuppliersCount = db.GetCollection<Supplier>(DBCollections.Suppliers.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        CardsCount = db.GetCollection<Item>(DBCollections.Items.ToString()).Count(x => x.Group == ItemGroup.Card);
                    });
                    await Task.Run(() =>
                    {
                        CompaniesCount = db.GetCollection<Company>(DBCollections.Companies.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        SalesMenCount = db.GetCollection<SalesMan>(DBCollections.SalesMen.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        NumbersCount = db.GetCollection<Note>(DBCollections.Notes.ToString()).Count();
                    });
                    await Task.Run(() =>
                    {
                        UsersCount = db.GetCollection<User>(DBCollections.Users.ToString()).Count();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await CountEveryThing();
        }

        private bool CanOpenBarcodesWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            return false;
        }

        private void DoOpenBarcodesWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<BarCodes>().Count();
            if (opened == 0)
            {
                new BarCodes().Show();
            }
            else
            {
                Application.Current.Windows.OfType<BarCodes>().FirstOrDefault().Activate();
            }
        }

        private bool CanSaveUser(object obj)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }
            return true;
        }

        private async void DoSaveUser(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var userCol = db.GetCollection<User>(DBCollections.Users.ToString());
                User u = null;
                await Task.Run(() =>
                {
                    u = userCol.Find(x => x.Name == UserName).FirstOrDefault();
                });
                if (u == null)
                {
                    await Message.ShowMessageAsync("خطا", "تاكد من اسم المستخدم و ان كلمه المرور الحاليه صحيحة");
                }
                else if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    u.Name = UserName;
                    u.Phone = Phone;
                }
                else
                {
                    if (SecurePasswordHasher.Verify(Password, u.Pass))
                    {
                        u.Name = UserName;
                        u.Pass = SecurePasswordHasher.Hash(NewPassword);
                        u.Phone = Phone;
                    }
                }
                userCol.Update(u);
                Password = null;
                NewPassword = null;
                await Message.ShowMessageAsync("تمت", "تم تعديل بيانات المستخدم بنجاح");
            }
        }

        private bool CanSignOut(object obj)
        {
            return true;
        }

        private void DoSignOut(object obj)
        {
            CurrentUser.Id = 0;
            CurrentUser.Name = null;
            CurrentUser.SecurePassword = null;
            CurrentUser.Group = UserGroup.None;
            v.PageName = "Users/Login";
        }

        private bool CanOpenSalesBillsWindow(object obj)
        {
            return true;
        }

        private void DoOpenSalesBillsWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<SalesBillsViewer>().Count();
            if (opened == 0)
            {
                new SalesBillsViewer().Show();
            }
            else
            {
                Application.Current.Windows.OfType<SalesBillsViewer>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenBillsWindow(object obj)
        {
            return true;
        }

        private void DoOpenBillsWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Bills>().Count();
            if (opened == 0)
            {
                new Bills().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Bills>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenUsersWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            return false;
        }

        private void DoOpenUsersWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<View.Users>().Count();
            if (opened == 0)
            {
                new View.Users().Show();
            }
            else
            {
                Application.Current.Windows.OfType<View.Users>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenNumbersWindow(object obj)
        {
            return true;
        }

        private void DoOpenNumbersWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Notes>().Count();
            if (opened == 0)
            {
                new Notes().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Notes>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenStoreInfoWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            return false;
        }

        private void DoOpenStoreInfoWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Stores>().Count();
            if (opened == 0)
            {
                new Stores().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Stores>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenSalesMenWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            return false;
        }

        private void DoOpenSalesMenWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<SalesMen>().Count();
            if (opened == 0)
            {
                new SalesMen().Show();
            }
            else
            {
                Application.Current.Windows.OfType<SalesMen>().FirstOrDefault().Activate();
            }
        }

        private bool CanRestoreBackup(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            return false;
        }

        private async void DoRestoreBackup(object obj)
        {
            isBacking = true;
            var progressbar = await Message.ShowProgressAsync("استرجع نسخه احتياطية", "جارى استعادة نسخه احتياطية الان");
            progressbar.SetIndeterminate();
            try
            {
                var dlg = new CommonOpenFileDialog();
                dlg.Title = "اختار نسخه احتياطية لاسترجعها";
                dlg.IsFolderPicker = false;
                dlg.InitialDirectory = Properties.Settings.Default.BackUpsFolder;
                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.Filters.Add(new CommonFileDialogFilter("Backup file", "*.bak"));
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;
                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    File.Copy(dlg.FileName, Properties.Settings.Default.DBFullName, true);
                    await Message.ShowMessageAsync("تمت العملية", "تم استرجاع النسخه الاحتياطية بنجاح");
                }
            }
            catch (Exception ex)
            {
                await progressbar.CloseAsync();
                Core.SaveException(ex);
            }
            finally
            {
                if (progressbar.IsOpen)
                {
                    await progressbar.CloseAsync();
                }
                isBacking = false;
            }
        }

        private bool CanTakeBackup(object obj)
        {
            return true;
        }

        private async void DoTakeBackup(object obj)
        {
            isBacking = true;
            var progressbar = await Message.ShowProgressAsync("اخذ نسخه احتياطية", "جارى اخذ نسخه احتياطية الان");
            progressbar.SetIndeterminate();
            try
            {
                var dlg = new CommonOpenFileDialog();
                dlg.Title = "اختار مكان لحفظ النسخه الاحتياطية";
                dlg.IsFolderPicker = true;
                dlg.InitialDirectory = Properties.Settings.Default.BackUpsFolder;
                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;
                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Properties.Settings.Default.BackUpsFolder = dlg.FileName;
                    if (!Properties.Settings.Default.BackUpsFolder.EndsWith("\\"))
                    {
                        Properties.Settings.Default.BackUpsFolder += "\\";
                    }
                    Properties.Settings.Default.Save();
                    File.Copy(Properties.Settings.Default.DBFullName, $"{Properties.Settings.Default.BackUpsFolder}PhonyDbBackup {DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.bak");
                    await Message.ShowMessageAsync("تمت العملية", "تم اخذ نسخه احتياطية بنجاح");
                }
            }
            catch (Exception ex)
            {
                await progressbar.CloseAsync();
                await Core.SaveExceptionAsync(ex);
                await Message.ShowMessageAsync("مشكله", "هناك مشكله فى حفظ النسخه الاحتياطية جرب مكان اخر");
            }
            finally
            {
                if (progressbar.IsOpen)
                {
                    await progressbar.CloseAsync();
                }
                isBacking = false;
            }
        }

        private bool CanOpenCompaniesWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoOpenCompaniesWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Companies>().Count();
            if (opened == 0)
            {
                new Companies().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Companies>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenCardsWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoOpenCardsWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Cards>().Count();
            if (opened == 0)
            {
                new Cards().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Cards>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenSuppliersWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoOpenSuppliersWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Suppliers>().Count();
            if (opened == 0)
            {
                new Suppliers().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Suppliers>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenServicesWindow(object obj)
        {
            if (CurrentUser.Group == UserGroup.Manager)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoOpenServicesWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Services>().Count();
            if (opened == 0)
            {
                new Services().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Services>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenShortagesWindow(object obj)
        {
            return true;
        }

        private void DoOpenShortagesWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Shortages>().Count();
            if (opened == 0)
            {
                new Shortages().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Shortages>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenClientsWindow(object obj)
        {
            return true;
        }

        private void DoOpenClientsWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Clients>().Count();
            if (opened == 0)
            {
                new Clients().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Clients>().FirstOrDefault().Activate();
            }
        }

        private bool CanOpenItemsWindow(object obj)
        {
            return true;
        }

        private void DoOpenItemsWindow(object obj)
        {
            var opened = Application.Current.Windows.OfType<Items>().Count();
            if (opened == 0)
            {
                new Items().Show();
            }
            else
            {
                Application.Current.Windows.OfType<Items>().FirstOrDefault().Activate();
            }
        }
    }
}