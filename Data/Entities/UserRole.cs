﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoEfP26.Data.Entities
{
    public class UserRole
    {
        public String  Id          { get; set; } = null!;   // "Admin" / "User" / ...
        public String  Description { get; set; } = null!;
        public Boolean CanCreate   { get; set; }   // C
        public Boolean CanRead     { get; set; }   // R
        public Boolean CanUpdate   { get; set; }   // U
        public Boolean CanDelete   { get; set; }   // D

        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}
