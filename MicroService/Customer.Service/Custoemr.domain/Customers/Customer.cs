﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Domain.Customers
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string  PassWord { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
