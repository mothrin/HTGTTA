using HTGTTA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTGTTA.Models
{
    public class ObjectInterations
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active{get; set; }

        public InteractionTypeEnum InteractionType;
    }
}
