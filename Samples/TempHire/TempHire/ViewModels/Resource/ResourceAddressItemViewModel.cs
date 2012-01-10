﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;
using DomainModel;
using IdeaBlade.EntityModel;

namespace TempHire.ViewModels.Resource
{
    public class ResourceAddressItemViewModel : PropertyChangedBase, IDisposable
    {
        private Address _item;

        public ResourceAddressItemViewModel(Address item)
        {
            Debug.Assert(item != null);
            Item = item;
        }

        public Address Item
        {
            get { return _item; }
            private set
            {
                _item = value;
                EntityAspect.Wrap(_item).EntityPropertyChanged += ItemPropertyChanged;

                NotifyOfPropertyChange(() => Item);
            }
        }

        public bool CanDelete
        {
            get { return !Item.Primary && (Item.Resource.Addresses.Count > 1); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            EntityAspect.Wrap(_item).EntityPropertyChanged -= ItemPropertyChanged;
            _item = null;
        }

        #endregion

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == Address.EntityPropertyNames.Primary)
                NotifyOfPropertyChange(() => CanDelete);
        }
    }
}