using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    class AddRoomTypes : BaseViewModel
    {
        private ObservableCollection<RoomType> _RoomTypesList;
        public ObservableCollection<RoomType> RoomTypesList { get => _RoomTypesList; set { _RoomTypesList = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomType> _FilteredList;
        public ObservableCollection<RoomType> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private RoomType _TypeFilter;
        public RoomType TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private decimal? _Price;
        public decimal? Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }

        private RoomType _SelectedItem;
        public RoomType SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    Name = SelectedItem.Name;
                    Price = SelectedItem.Price;
                }
                OnPropertyChanged();
            }
        }
        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }
        public AddRoomTypes()
        {
            LoadRoomTypesList();
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name)||  Price == 0 || Price == null)
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.Name == Name);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var roomType = new RoomType()
                {
                    Name = Name,
                    Price = Price,
                };
                DataProvider.Instance.DB.RoomTypes.Add(roomType);
                DataProvider.Instance.DB.SaveChanges();
                LoadRoomTypesList();
                ClearFields();
            });
            editCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || Price == 0 || Price == null)
                {
                    return false;
                }
                if(SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var roomType = DataProvider.Instance.DB.RoomTypes.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                roomType.Name = Name;
                roomType.Price = Price;

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadRoomTypesList();
                ClearFields();
            });
            deleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var roomType = DataProvider.Instance.DB.RoomTypes.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                roomType.Deleted = true;

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadRoomTypesList();
                ClearFields();
            });
        }

        private void LoadRoomTypesList()
        {
            RoomTypesList = new ObservableCollection<RoomType>();
            RoomTypesList.Add(new RoomType() { Name = null });
            var roomTypesList = DataProvider.Instance.DB.RoomTypes.Where(x => x.Deleted == false);
            foreach (var item in roomTypesList)
            {
                RoomTypesList.Add(item);
            }
            LoadFilteredList();
        }
        private void LoadFilteredList()
        {
            ObservableCollection<RoomType> list = new ObservableCollection<RoomType>();
            foreach (var item in RoomTypesList)
            {
                if (item.Name == null)
                    continue;
                if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && (TypeFilter == null || TypeFilter.Name == null))
                {
                    list.Add(item);
                }
                else if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && (TypeFilter != null))
                {
                    if (item.Name == TypeFilter.Name)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    switch (Filter)
                    {
                        case "ID":
                            if (string.IsNullOrEmpty(SearchString) || (item.ID == Convert.ToInt32(SearchString)) && (TypeFilter == null || TypeFilter.Name == null || item.Name == TypeFilter.Name))
                            {
                                list.Add(item);
                            }
                            break;

                        case "Name":
                            if ((string.IsNullOrEmpty(SearchString) || item.Name.Contains(SearchString)) && (TypeFilter == null || TypeFilter.Name == null || item.Name == TypeFilter.Name))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Price":
                            if ((string.IsNullOrEmpty(SearchString) || item.Price == Convert.ToDecimal(SearchString)) && (TypeFilter == null || TypeFilter.Name == null || item.Name == TypeFilter.Name))
                            {
                                list.Add(item);
                            }
                            break;
                    }
                }
            }
            FilteredList = list;
        }
        private void ClearFields()
        {
            Name = null;
            Price = null;
        }
    }
}
