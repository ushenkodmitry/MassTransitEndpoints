using System.Dynamic;
using System.Threading.Tasks;

namespace MassTransit.RazorRenderer.Contexts
{
    public interface RendererContext
    {
        Task<string> CompileRender(string templateKey, object model, ExpandoObject viewBag = null);
    }
}