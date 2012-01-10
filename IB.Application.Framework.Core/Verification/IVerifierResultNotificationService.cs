﻿//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using IdeaBlade.Core.Composition;
using IdeaBlade.Validation;

namespace IdeaBlade.Application.Framework.Core.Verification
{
    /// <summary>Implement this interface to be notified of validation errors during a save. The framework automatically performs validation before saving changed
    /// entities. If any validation errors occur, the save is aborted and any implementation of IVerifierResultNotificationService is notified of the error(s).</summary>
    /// <example>
    /// 	<code title="Example" description="In this example, the implementation of IVerifierResultNotificationService publishes a message to the UI EventAggregator for consumption by any view model and processing of the validation error." lang="CS">
    /// // Create this implementation as a singleton.
    /// [PartCreationPolicy(CreationPolicy.Shared)]
    /// public class VerifierResultNotificationService : IVerifierResultNotificationService
    /// {
    ///     private readonly IEventAggregator _eventAggregator;
    ///  
    ///     #region Implementation of IVerifierResultNotificationService
    ///  
    ///     [ImportingConstructor]
    ///     public VerifierResultNotificationService(IEventAggregator eventAggregator)
    ///     {
    ///         _eventAggregator = eventAggregator;
    ///     }
    ///  
    ///     // Let the UI know that a validation error occured.
    ///     public void OnVerificationError(VerifierResultCollection verifierResultCollection)
    ///     {
    ///         _eventAggregator.Publish(new HandleValidationErrors(verifierResultCollection));
    ///     }
    ///  
    ///     #endregion
    /// }</code>
    /// </example>
    [InterfaceExport(typeof(IVerifierResultNotificationService))]
    public interface IVerifierResultNotificationService
    {
        /// <summary>Method called by the framework if validation errors occured during the save.</summary>
        /// <param name="validationErrors">Collection containing all validation errors.</param>
        void OnVerificationError(VerifierResultCollection validationErrors);
    }
}
