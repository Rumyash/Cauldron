﻿using Couldron.Core;
using Couldron.Validation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Couldron.ViewModels
{
    /// <summary>
    /// Represents the Base class of a ViewModel
    /// </summary>
    public abstract class ViewModelBase : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        [InjectionConstructor]
        public ViewModelBase()
        {
            this.Id = Guid.NewGuid();
            this.Dispatcher = new CouldronDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelBase"/>
        /// </summary>
        /// <param name="id">A unique identifier of the viewmodel</param>
        public ViewModelBase(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> this <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public CouldronDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets the unique Id of the view model
        /// </summary>
        [SuppressIsChanged]
        public Guid Id { get; private set; }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        public async void RaiseNotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.OnBeforeRaiseNotifyPropertyChanged(propertyName))
                return;

            if (this.PropertyChanged != null)
                await this.Dispatcher.RunAsync(() => this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));

            this.OnAfterRaiseNotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occures after the event <see cref="PropertyChanged"/> has been invoked
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        protected virtual void OnAfterRaiseNotifyPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Occured before the <see cref="PropertyChanged"/> event is invoked.
        /// </summary>
        /// <param name="propertyName">The name of the property where the value change has occured</param>
        /// <returns>Returns true if <see cref="RaiseNotifyPropertyChanged(string)"/> should be cancelled. Otherwise false</returns>
        protected virtual bool OnBeforeRaiseNotifyPropertyChanged(string propertyName)
        {
            return false;
        }
    }
}