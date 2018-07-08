﻿using LiteDB;
using MahApps.Metro.Controls.Dialogs;
using Phony.Kernel;
using Phony.Model;
using Phony.Utility;
using Phony.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Phony.ViewModel
{
    public class ClientVM : CommonBase
    {
        long _clientsId;
        string _name;
        string _site;
        string _email;
        string _phone;
        string _notes;
        string _searchText;
        string _childName;
        string _childPrice;
        static string _clientsCount;
        static string _clientsPurchasePrice;
        static string _clientsSalePrice;
        static string _clientsProfit;
        decimal _balance;
        bool _fastResult;
        bool _openFastResult;
        bool _isAddClientFlyoutOpen;
        Client _dataGridSelectedClient;

        ObservableCollection<Client> _clients;

        public long ClientId
        {
            get => _clientsId;
            set
            {
                if (value != _clientsId)
                {
                    _clientsId = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Site
        {
            get => _site;
            set
            {
                if (value != _site)
                {
                    _site = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (value != _email)
                {
                    _email = value;
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

        public string Notes
        {
            get => _notes;
            set
            {
                if (value != _notes)
                {
                    _notes = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ChildName
        {
            get => _childName;
            set
            {
                if (value != _childName)
                {
                    _childName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ChildPrice
        {
            get => _childPrice;
            set
            {
                if (value != _childPrice)
                {
                    _childPrice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ClientCount
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

        public string ClientCredits
        {
            get => _clientsPurchasePrice;
            set
            {
                if (value != _clientsPurchasePrice)
                {
                    _clientsPurchasePrice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ClientDebits
        {
            get => _clientsSalePrice;
            set
            {
                if (value != _clientsSalePrice)
                {
                    _clientsSalePrice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ClientProfit
        {
            get => _clientsProfit;
            set
            {
                if (value != _clientsProfit)
                {
                    _clientsProfit = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Balance
        {
            get => _balance;
            set
            {
                if (value != _balance)
                {
                    _balance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool FastResult
        {
            get => _fastResult;
            set
            {
                if (value != _fastResult)
                {
                    _fastResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool OpenFastResult
        {
            get => _openFastResult;
            set
            {
                if (value != _openFastResult)
                {
                    _openFastResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsAddClientFlyoutOpen
        {
            get => _isAddClientFlyoutOpen;
            set
            {
                if (value != _isAddClientFlyoutOpen)
                {
                    _isAddClientFlyoutOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Client DataGridSelectedClient
        {
            get => _dataGridSelectedClient;
            set
            {
                if (value != _dataGridSelectedClient)
                {
                    _dataGridSelectedClient = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                if (value != _clients)
                {
                    _clients = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<User> Users { get; set; }

        public ICommand Search { get; set; }
        public ICommand OpenAddClientFlyout { get; set; }
        public ICommand FillUI { get; set; }
        public ICommand ClientPay { get; set; }
        public ICommand PayClient { get; set; }
        public ICommand ReloadAllClients { get; set; }
        public ICommand AddClient { get; set; }
        public ICommand EditClient { get; set; }
        public ICommand DeleteClient { get; set; }

        Users.LoginVM CurrentUser = new Users.LoginVM();

        Clients ClientsMessage = Application.Current.Windows.OfType<Clients>().FirstOrDefault();

        public ClientVM()
        {
            LoadCommands();
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                Clients = new ObservableCollection<Client>(db.GetCollection<Client>(DBCollections.Clients.ToString()).FindAll());
                Users = new ObservableCollection<User>(db.GetCollection<User>(DBCollections.Users.ToString()).FindAll());
            }
            DebitCredit();
        }

        public void LoadCommands()
        {
            Search = new CustomCommand(DoSearch, CanSearch);
            OpenAddClientFlyout = new CustomCommand(DoOpenAddClientFlyout, CanOpenAddClientFlyout);
            FillUI = new CustomCommand(DoFillUI, CanFillUI);
            ClientPay = new CustomCommand(DoClientPayAsync, CanClientPay);
            PayClient = new CustomCommand(DoPayClientAsync, CanPayClient);
            ReloadAllClients = new CustomCommand(DoReloadAllClients, CanReloadAllClients);
            AddClient = new CustomCommand(DoAddClient, CanAddClient);
            EditClient = new CustomCommand(DoEditClient, CanEditClient);
            DeleteClient = new CustomCommand(DoDeleteClient, CanDeleteClient);
        }

        async void DebitCredit()
        {
            decimal Debit = decimal.Round(Clients.Where(c => c.Balance > 0).Sum(i => i.Balance), 2);
            decimal Credit = decimal.Round(Clients.Where(c => c.Balance < 0).Sum(i => i.Balance), 2);
            await Task.Run(() =>
            {
                ClientCount = $"مجموع العملاء: {Clients.Count().ToString()}";
            });
            await Task.Run(() =>
            {
                ClientDebits = $"اجمالى لينا: {Math.Abs(Debit).ToString()}";
            });
            await Task.Run(() =>
            {
                ClientCredits = $"اجمالى علينا: {Math.Abs(Credit).ToString()}";
            });
            await Task.Run(() =>
            {
                ClientProfit = $"تقدير لصافى لينا: {(Math.Abs(Debit) - Math.Abs(Credit)).ToString()}";
            });
        }

        private bool CanAddClient(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            return true;
        }

        private void DoAddClient(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var exist = db.GetCollection<Client>(DBCollections.Clients.ToString()).Find(x => x.Name == Name).FirstOrDefault();
                if (exist == null)
                {
                    var c = new Client
                    {
                        Name = Name,
                        Balance = Balance,
                        Site = Site,
                        Email = Email,
                        Phone = Phone,
                        Notes = Notes,
                        CreateDate = DateTime.Now,
                        CreatedById = CurrentUser.Id,
                        EditDate = null,
                        EditById = null
                    };
                    db.GetCollection<Client>(DBCollections.Clients.ToString()).Insert(c);
                    Clients.Add(c);
                    ClientsMessage.ShowMessageAsync("تمت العملية", "تم اضافة العميل بنجاح");
                    DebitCredit();
                }
                else
                {
                    ClientsMessage.ShowMessageAsync("موجود", "هناك عميل بنفس الاسم بالفعل");
                }
            }
        }

        private bool CanEditClient(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name) || ClientId == 0 || DataGridSelectedClient == null)
            {
                return false;
            }
            return true;
        }

        private void DoEditClient(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                var c = db.GetCollection<Client>(DBCollections.Clients.ToString()).FindById(DataGridSelectedClient.Id);
                c.Name = Name;
                c.Balance = Balance;
                c.Site = Site;
                c.Email = Email;
                c.Phone = Phone;
                c.Notes = Notes;
                c.EditDate = DateTime.Now;
                c.EditById = CurrentUser.Id;
                db.GetCollection<Client>(DBCollections.Clients.ToString()).Update(c);
                ClientsMessage.ShowMessageAsync("تمت العملية", "تم تعديل العميل بنجاح");
                Clients[Clients.IndexOf(DataGridSelectedClient)] = c;
                DebitCredit();
                DataGridSelectedClient = null;
                ClientId = 0;
            }
        }

        private bool CanDeleteClient(object obj)
        {
            if (DataGridSelectedClient == null || DataGridSelectedClient.Id == 1)
            {
                return false;
            }
            return true;
        }

        private async void DoDeleteClient(object obj)
        {
            var result = await ClientsMessage.ShowMessageAsync("حذف الصنف", $"هل انت متاكد من حذف العميل {DataGridSelectedClient.Name}", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                {
                    db.GetCollection<Client>(DBCollections.Clients.ToString()).Delete(DataGridSelectedClient.Id);
                    Clients.Remove(DataGridSelectedClient);
                }
                await ClientsMessage.ShowMessageAsync("تمت العملية", "تم حذف العميل بنجاح");
                DebitCredit();
                DataGridSelectedClient = null;
            }
        }

        private bool CanSearch(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return false;
            }
            return true;
        }

        private void DoSearch(object obj)
        {
            try
            {
                using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                {
                    Clients = new ObservableCollection<Client>(db.GetCollection<Client>(DBCollections.Clients.ToString()).Find(x => x.Name.Contains(SearchText)));
                    if (Clients.Count > 0)
                    {
                        if (FastResult)
                        {
                            ChildName = Clients.FirstOrDefault().Name;
                            ChildPrice = Clients.FirstOrDefault().Balance.ToString();
                            OpenFastResult = true;
                        }
                    }
                    else
                    {
                        ClientsMessage.ShowMessageAsync("غير موجود", "لم يتم العثور على شئ");
                    }
                }
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                BespokeFusion.MaterialMessageBox.ShowError("لم يستطع ايجاد ما تبحث عنه تاكد من صحه البيانات المدخله");
            }
        }

        private bool CanReloadAllClients(object obj)
        {
            return true;
        }

        private void DoReloadAllClients(object obj)
        {
            using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
            {
                Clients = new ObservableCollection<Client>(db.GetCollection<Client>(DBCollections.Clients.ToString()).FindAll());
            }
        }

        private bool CanFillUI(object obj)
        {
            if (DataGridSelectedClient == null)
            {
                return false;
            }
            return true;
        }

        private void DoFillUI(object obj)
        {
            ClientId = DataGridSelectedClient.Id;
            Name = DataGridSelectedClient.Name;
            Balance = DataGridSelectedClient.Balance;
            Site = DataGridSelectedClient.Site;
            Email = DataGridSelectedClient.Email;
            Phone = DataGridSelectedClient.Phone;
            Notes = DataGridSelectedClient.Notes;
            IsAddClientFlyoutOpen = true;
        }

        private bool CanClientPay(object obj)
        {
            if (DataGridSelectedClient == null)
            {
                return false;
            }
            return true;
        }

        private async void DoClientPayAsync(object obj)
        {
            var result = await ClientsMessage.ShowInputAsync("تدفيع", $"ادخل المبلغ الذى تريد خصمه من حساب العميل {DataGridSelectedClient.Name}");
            if (string.IsNullOrWhiteSpace(result))
            {
                await ClientsMessage.ShowMessageAsync("ادخل مبلغ", "لم تقم بادخال اى مبلغ لتدفيعه");
            }
            else
            {
                bool isvalidmoney = decimal.TryParse(result, out decimal clientpaymentamount);
                if (isvalidmoney)
                {
                    using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                    {
                        var c = db.GetCollection<Client>(DBCollections.Clients.ToString()).FindById(DataGridSelectedClient.Id);
                        c.Balance -= clientpaymentamount;
                        c.EditDate = DateTime.Now;
                        c.EditById = CurrentUser.Id;
                        db.GetCollection<ClientMove>(DBCollections.ClientsMoves.ToString()).Insert(new ClientMove
                        {
                            ClientId = DataGridSelectedClient.Id,
                            Credit = clientpaymentamount,
                            CreateDate = DateTime.Now,
                            CreatedById = CurrentUser.Id,
                            EditDate = null,
                            EditById = null
                        });
                        if (clientpaymentamount > 0)
                        {
                            db.GetCollection<TreasuryMove>(DBCollections.TreasuriesMoves.ToString()).Insert(new TreasuryMove
                            {
                                TreasuryId = 1,
                                Debit = clientpaymentamount,
                                Notes = $"استلام من العميل بكود {DataGridSelectedClient.Id} باسم {DataGridSelectedClient.Name}",
                                CreateDate = DateTime.Now,
                                CreatedById = CurrentUser.Id
                            });
                        }
                        await ClientsMessage.ShowMessageAsync("تمت العملية", $"تم تدفيع {DataGridSelectedClient.Name} مبلغ {clientpaymentamount} جنية بنجاح");
                        Clients[Clients.IndexOf(DataGridSelectedClient)] = c;
                        DebitCredit();
                        ClientId = 0;
                        DataGridSelectedClient = null;
                    }
                }
                else
                {
                    await ClientsMessage.ShowMessageAsync("خطاء فى المبلغ", "ادخل مبلغ صحيح بعلامه عشرية واحدة");
                }
            }
        }

        private bool CanPayClient(object obj)
        {
            if (DataGridSelectedClient == null)
            {
                return false;
            }
            return true;
        }

        private async void DoPayClientAsync(object obj)
        {
            var result = await ClientsMessage.ShowInputAsync("تدفيع", $"ادخل المبلغ الذى تريد اضافته لحساب للعميل {DataGridSelectedClient.Name}");
            if (string.IsNullOrWhiteSpace(result))
            {
                await ClientsMessage.ShowMessageAsync("ادخل مبلغ", "لم تقم بادخال اى مبلغ لتدفيعه");
            }
            else
            {
                bool isvalidmoney = decimal.TryParse(result, out decimal clientpaymentamount);
                if (isvalidmoney)
                {
                    using (var db = new LiteDatabase(Properties.Settings.Default.DBFullName))
                    {
                        var c = db.GetCollection<Client>(DBCollections.Clients.ToString()).FindById(DataGridSelectedClient.Id);
                        c.Balance += clientpaymentamount;
                        c.EditDate = DateTime.Now;
                        c.EditById = CurrentUser.Id;
                        db.GetCollection<ClientMove>(DBCollections.ClientsMoves.ToString()).Insert(new ClientMove
                        {
                            ClientId = DataGridSelectedClient.Id,
                            Debit = clientpaymentamount,
                            CreateDate = DateTime.Now,
                            CreatedById = CurrentUser.Id,
                            EditDate = null,
                            EditById = null
                        });
                        db.GetCollection<TreasuryMove>(DBCollections.TreasuriesMoves.ToString()).Insert(new TreasuryMove
                        {
                            TreasuryId = 1,
                            Credit = clientpaymentamount,
                            Notes = $"تسليم المبلغ للعميل بكود {DataGridSelectedClient.Id} باسم {DataGridSelectedClient.Name}",
                            CreateDate = DateTime.Now,
                            CreatedById = CurrentUser.Id
                        });
                        await ClientsMessage.ShowMessageAsync("تمت العملية", $"تم دفع {DataGridSelectedClient.Name} مبلغ {clientpaymentamount} جنية بنجاح");
                        Clients[Clients.IndexOf(DataGridSelectedClient)] = c;
                        DebitCredit();
                        ClientId = 0;
                        DataGridSelectedClient = null;
                    }
                }
                else
                {
                    await ClientsMessage.ShowMessageAsync("خطاء فى المبلغ", "ادخل مبلغ صحيح بعلامه عشرية واحدة");
                }
            }
        }

        private bool CanOpenAddClientFlyout(object obj)
        {
            return true;
        }

        private void DoOpenAddClientFlyout(object obj)
        {
            if (IsAddClientFlyoutOpen)
            {
                IsAddClientFlyoutOpen = false;
            }
            else
            {
                IsAddClientFlyoutOpen = true;
            }
        }
    }
}