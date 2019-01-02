using System.Collections.Generic;
using System.Data;

namespace OracleSamples.Model
{
    public class User
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Role> Roles { get; set; }

        public static User FromDataReader(IDataRecord record, IDictionary<string, int> ordinals)
        {
            return new User
            {
                UserId = record.GetString(ordinals["USER_ID"]),
                FirstName = record.GetString(ordinals["FIRST_NAME"]),
                LastName = record.GetString(ordinals["LAST_NAME"])
            };
        }
    }
}
