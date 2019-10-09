﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DccController.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Defines the DccController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DccControllersLibNetStandard
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SerialCommsStandard;

    public class DccController : INotifyPropertyChanged
    {
        private ObservableCollection<DccDecoder> decoders = new ObservableCollection<DccDecoder>();

        private SerialAdapter serialAdapter;

        private string selectedComPort = string.Empty;

        private const int BaudRate = 115200;

        private bool power = false;

        private RelayCommand togglePowerCommand;

        private Action<string> sendCommandDelegate = null;

        private Action<DccDecoder> editDecoderDelegate;

        public ObservableCollection<DccDecoder> Decoders
        {
            get
            {
                return this.decoders;
            }
            set
            {
                this.decoders = value;
            }
        }

        public string SelectedComPort
        {
            get
            {
                return this.selectedComPort;
            }
            set
            {
                this.selectedComPort = value;
                this.OnPropertyChanged();
                this.ConnectToPort();
            }
        }

        public bool Power
        {
            get { return power; }
            set
            {
                this.power = value;
                this.SendCommand(this.power ? "<1>" : "<0>");
            }
        }

        public RelayCommand TogglePowerCommand
        {
            get { return togglePowerCommand; }
            set { togglePowerCommand = value; }
        }

        public DccController(Action<string> sendCommandDelegate = null, Action<DccDecoder> editDecoderDelegate = null)
        {
            this.TogglePowerCommand = new RelayCommand(this.TogglePower);
            this.sendCommandDelegate = sendCommandDelegate;
            this.editDecoderDelegate = editDecoderDelegate;

            for (int i = 1; i < 7; i++)
            {
                DccDecoder decoder = new DccDecoder(this.SendCommand, this.editDecoderDelegate) { Address = i, Name = $"Loco {i}" };
                this.decoders.Add(decoder);
            }
        }

        private void TogglePower()
        {
            this.Power = !this.power;
        }

        private void SendCommand(string command)
        {
            this.serialAdapter?.WriteString(command);
            this.sendCommandDelegate?.Invoke(command);
        }

        private void ConnectToPort()
        {
            try
            {
                this.serialAdapter = new SerialAdapter(this.selectedComPort, BaudRate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ProcessBaseStationReply(string obj)
        {
            
        }
    }
}
