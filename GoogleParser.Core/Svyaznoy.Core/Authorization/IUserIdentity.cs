using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Authorization
{
    public interface IUserIdentity
    {
        Guid Id { get; }

        string Name { get; set; }

        string FullName { get; set; }

        string Email { get; set; }

        IEnumerable<UserRoleType> UserRoles { get; }
    }
}