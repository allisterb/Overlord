﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public static class IListExtensions
    {
        public static bool ContainsDevice(this IList<IStorageDevice> list, IStorageDevice device)
        {
            return list.Contains(device, new IStorageDeviceEq());
        }
    }
}
