using System;
using System.Data;


namespace Siscom.Agua.Api.runSp
{
    public class SPParameters: IDisposable
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Size { get; set; }
        public DbType DbType { get; set; }
        public ParameterDirection Direccion { get; set; } = ParameterDirection.Input;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
