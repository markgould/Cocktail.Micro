﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro.Extensions;
using Common.Errors;
using Common.Messages;
using DomainModel;
using Security;
using TempHire.ViewModels;

namespace TempHire
{
    public class AppBootstrapper : FrameworkBootstrapper<ShellViewModel>
    {
        static AppBootstrapper()
        {
            UsesFakeStore<TempHireEntities>();
        }

        // Automatically instantiate and hold all discovered MessageProcessors
        [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<IMessageProcessor> MessageProcessors { get; set; }

        [Import]
        public IErrorHandler ErrorHandler { get; set; }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ErrorHandler.HandleError(e.ExceptionObject);
            e.Handled = true;
        }
    }
}