﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DccDecoder.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Defines the DccDecoder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DccControllersLibNetStandard
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class DccDecoder : INotifyPropertyChanged
    {
        private const int ShortAddressCV = 1;

        private readonly RelayCommand toggleLightsCommand;

        private Action<string> sendCommandDelegate = null;

        private int address = 0;

        private int newAddress = 0;

        private string name = "New Loco";

        /// <summary>
        /// The last speed value given send to the decoder
        /// 0 - 126 (-1 for emergency stop)
        /// </summary>
        private int speed = 0;

        /// <summary>
        /// 1 = Forward, 0 = Reverse
        /// </summary>
        private int direction = 1;

        private bool lights = false;


        public int Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
                this.NewAddress = value;
                this.OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The last speed value given send to the decoder
        /// 0 - 126 (-1 for emergency stop)
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                this.UpdateLocomotion();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 1 = Forward, 0 = Reverse
        /// </summary>
        public int Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.UpdateLocomotion();

            }
        }

        private void UpdateLocomotion()
        {
            this.sendCommandDelegate?.Invoke($"<t 1 {this.address} {this.speed} {this.direction}>");
        }

        private void ReassignAddress()
        {
            this.sendCommandDelegate?.Invoke($"<w {this.address} {ShortAddressCV} {this.newAddress}>");
            this.sendCommandDelegate?.Invoke($"<W {this.address} {ShortAddressCV} {this.newAddress}>");

        }

        public bool Lights
        {
            get { return lights; }
            set { lights = value; }
        }

        public RelayCommand ToggleLightsCommand
        {
            get { return toggleLightsCommand; }
        }

        public int NewAddress
        {
            get => this.newAddress;
            set
            {
                this.newAddress = value;
                this.OnPropertyChanged();
            }
        }

        public DccDecoder(Action<string> sendCommandDelegate)
        {
            this.sendCommandDelegate = sendCommandDelegate;
            this.toggleLightsCommand = new RelayCommand(this.ToggleLights);
        }

        private void ToggleLights()
        {
            lights = !lights;

            if (lights)
            {
                this.sendCommandDelegate?.Invoke($"<f {this.address} 144>");
            }
            else
            {
                this.sendCommandDelegate?.Invoke($"<f {this.address} 122>");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
