using System;
using System.Collections.Generic;
using System.Data;

namespace OracleSamples.Data
{
    // Notes: 
    // I should probably prefer the reader.GetString/GetInt32 methods over the indexed fields reader[0] so to avoid unboxing/the large type switch statements getting executed in the data provider.
    //
    // A good thing to do when looping over multiple results, cache the ordinals and then check for dbnull on nullable columns
    //   LastName = reader.IsDBNull(field.LastName) ? null : reader.GetString(field.LastName) 

    public static class DataRecordExtensions
    {
        public static IDictionary<string, int> GetOrdinals(this IDataRecord reader, params string[] names)
        {
            var ordinals = new Dictionary<string, int>();

            foreach(var name in names)
            {
                ordinals.Add(name, reader.GetOrdinal(name));
            }

            return ordinals;
        }

        // This library has some reasonable extension methods to help with nullable fields instead of writing my own stuff
        // https://github.com/jehugaleahsa/DataRecordExtensions
        // Examples:
        //   string name = reader.GetNullableString("Name");
        //   string name = reader.GetNullableString("Name", String.Empty); // providing your own default
        //   DateTime creationDate = reader.GetNullableDate("Created") ?? DateTime.MinValue;
        public static string SafeGetString(this IDataRecord reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return null;
        }
    }
}
