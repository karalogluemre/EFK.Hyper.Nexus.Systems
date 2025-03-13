using Commons.Domain.Models.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Domain.Models.Branches
{
    public class BranchMenu
    {
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
