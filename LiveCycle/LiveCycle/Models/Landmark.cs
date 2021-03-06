﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace LiveCycle.Models
{
    public class Landmark : INotifyPropertyChanged
    {
        public GeoCoordinate Geocoordinate { get; set; }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }
        #region ImageSource
        private string _ImageSource;
        public string ImageSource
        {
            get { return _ImageSource; }
            set { _ImageSource = value; OnPropertyChanged(); }
        }
        #endregion

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
