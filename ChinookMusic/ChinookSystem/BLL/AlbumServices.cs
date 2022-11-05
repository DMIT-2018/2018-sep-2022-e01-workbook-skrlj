﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
#endregion

namespace ChinookSystem.BLL
{
    // Public as it will be called outside the application
    public class AlbumServices
    {
        #region Constructor for Context Dependencies Dependency
        private readonly ChinookContext _context;

        internal AlbumServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion
    }
}
