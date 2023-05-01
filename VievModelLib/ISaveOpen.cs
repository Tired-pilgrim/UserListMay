using System.Threading.Tasks;

namespace VievModelLib
{
    public interface ISaveOpen
    {
        Task OpenListUserAsync(string path);
        Task SaveListUserAsync(string path);
    }
}
