﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageMuseumData.DataBaseModel
{
    public class EventType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Events> Events { get; set; }
    }
}