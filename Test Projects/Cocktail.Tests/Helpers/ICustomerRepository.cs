﻿//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.Model;

namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// Interface for the Repository.
    /// 
    /// <seealso cref="CustomerRepository"/>
    /// </summary>
    public interface ICustomerRepository
    {
        event EventHandler<DataChangedEventArgs> DataChanged;

        Task<IEnumerable<Customer>> GetCustomersAsync(string orderByPropertyName);

        void AddCustomer(Customer customer);

        void DeleteCustomer(Customer customer);

        Task SaveAsync();
    }
}