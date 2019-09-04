using System.Collections.Generic;
namespace Siscom.Agua.Api.Model
{
    public class GroupCatalogueVM
    {
       
        public int Id { get; set; }
        
        public string Name { get; set; }

        public List<CatalogueVM> Catalogues { get; set; }
    }
}
