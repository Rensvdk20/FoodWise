using Domain;

namespace WebApi.Model
{
    public class PackageViewModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Package Package { get; set; }
    }
}
