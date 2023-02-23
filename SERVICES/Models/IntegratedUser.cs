using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class IntegratedUser
    {
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string LevelSP { get; set; }

        public IntegratedUser()
        {
            LevelSP = string.Empty;
        }

    }

    public class IntregratedUserComparer: EqualityComparer<IntegratedUser>
    {
        public override bool Equals(IntegratedUser u1, IntegratedUser u2)
        {
            return
                u1.Email.ToUpperInvariant() == u2.Email.ToUpperInvariant() &&
                u1.LevelSP.ToUpperInvariant() == u2.LevelSP.ToUpperInvariant() &&
                u1.LoginName.ToUpperInvariant() == u2.LoginName.ToUpperInvariant();
        }


        public override int GetHashCode(IntegratedUser obj)
        {
            return 0;
        }
    }
}
