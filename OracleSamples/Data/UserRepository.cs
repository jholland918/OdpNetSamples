using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Oracle.DataAccess.Client;
using OracleSamples.Model;
using OracleSamples.OracleNetUdt;

namespace OracleSamples.Data
{
    // Some other ideas for mapping results
    // https://github.com/mgravell/fast-member
    // https://www.nuget.org/packages/FastMember.Signed/
    // https://codereview.stackexchange.com/questions/58251/transform-datareader-to-listt-using-reflections
    public class UserRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        #region CRUD Operations

        public User GetById(string userId)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_BY_ID", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("P_USER_ID", OracleDbType.Varchar2).Value = userId;

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    reader.Read();
                    var output = new User();
                    output.UserId = (string)reader["USER_ID"];
                    output.FirstName = (string)reader["FIRST_NAME"];
                    output.LastName = (string)reader["LAST_NAME"];
                    return output;
                }
            }
        }

        public void Insert(User record)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.INS", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_USER_ID", OracleDbType.Varchar2).Value = record.UserId;
                command.Parameters.Add("P_FIRST_NAME", OracleDbType.Varchar2).Value = record.FirstName;
                command.Parameters.Add("P_LAST_NAME", OracleDbType.Varchar2).Value = record.LastName;

                var rowsAffected = command.ExecuteNonQuery(); // rowsAffected returns -1 even when successful, might look into why this is happening
            }
        }

        public void Update(User record)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.UPD", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_USER_ID", OracleDbType.Varchar2).Value = record.UserId;
                command.Parameters.Add("P_FIRST_NAME", OracleDbType.Varchar2).Value = record.FirstName;
                command.Parameters.Add("P_LAST_NAME", OracleDbType.Varchar2).Value = record.LastName;

                var rowsAffected = command.ExecuteNonQuery(); // rowsAffected returns -1 even when successful, might look into why this is happening
            }
        }

        public void Update_CommandBuilder_DeriveParameters(User record)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.UPD", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                // Command Builder can also generate Insert, Update, and Delete commands when given a data adapter.
                // See https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/generating-commands-with-commandbuilders?view=netframework-4.7.2
                OracleCommandBuilder.DeriveParameters(command);
                command.Parameters["P_USER_ID"].Value = record.UserId;
                command.Parameters["P_FIRST_NAME"].Value = record.FirstName;
                command.Parameters["P_LAST_NAME"].Value = record.LastName;

                var rowsAffected = command.ExecuteNonQuery(); // rowsAffected returns -1 even when successful, might look into why this is happening
            }
        }

        public void Delete(string userName)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.DEL", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_USER_ID", OracleDbType.Varchar2).Value = userName;

                var rowsAffected = command.ExecuteNonQuery(); // rowsAffected returns -1 even when successful, might look into why this is happening
            }
        }

        #endregion CRUD Operations

        #region Scalar Queries

        public decimal Count_Sql()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT COUNT(*) FROM APP_USER";
                var count = (decimal)command.ExecuteScalar();
                return count;
            }
        }

        public decimal Count_Procedure()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.COUNT_RECORDS", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                var count = (decimal)command.ExecuteScalar();
                return count;
            }
        }

        #endregion Scalar Queries

        #region Result Set Queries

        // These are just various ways of working with DataTables and DataReaders. Prefer reader over DataTables as it's faster unless you actually need to return a DataTable.

        public DataTable GetAll_DataTable()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            using (var adapter = new OracleDataAdapter(command))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                
                var output = new DataTable();
                adapter.Fill(output);
                
                return output;
            }
        }

        public IEnumerable<User> GetAll_DataTableLinqToList()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            using (var adapter = new OracleDataAdapter(command))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                
                var table = new DataTable();
                adapter.Fill(table);

                var output =
                    from row in table.AsEnumerable()
                    select new User
                    {
                        UserId = row.Field<string>("P_USER_ID"),
                        FirstName = row.Field<string>("P_FIRST_NAME"),
                        LastName = row.Field<string>("P_LAST_NAME")
                    };
                
                return output;
            }
        }

        public IEnumerable<User> GetAll_ReaderToList()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                
                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    while (reader.Read())
                    {
                        // https://stackoverflow.com/questions/41040189/fastest-way-to-map-result-of-sqldatareader-to-object                        
                        var record = new User();
                        record.UserId = (string)reader["USER_ID"];
                        record.FirstName = (string)reader["FIRST_NAME"];
                        record.LastName = (string)reader["LAST_NAME"];
                        output.Add(record);
                    }

                    return output;
                }
            }
        }

        public IEnumerable<User> GetAll_ReaderLinqToList()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                
                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    output = reader.Cast<IDataRecord>()
                        .Select(dr => new User
                        {
                            UserId = (string)dr["USER_ID"],
                            FirstName = (string)dr["FIRST_NAME"],
                            LastName = (string)dr["LAST_NAME"]
                        })
                        .ToList();

                    return output;
                }
            }
        }

        public IEnumerable<User> GetAll_ReaderLinqToListWithOrdinal()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    var field = new
                    {
                        UserId = reader.GetOrdinal("USER_ID"),
                        FirstName = reader.GetOrdinal("FIRST_NAME"),
                        LastName = reader.GetOrdinal("LAST_NAME")
                    };

                    int stockvalue = (reader["StockValue"] as int?).GetValueOrDefault(-1);


                    output = reader.Cast<IDataRecord>()
                        .Select(record => new User
                        {
                            UserId = record.GetString(field.UserId),
                            FirstName = record.GetString(field.FirstName),
                            LastName = record.GetString(field.LastName)
                        })
                        .ToList();

                    return output;
                }
            }
        }

        public IEnumerable<User> GetAll_ReaderLinqWithDtoMapper()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    var ordinals = reader.GetOrdinals("USER_ID", "FIRST_NAME", "LAST_NAME");

                    output = reader.Cast<IDataRecord>()
                        .Select(record => User.FromDataReader(record, ordinals))
                        .ToList();

                    return output;
                }
            }
        }

        #endregion Result Set Queries

        #region Mapping 1:M Joined Results

        public IEnumerable<User> GetAllWithRoles()
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_ALL_WITH_ROLES", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                using (var reader = command.ExecuteReader())
                {
                    IEnumerable<User> output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    // https://stackoverflow.com/questions/25378033/c-sharp-map-one-to-many-object-query-to-complex-type
                    // Alternatively we could probably use Dictionaries or multiple cursors for the value types
                    var field = new
                    {
                        UserId = reader.GetOrdinal("USER_ID"),
                        FirstName = reader.GetOrdinal("FIRST_NAME"),
                        LastName = reader.GetOrdinal("LAST_NAME"),
                        RoleId = reader.GetOrdinal("ROLE_ID")
                    };

                    var rows = reader.Cast<IDataRecord>()
                        .Select(record => new
                        {
                            UserId = record.GetString(field.UserId),
                            FirstName = record.GetString(field.FirstName),
                            LastName = record.GetString(field.LastName),
                            RoleId = record.IsDBNull(field.RoleId) ? null : record.GetString(field.RoleId)
                        });

                    output = rows
                        .GroupBy(r => new { r.UserId, r.FirstName, r.LastName },
                        (key, g) => new User
                        {
                            UserId = key.UserId,
                            FirstName = key.FirstName,
                            LastName = key.LastName,
                            Roles = g.Where(r => r.RoleId != null).Select(r => new Role { RoleId = r.RoleId }).ToList()
                        });

                    return output;
                }
            }
        }
        
        #endregion Mapping 1:M Joined Results

        #region Collection Parameters

        public IEnumerable<User> GetByIds(string[] userIds)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_BY_IDS", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                command.Parameters.Add(new OracleParameter("P_USER_IDS", OracleDbType.Varchar2)
                {
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    Value = userIds
                });

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    while (reader.Read())
                    {
                        var record = new User();
                        record.UserId = (string)reader["USER_ID"];
                        record.FirstName = (string)reader["FIRST_NAME"];
                        record.LastName = (string)reader["LAST_NAME"];
                        output.Add(record);
                    }

                    return output;
                }
            }
        }

        public IEnumerable<User> GetByIds2(string[] userIds)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_BY_IDS2", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(command);
                command.Parameters["P_USER_IDS"].Value = userIds;

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    while (reader.Read())
                    {
                        var record = new User();
                        record.UserId = (string)reader["USER_ID"];
                        record.FirstName = (string)reader["FIRST_NAME"];
                        record.LastName = (string)reader["LAST_NAME"];
                        output.Add(record);
                    }

                    return output;
                }
            }
        }

        public IEnumerable<User> GetByIds3(string[] userIds)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_BY_IDS3", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(command);
                command.Parameters["P_USER_IDS"].Value = userIds;

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    while (reader.Read())
                    {
                        var record = new User();
                        record.UserId = (string)reader["USER_ID"];
                        record.FirstName = (string)reader["FIRST_NAME"];
                        record.LastName = (string)reader["LAST_NAME"];
                        output.Add(record);
                    }

                    return output;
                }
            }
        }

        public IEnumerable<User> GetByIds4(string[] userIds)
        {
            using (var connection = new OracleConnection(connectionString))
            using (var command = new OracleCommand("USER_PACKAGE.GET_BY_IDS4", connection))
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;

                //OracleCommandBuilder.DeriveParameters(command);
                //command.Parameters["P_USER_IDS"].Value = new SimpleVarray(userIds);

                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add(new OracleParameter("P_USER_IDS", OracleDbType.Array)
                {
                    UdtTypeName = "SCOTT.VARCHAR2_1000_VARRAY_T",
                    Value = new SimpleVarray(userIds)
                });

                using (var reader = command.ExecuteReader())
                {
                    var output = new List<User>();

                    if (!reader.HasRows)
                    {
                        return output;
                    }

                    while (reader.Read())
                    {
                        var record = new User();
                        record.UserId = (string)reader["USER_ID"];
                        record.FirstName = (string)reader["FIRST_NAME"];
                        record.LastName = (string)reader["LAST_NAME"];
                        output.Add(record);
                    }

                    return output;
                }
            }
        }

        #endregion Collection Parameters
    }
}
