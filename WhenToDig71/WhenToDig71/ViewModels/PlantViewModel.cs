using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using WhenToDig71.Models;
using WhenToDig71.Views;
using Xamarin.Forms;

namespace WhenToDig71.ViewModels
{
    public class PlantViewModel : INotifyPropertyChanged
    {
      

        Realm _realmInstance = Realm.GetInstance();      

        #region constructures
        public PlantViewModel()
        {
            //var config = _realmInstance.Config;
            //Realm.DeleteRealm(config);
            //_realmInstance = Realm.GetInstance();

            Plants = _realmInstance.All<Plant>().ToList();
        }
        #endregion

        #region properties
        private List<Plant> _listOfPlants;
        public List<Plant> Plants
        {
            get { return _listOfPlants; }
            set
            {
                _listOfPlants = value;
                OnPropertyChanged(); // Added the OnPropertyChanged Method
            }
        }

        private Plant _plant = new Plant();
        public Plant Plant
        {
            get { return _plant; }
            set
            {
                _plant = value;
                OnPropertyChanged(); // Add the OnPropertyChanged();
            }
        }
        #endregion

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region commands
        public Command CreateCommand // for ADD
        {
            get
            {
                return new Command(() => {
                    // for auto increment the id upon adding
                    _plant.PlantId = _realmInstance.All<Plant>().Count() + 1;
                    _realmInstance.Write(() =>
                    {
                        _realmInstance.Add(_plant); // Add the whole set of details
                    });
                });
            }
        }

        public Command UpdateCommand // For UPDATE
        {
            get
            {
                return new Command(() =>
                {
                    // instantiate to supply the new set of details
                    var plantUpdate = new Plant
                    {
                        Name = _plant.Name,
                        Variety = _plant.Variety,
                        Notes = _plant.Notes
                    };

                    _realmInstance.Write(() =>
                    {
                        // when there's id match, the details will be replaced except by primary key
                        _realmInstance.Add(plantUpdate, update: true);
                    });
                });
            }
        }

        public Command RemoveCommand
        {
            get
            {
                return new Command(() =>
                {
                    // get the details with specific id
                    var getAllplantsById = _realmInstance.All<Plant>().First(x => x.PlantId == _plant.PlantId);

                    using (var transaction = _realmInstance.BeginWrite())
                    {
                        // remove all details
                        _realmInstance.Remove(getAllplantsById);
                        transaction.Commit();
                    };
                });
            }
        }
        #endregion

        #region navigation
        public Command NavToListCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await App.Current.MainPage.Navigation.PushAsync(new ListOfPlants());
                });
            }
        }
        public Command NavToCreateCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await App.Current.MainPage.Navigation.PushAsync(new CreatePlant());
                });
            }
        }
        public Command NavToUpdateDeleteCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await App.Current.MainPage.Navigation.PushAsync(new UpdateOrDeletePlant());
                });
            }
        }
        #endregion
    }
}
