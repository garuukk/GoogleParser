using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svyaznoy.Core.Authorization
{
    public class UserIdentity : IUserIdentity
    {
        public static readonly IUserIdentity AnonymousUser = new UserIdentity()
        {
            Id = new Guid("ffff0000-0000-0000-0000-00000000000f"),
            Name = "Anonymous",
            FullName = "Anonymous user"
        };

        public Guid Id { get; private set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserRoleType> UserRoles { get; private set; }
    }
}