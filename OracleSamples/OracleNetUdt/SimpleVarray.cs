using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;

namespace OracleSamples.OracleNetUdt
{
    /// <summary>
    /// SimpleVarray Class
    /// An instance of a SimpleVarray class represents an ODP_VARRAY_SAMPLE_TYPE object
    /// A custom type must implement INullable and IOracleCustomType interfaces
    /// Taken from: https://www.codeproject.com/Articles/33829/How-to-use-Oracle-11g-ODP-NET-UDT-in-an-Oracle-Sto
    /// See also: https://stackoverflow.com/questions/980421/c-pass-a-user-defined-type-to-a-oracle-stored-procedure
    /// </summary>
    public class SimpleVarray : IOracleCustomType, INullable
    {
        [OracleArrayMapping()]
        public string[] Array;

        public OracleUdtStatus[] StatusArray { get; set; }

        public bool IsNull { get; private set; }

        public SimpleVarray()
        {
        }

        public SimpleVarray(string[] array)
        {
            Array = array;
        }

        public static SimpleVarray Null
        {
            get
            {
                SimpleVarray obj = new SimpleVarray();
                obj.IsNull = true;
                return obj;
            }
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            Array = (string[])OracleUdt.GetValue(con, pUdt, 0, out object objectStatusArray);
            StatusArray = (OracleUdtStatus[])objectStatusArray;
        }

        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, Array, StatusArray);
        }
    }
}
