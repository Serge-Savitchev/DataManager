using DataManagerAPI.Repository.Models;

namespace DataManagerAPI.Repository.Helpers
{
    public static class RolesHelper
    {
        public static Role[] GetAllRoles()
        {
            var roles = new List<Role>();
            foreach (RoleIds s in Enum.GetValues(typeof(RoleIds)))
            {
                roles.Add(new Role { Id = s, Name = s.ToString() });
            }

            return roles.ToArray();
        }

        public static string[] GetAllNames()
        {
            var roles = new List<string>();
            foreach (RoleIds s in Enum.GetValues(typeof(RoleIds)))
            {
                roles.Add(s.ToString());
            }

            return roles.ToArray();
        }
    }
}
