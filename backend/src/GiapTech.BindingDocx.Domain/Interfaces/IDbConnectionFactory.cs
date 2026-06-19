using System.Data;

namespace GiapTech.BindingDocx.Domain.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
