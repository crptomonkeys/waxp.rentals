﻿using System;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Data
{
    public interface IDataCache
    {

        event EventHandler AppStateChanged;
        AppState AppState { get; }

        event EventHandler RecentsChanged;
        Recents Recents { get; }

    }
}
