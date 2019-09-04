
namespace Siscom.Agua.Api.Model
{
    public class CatalogueVM
    {       
        public string Id { get; set; }
       
        public string Value { get; set; }

        public int GroupCatalogueId { get; set; }

        public GroupCatalogueVM GroupCatalogue { get; set; }
    }
}
