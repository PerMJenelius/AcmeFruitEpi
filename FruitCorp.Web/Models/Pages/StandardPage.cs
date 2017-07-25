using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace FruitCorp.Web.Models.Pages
{
    [ContentType(DisplayName = "StandardPage", GUID = "71bdf282-d3ff-4eaa-bac8-017cb088a4fa", Description = "The standard page for the site")]
    public class StandardPage : PageData
    {
        public virtual string MainIntro { get; set; }

        public virtual XhtmlString MainBody { get; set; }
    }
}