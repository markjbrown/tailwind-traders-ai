using CsvHelper.Configuration;
using System.Collections.Generic;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public interface IFileProcessor
    {
        IEnumerable<TModel> Process<TModel>(string root, string fileName, Configuration cfg = null);
    }
}
