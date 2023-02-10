using System.Linq;
using suffusive.uveitis.Dependencies.Model;

namespace suffusive.uveitis.Dependencies.Interfaces
{
    public interface IDataContext
    {
        IQueryable<Application> Applications { get; set; }
    }
}