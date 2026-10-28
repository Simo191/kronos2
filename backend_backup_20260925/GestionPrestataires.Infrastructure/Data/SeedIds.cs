using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionPrestataires.Infrastructure.Data
{
    public static class SeedIds
    {
        // Roles
        public static readonly Guid Role_Admin = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Role_Manager = Guid.Parse("11111111-1111-1111-1111-111111111112");
        public static readonly Guid Role_User = Guid.Parse("11111111-1111-1111-1111-111111111113");

    }
}
