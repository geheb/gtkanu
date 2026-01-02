using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Entities
{
    public sealed class IdentityUserRoleGuid : IdentityUserRole<Guid>
    {
        public IdentityUserGuid? User { get; set; }
        public IdentityRoleGuid? Role { get; set; }
    }
}
