using System;
using System.Collections.Generic;
using System.Text;

namespace Siscom.Agua.DAL.Enums
{
    public static class TypeAddress
    {
        public enum TypeAddressA : byte
        {
            MainAddress = 0,
            ShipAddress = 1,
            BillAddress = 2
        }
    }
}
