﻿using Linphone.Model;
using System;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Linphone
{
    /// <summary>
    /// Specific listener for any view which want to be notified when the mute state changes.
    /// </summary>
    public interface MuteChangedListener
    {
        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        void MuteStateChanged(bool isMicMuted);
    }

    /// <summary>
    /// Specific listener for any view which want to be notified when the pause state changes.
    /// </summary>
    public interface PauseChangedListener
    {
        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        void PauseStateChanged(bool isMicMuted);
    }

    /// <summary>
    /// Model view for each page implementing the call controller listener to adjust displayed page depending on call events.
    /// </summary>
    public class BaseModel : INotifyPropertyChanged, CallControllerListener
    {
        /// <summary>
        /// Specific listener for any view which want to be notified when the mute state changes.
        /// </summary>
        public MuteChangedListener MuteListener { get; set; }

        /// <summary>
        /// Specific listener for any view which want to be notified when the pause state changes.
        /// </summary>
        public PauseChangedListener PauseListener { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public BaseModel()
        {

        }

        /// <summary>
        /// Page currently displayed.
        /// </summary>
        public BasePage Page { get; set; }

        /// <summary>
        /// Dispatcher used to run tasks on the UI thread.
        /// </summary>
        public static Dispatcher UIDispatcher;

        /// <summary>
        /// Called when a call is starting.
        /// Displays the InCall.xaml page.
        /// </summary>
        /// <param name="callerNumber"></param>
        public void NewCallStarted(string callerNumber)
        {
            this.Page.NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + callerNumber, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Called when a call is finished.
        /// Goes back to the last page if possible, else displays Dialer.xaml.
        /// </summary>
        public void CallEnded()
        {
            Logger.Msg("[CallListener] Call ended, can go back ? " + this.Page.NavigationService.CanGoBack);

            if (this.Page.NavigationService.CanGoBack)
                this.Page.NavigationService.GoBack();
            else
            {
                //If incall view directly accessed from home page, backstack is empty
                //If so, instead of keeping the incall view, launch the Dialer and remove the incall view from the backstack
                this.Page.NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                this.Page.NavigationService.RemoveBackEntry();
            }
        }

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        public void MuteStateChanged(Boolean isMicMuted)
        {
            if (this.MuteListener != null)
                this.MuteListener.MuteStateChanged(isMicMuted);
        }

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        public void PauseStateChanged(bool isCallPaused)
        {
            if (this.PauseListener != null)
                this.PauseListener.PauseStateChanged(isCallPaused);
        }

        /// <summary>
        /// Actualises the listener when the pages changes.
        /// </summary>
        public virtual void OnNavigatedTo(NavigationEventArgs nea)
        {
            LinphoneManager.Instance.CallListener = this;
            UIDispatcher = this.Page.Dispatcher;
        }

        /// <summary>
        /// Actualises the listener when the pages changes.
        /// </summary>
        public virtual void OnNavigatedFrom(NavigationEventArgs nea)
        {
            LinphoneManager.Instance.CallListener = null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
