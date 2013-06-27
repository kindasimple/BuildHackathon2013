﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiveCycle.Resources;
using LiveCycle.Data;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using System.Device.Location;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using LiveCycle.Models;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace LiveCycle
{
    public partial class MainPage : PhoneApplicationPage
    {
        Landmark _activeLocation;

        public DefaultViewModel DefaultViewModel
        {
            get { return (DefaultViewModel)GetValue(DefaultViewModelProperty); }
            set { SetValue(DefaultViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(DefaultViewModel), typeof(MainPage), new PropertyMetadata(new DefaultViewModel()));

        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DefaultViewModel.DesignTimeSetup();


            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        Geolocator geolocator;

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.MovementThreshold = 50;
            geolocator.StatusChanged += geolocator_StatusChanged;
            geolocator.PositionChanged += geolocator_PositionChanged;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            geolocator.StatusChanged -= geolocator_StatusChanged;
            geolocator.PositionChanged -= geolocator_PositionChanged;
            geolocator = null;

            base.OnNavigatedFrom(e);
        }


        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.map.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                ShowNearestPOI(this.map.Center);
                this.map.ZoomLevel = 12;
                Dispatcher.BeginInvoke(() => SetupMyLocation());
            });
        }

        private void ShowNearestPOI(GeoCoordinate geoCoordinate)
        {
            var nearest = this.DefaultViewModel.Landmarks.OrderBy(x => geoCoordinate.GetDistanceTo(x.Geocoordinate)).First();
            var distance = geoCoordinate.GetDistanceTo(nearest.Geocoordinate);
            double notificationThreshold = 8000;
            if (distance < notificationThreshold && _activeLocation != nearest)
            {
                _activeLocation = nearest;
                ShowLandmark(nearest);
            }
        }

        private void ShowLandmark(Landmark landmark)
        {
            this.LandmarkName.Text = landmark.Name;
            var imageUri = new Uri(landmark.ImageSource, UriKind.Relative);
            BitmapImage bi = new BitmapImage(imageUri);
            this.LandmarkImage.Source = bi;
            (this.Resources["ShowLandmark"] as Storyboard).Begin();
        }

        Ellipse myCircle;

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off 
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation 
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location 
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters 
                    status = "ready";
                    if (myCircle == null)
                    {
                        Dispatcher.BeginInvoke(() => SetupMyLocation());
                    }
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information 
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state 

                    break;
            }
        }

        MapOverlay myLocationOverlay;
        MapLayer myLocationLayer;
        private void SetupMyLocation()
        {
            if (myCircle == null)
            {
                myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Red);
                myCircle.Height = 20;
                myCircle.Width = 20;
                myCircle.Opacity = 50;

                myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.GeoCoordinate = map.Center;

                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);

                myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);
                map.Layers.Add(myLocationLayer);

            }

            myLocationOverlay.GeoCoordinate = map.Center;

            // Add the MapLayer to the Map.
        }


        private void map_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "LiveCycle";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "AoV7HS_tghDBN2FVszDWjtNp6-XXaIBzFxCiJ7-CTiynSF94EPLo9abG4vY8yx3A";
        }


        

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}