using Oracle.DataAccess.Types;
using System;

namespace OracleSamples.OracleNetUdt
{
    /// <summary>
    /// An instance of the SimpleVarrayFactory class is used to create SimpleVarray objects
    /// See also https://www.codeproject.com/Articles/33829/How-to-use-Oracle-11g-ODP-NET-UDT-in-an-Oracle-Sto
    /// </summary>
    [OracleCustomTypeMapping("SCOTT.VARCHAR2_1000_VARRAY_T")]
    public class SimpleVarrayFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        // IOracleCustomTypeFactory
        public IOracleCustomType CreateObject()
        {
            return new SimpleVarray();
        }

        // IOracleArrayTypeFactory Inteface
        public Array CreateArray(int numElems)
        {
            return new string[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            // CreateStatusArray may return null if null status information 
            // is not required.
            return new OracleUdtStatus[numElems];
        }
    }
}
